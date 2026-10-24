using System.Collections.Generic;

namespace FirstResponse
{
    public static class ScenarioFactory
    {
        public const int Spurting = 0;
        public const int Oozing = 2;

        public static List<ProcedureStep> Build(ScenarioConfig config)
        {
            bool thigh = config.NeedsTourniquet;
            List<ProcedureStep> steps = new List<ProcedureStep>
            {
                new TapStep("SecureScene", "Secure the scene", "Tap each hazard to make it safe", "ShieldExclamation", 2),
                new ChoiceStep("HandProtection", "Protect yourself", "What goes on your hands?", "HandStop",
                    new[] { "Gloves or plastic bags", "Bare hands" }, 0),
                new ChoiceStep("CallForHelp", "Call for help", "Which number do you dial?", "Volume",
                    new[] { config.emergencyNumber, "911", "Wait and see" }, 0),
                new ChoiceStep("AssessBleeding", "Look at the bleeding", "How is the blood coming out?", "Droplet",
                    new[] { "Spurting", "Pooling", "Oozing" }, thigh ? Spurting : Oozing),
                new ChoiceStep("ChooseIntervention", "Choose your first action", "What do you do first?", "HandStop",
                    new[] { "Direct pressure", "Tourniquet" }, 0, thigh ? -1 : 1),
                new PressureStep()
            };

            if (thigh)
            {
                steps.Add(new TourniquetStep());
                steps.Add(new WindlassStep());
                steps.Add(new TapStep("RecordTime", "Note the time", "Tap to record when the tourniquet went on", "Clock", 1));
            }

            steps.Add(new TapStep("ShockCare", "Treat for shock", "Lay them down and keep them warm", "Bulb", 1));
            return steps;
        }
    }

    public readonly struct SessionFinished
    {
        public readonly SessionRecord Record;
        public readonly string Tip;

        public SessionFinished(SessionRecord record, string tip)
        {
            Record = record;
            Tip = tip;
        }
    }

    public static class Scoring
    {
        public static float Percent(IReadOnlyList<StepResult> results, int totalSteps)
        {
            if (totalSteps == 0)
            {
                return 0f;
            }

            float sum = 0f;
            foreach (StepResult result in results)
            {
                sum += result.score;
            }

            return sum / totalSteps * 100f;
        }

        public static string Outcome(float percent, int criticalErrors, bool casualtyLost, float passScore)
        {
            bool passed = !casualtyLost && criticalErrors == 0 && percent >= passScore;
            return passed ? "Passed" : "Failed";
        }

        public static void Complete(SessionRecord record, List<StepResult> results, int totalSteps, StepContext context, ScenarioConfig config)
        {
            record.endedAt = SessionRecord.Now();
            record.steps = new List<StepResult>(results);
            record.criticalErrors = new List<string>(context.CriticalErrors);
            record.bloodLostMl = context.Bleeding.TotalLoss;
            record.scorePercent = Percent(results, totalSteps);
            bool lost = context.Bleeding.State == BleedingState.Exsanguinated;
            record.outcome = Outcome(record.scorePercent, record.criticalErrors.Count, lost, config.passScore);
            StepResult timed = results.Find(r => r.stepId == "RecordTime" && r.completed);
            if (timed != null)
            {
                record.tourniquetTimeRecorded = record.endedAt;
            }
        }

        public static string Tip(SessionRecord record)
        {
            if (record.criticalErrors.Count > 0)
            {
                return "A tourniquet is only for bleeding that pressure cannot stop.";
            }

            foreach (StepResult step in record.steps)
            {
                if (step.events.Contains("early_release"))
                {
                    return "Hold pressure for the full time before lifting your hand.";
                }
            }

            if (record.bloodLostMl > 1000f)
            {
                return "Act faster. Every second of bleeding counts.";
            }

            return "Well done. Repeat this drill every few months to keep the skill.";
        }
    }
}
