using System.Collections.Generic;

namespace FirstResponse
{
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
