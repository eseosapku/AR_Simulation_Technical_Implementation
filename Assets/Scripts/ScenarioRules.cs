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
                new CardStep("DirectPressure", "Press on the wound", "Show card A: the gauze pad you press with", "HandStop",
                    MarkerId.Wound, "Pressure", Treatment.Pressure)
            };

            if (thigh)
            {
                steps.Add(new ChoiceStep("ImproviseTourniquet", "Pressure is failing", "No tourniquet kit. What do you make one from?", "HandStop",
                    new[] { "Cloth folded 5 cm wide and a stick", "Thin rope or wire", "Elastic hair band" }, 0, 1));
                steps.Add(new CardStep("FetchCloth", "Get the cloth", "Show card B: the 5 cm cloth band", "Scan",
                    MarkerId.Tourniquet, "Cloth", Treatment.None));
                steps.Add(new DemoStep("TourniquetPlacement", "Place the band", "Where does the band go?", "Scan",
                    new[] { "Right over the wound", "Over the knee", "5 to 7 cm above the wound" }, 2, 1, "Wrap", Treatment.Tourniquet));
                steps.Add(new DemoStep("TieKnot", "Start the knot", "How do you tie the band?", "HandStop",
                    new[] { "Half knot on top of the leg", "Tie it under the leg", "Just tuck the end in" }, 0, -1, "Knot", Treatment.None));
                steps.Add(new CardStep("FetchStick", "Add the stick", "Show card C: the stick goes on the half knot", "Rotate",
                    MarkerId.Windlass, "Stick", Treatment.None));
                steps.Add(new DemoStep("WindlassTightening", "Tighten", "How far do you twist the stick?", "Rotate",
                    new[] { "One turn only", "Until it feels snug", "Until the bleeding stops" }, 2, -1, "Twist", Treatment.Tighten));
                steps.Add(new DemoStep("LockWindlass", "Secure the stick", "How do you stop it unwinding?", "HandStop",
                    new[] { "Hold it in your hand", "Let it go", "Tie both stick ends down" }, 2, 1, "Lock", Treatment.None));
                steps.Add(new CardStep("RecordTime", "Record the time", "Show card D: the pen writes TK and the time", "Clock",
                    MarkerId.Joint, "Mark", Treatment.None));
                steps.Add(new ChoiceStep("KeepOn", "Leave it on", "When can the tourniquet come off?", "Clock",
                    new[] { "After 10 minutes", "Only at the hospital", "When the bleeding stops" }, 1));
            }
            else
            {
                steps.Add(new CardStep("Dressing", "Bandage the wound", "Show card B: roll the bandage over the pad", "Droplet",
                    MarkerId.Tourniquet, "Bandage", Treatment.None));
                steps.Add(new ChoiceStep("CheckCirculation", "Check the hand", "The fingers turn cold and blue. What now?", "Bulb",
                    new[] { "Leave it alone", "Loosen the bandage a little", "Add another bandage" }, 1));
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
