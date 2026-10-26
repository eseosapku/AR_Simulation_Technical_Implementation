using UnityEngine;

namespace FirstResponse
{
    public abstract class PhysicalStep : ProcedureStep
    {
        private IActionDetector detector;

        protected PhysicalStep(string id, string title, string instruction, string icon)
            : base(id, title, instruction, icon)
        {
        }

        public override bool NeedsMarkers => true;

        public DetectionResult Latest { get; private set; }

        protected override void OnEnter(StepContext context)
        {
            detector = CreateDetector(context.Config);
            Latest = default;
        }

        protected override void OnTick(StepContext context, float deltaTime)
        {
            DetectionResult previous = Latest;
            Latest = detector.Evaluate(context.Markers, deltaTime);
            React(context, previous, Latest);
        }

        protected abstract IActionDetector CreateDetector(ScenarioConfig config);

        protected abstract void React(StepContext context, DetectionResult previous, DetectionResult current);
    }

    public class PressureStep : PhysicalStep
    {
        public PressureStep()
            : base("DirectPressure", "Press on the wound", "Cover marker A, or hold the screen", "HandStop")
        {
        }

        protected override IActionDetector CreateDetector(ScenarioConfig config)
        {
            return new OcclusionDetector(config.pressureSeconds);
        }

        protected override void React(StepContext context, DetectionResult previous, DetectionResult current)
        {
            if (current.IsActive && !previous.IsActive)
            {
                context.Bleeding.PressureStarted();
            }
            else if (!current.IsActive && previous.IsActive)
            {
                context.Bleeding.PressureReleased();
                Log("early_release");
            }

            if (current.IsSatisfied)
            {
                context.Bleeding.PressureHeld();
                Complete();
            }
        }

        protected override float Score()
        {
            return Mathf.Max(0f, 1f - 0.25f * Count("early_release"));
        }
    }

    public class TourniquetStep : PhysicalStep
    {
        public TourniquetStep()
            : base("TourniquetPlacement", "Place the tourniquet", "Put strap B on the green line, above the wound and away from the knee", "Scan")
        {
        }

        protected override IActionDetector CreateDetector(ScenarioConfig config)
        {
            return new PlacementDetector(config.tourniquetMinCm, config.tourniquetMaxCm, config.jointClearanceCm);
        }

        protected override void React(StepContext context, DetectionResult previous, DetectionResult current)
        {
            if (current.IsSatisfied)
            {
                Log("distance_" + current.Measurement.ToString("0.0") + "cm");
                context.Bleeding.TourniquetPlaced();
                Complete();
            }
        }

        protected override float Score()
        {
            return 1f;
        }
    }

    public class WindlassStep : PhysicalStep
    {
        private const float FastSeconds = 30f;

        public WindlassStep()
            : base("WindlassTightening", "Twist the windlass", "Turn marker C like a dial until the bleeding stops", "Rotate")
        {
        }

        protected override IActionDetector CreateDetector(ScenarioConfig config)
        {
            return new RotationDetector(config.windlassDegrees);
        }

        protected override void React(StepContext context, DetectionResult previous, DetectionResult current)
        {
            context.Bleeding.Tighten(current.Progress);
            if (context.Bleeding.State == BleedingState.Controlled)
            {
                Complete();
            }
        }

        protected override float Score()
        {
            return Elapsed <= FastSeconds ? 1f : 0.7f;
        }
    }
}
