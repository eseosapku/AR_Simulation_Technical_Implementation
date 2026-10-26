using UnityEngine;

namespace FirstResponse
{
    public readonly struct DetectionResult
    {
        public readonly bool IsActive;
        public readonly float Progress;
        public readonly float Measurement;

        public DetectionResult(bool isActive, float progress, float measurement)
        {
            IsActive = isActive;
            Progress = Mathf.Clamp01(progress);
            Measurement = measurement;
        }

        public bool IsSatisfied => Progress >= 1f;
    }

    public interface IActionDetector
    {
        DetectionResult Evaluate(MarkerSnapshot snapshot, float deltaTime);

        void Reset();
    }

    public class OcclusionDetector : IActionDetector
    {
        private readonly float requiredSeconds;
        private float heldSeconds;
        private bool woundSeen;

        public OcclusionDetector(float requiredSeconds)
        {
            this.requiredSeconds = requiredSeconds;
        }

        public DetectionResult Evaluate(MarkerSnapshot snapshot, float deltaTime)
        {
            bool woundVisible = snapshot.IsVisible(MarkerId.Wound);
            woundSeen = woundSeen || woundVisible;
            bool hidden = !woundVisible && (woundSeen || snapshot.IsVisible(MarkerId.Joint));
            bool covered = snapshot.ScreenHeld || hidden;
            heldSeconds = covered ? heldSeconds + deltaTime : 0f;
            return new DetectionResult(covered, heldSeconds / requiredSeconds, heldSeconds);
        }

        public void Reset()
        {
            heldSeconds = 0f;
            woundSeen = false;
        }
    }

    public class PlacementDetector : IActionDetector
    {
        private const float SteadySeconds = 1f;
        private readonly float minCm;
        private readonly float maxCm;
        private readonly float jointClearanceCm;
        private float steadySeconds;

        public PlacementDetector(float minCm, float maxCm, float jointClearanceCm)
        {
            this.minCm = minCm;
            this.maxCm = maxCm;
            this.jointClearanceCm = jointClearanceCm;
        }

        public DetectionResult Evaluate(MarkerSnapshot snapshot, float deltaTime)
        {
            bool seen = snapshot.IsVisible(MarkerId.Wound) && snapshot.IsVisible(MarkerId.Tourniquet) && snapshot.IsVisible(MarkerId.Joint);
            float fromWound = seen ? snapshot.DistanceCm(MarkerId.Wound, MarkerId.Tourniquet) : 0f;
            bool valid = seen && IsValid(fromWound, snapshot.DistanceCm(MarkerId.Wound, MarkerId.Joint), snapshot.DistanceCm(MarkerId.Tourniquet, MarkerId.Joint));
            if (snapshot.ScreenHeld && !valid)
            {
                valid = true;
                fromWound = (minCm + maxCm) * 0.5f;
            }

            steadySeconds = valid ? steadySeconds + deltaTime : 0f;
            return new DetectionResult(seen || snapshot.ScreenHeld, steadySeconds / SteadySeconds, fromWound);
        }

        public bool IsValid(float woundToBand, float woundToJoint, float bandToJoint)
        {
            bool rightDistance = woundToBand >= minCm && woundToBand <= maxCm;
            bool aboveWound = bandToJoint > woundToJoint;
            bool clearOfJoint = bandToJoint >= jointClearanceCm;
            return rightDistance && aboveWound && clearOfJoint;
        }

        public void Reset()
        {
            steadySeconds = 0f;
        }
    }

    public class RotationDetector : IActionDetector
    {
        private const float MaxStepDegrees = 90f;
        private const float HoldDegreesPerSecond = 120f;
        private readonly float targetDegrees;
        private float turnedDegrees;
        private Quaternion lastRotation;
        private bool hasLast;

        public RotationDetector(float targetDegrees)
        {
            this.targetDegrees = targetDegrees;
        }

        public DetectionResult Evaluate(MarkerSnapshot snapshot, float deltaTime)
        {
            bool seen = snapshot.IsVisible(MarkerId.Windlass);
            if (seen)
            {
                Quaternion current = snapshot.Rotation(MarkerId.Windlass);
                float step = hasLast ? Quaternion.Angle(lastRotation, current) : 0f;
                if (step < MaxStepDegrees)
                {
                    turnedDegrees += step;
                }

                lastRotation = current;
                hasLast = true;
            }

            if (snapshot.ScreenHeld)
            {
                turnedDegrees += HoldDegreesPerSecond * deltaTime;
            }

            return new DetectionResult(seen || snapshot.ScreenHeld, turnedDegrees / targetDegrees, turnedDegrees);
        }

        public void Reset()
        {
            turnedDegrees = 0f;
            hasLast = false;
        }
    }
}
