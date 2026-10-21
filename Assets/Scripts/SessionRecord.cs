using System;
using System.Collections.Generic;

namespace FirstResponse
{
    [Serializable]
    public class SessionRecord
    {
        public const int CurrentSchema = 1;

        public int schemaVersion = CurrentSchema;
        public string sessionId = Guid.NewGuid().ToString();
        public string traineeId;
        public string scenarioId;
        public string startedAt;
        public string endedAt;
        public string outcome;
        public float scorePercent;
        public float timeToControlSec;
        public float bloodLostMl;
        public string tourniquetTimeRecorded;
        public List<string> criticalErrors = new List<string>();
        public List<StepResult> steps = new List<StepResult>();

        public bool Passed => outcome == "Passed";

        public static string Now() => DateTime.UtcNow.ToString("o");
    }

    [Serializable]
    public class StepResult
    {
        public string stepId;
        public bool completed;
        public float durationSec;
        public float score;
        public List<string> events = new List<string>();
    }
}
