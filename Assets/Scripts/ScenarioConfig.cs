using UnityEngine;

namespace FirstResponse
{
    [CreateAssetMenu(fileName = "Scenario", menuName = "First Response/Scenario Config")]
    public class ScenarioConfig : ScriptableObject
    {
        [Header("Identity")]
        public ScenarioId scenario;
        public string displayName = "Thigh wound";
        [TextArea] public string briefing;

        [Header("Bleeding")]
        [Min(0.1f)] public float baseFlowMlPerSec = 25f;
        [Min(1f)] public float fatalLossMl = 1500f;
        public bool pressureIsEnough;

        [Header("Physical actions")]
        [Min(1f)] public float pressureSeconds = 30f;
        public float tourniquetMinCm = 5f;
        public float tourniquetMaxCm = 7f;
        public float jointClearanceCm = 5f;
        [Min(90f)] public float windlassDegrees = 540f;

        [Header("Scoring")]
        [Range(0f, 100f)] public float passScore = 80f;
        public string emergencyNumber = "112";

        public BleedingModel CreateBleeding()
        {
            return new BleedingModel(baseFlowMlPerSec, fatalLossMl, pressureIsEnough);
        }

        public bool NeedsTourniquet => !pressureIsEnough;
    }
}
