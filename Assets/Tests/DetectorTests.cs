using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class DetectorTests
    {
        private MarkerSnapshot snapshot;

        private void Show(MarkerId id, Vector3 position)
        {
            snapshot.Set(id, true, new Pose(position, Quaternion.identity));
        }

        [SetUp]
        public void Create()
        {
            snapshot = new MarkerSnapshot();
            Show(MarkerId.Wound, new Vector3(0f, 0f, 0.30f));
            Show(MarkerId.Joint, Vector3.zero);
        }

        [Test]
        public void CoveringTheWoundCountsAsPressure()
        {
            OcclusionDetector detector = new OcclusionDetector(30f);
            snapshot.Set(MarkerId.Wound, false, default);

            DetectionResult result = detector.Evaluate(snapshot, 15f);

            Assert.IsTrue(result.IsActive);
            Assert.AreEqual(0.5f, result.Progress, 0.01f);
        }

        [Test]
        public void LookingAwayIsNotPressure()
        {
            OcclusionDetector detector = new OcclusionDetector(30f);
            snapshot.Set(MarkerId.Wound, false, default);
            snapshot.Set(MarkerId.Joint, false, default);

            Assert.IsFalse(detector.Evaluate(snapshot, 1f).IsActive);
        }

        [Test]
        public void LiftingTheHandResetsTheHoldTimer()
        {
            OcclusionDetector detector = new OcclusionDetector(30f);
            snapshot.Set(MarkerId.Wound, false, default);
            detector.Evaluate(snapshot, 20f);
            Show(MarkerId.Wound, new Vector3(0f, 0f, 0.30f));

            Assert.AreEqual(0f, detector.Evaluate(snapshot, 1f).Progress);
        }

        [Test]
        public void CoveringASeenWoundCountsWithoutTheJoint()
        {
            OcclusionDetector detector = new OcclusionDetector(30f);
            snapshot.Set(MarkerId.Joint, false, default);
            detector.Evaluate(snapshot, 1f);
            snapshot.Set(MarkerId.Wound, false, default);

            Assert.IsTrue(detector.Evaluate(snapshot, 1f).IsActive);
        }

        [Test]
        public void HoldingTheScreenCountsAsPressure()
        {
            OcclusionDetector detector = new OcclusionDetector(10f);
            snapshot.ScreenHeld = true;

            Assert.IsTrue(detector.Evaluate(snapshot, 10f).IsSatisfied);
        }

        [Test]
        public void HoldingTheScreenPlacesTheBandAndTurnsTheWindlass()
        {
            snapshot.ScreenHeld = true;

            Assert.IsTrue(new PlacementDetector(5f, 7f, 5f).Evaluate(snapshot, 1f).IsSatisfied);
            Assert.IsTrue(new RotationDetector(540f).Evaluate(snapshot, 4.5f).IsSatisfied);
        }

        [Test]
        public void TourniquetSixCentimetresAboveTheWoundIsAccepted()
        {
            PlacementDetector detector = new PlacementDetector(5f, 7f, 5f);
            Show(MarkerId.Tourniquet, new Vector3(0f, 0f, 0.36f));

            Assert.IsTrue(detector.Evaluate(snapshot, 1f).IsSatisfied);
        }

        [Test]
        public void TourniquetOnTheJointSideIsRejected()
        {
            PlacementDetector detector = new PlacementDetector(5f, 7f, 5f);

            Assert.IsFalse(detector.IsValid(6f, 30f, 24f));
            Assert.IsFalse(detector.IsValid(2f, 30f, 32f));
            Assert.IsTrue(detector.IsValid(6f, 30f, 36f));
        }

        [Test]
        public void TwistingTheWindlassAddsUpTurns()
        {
            RotationDetector detector = new RotationDetector(540f);
            for (int i = 0; i <= 12; i++)
            {
                snapshot.Set(MarkerId.Windlass, true, new Pose(Vector3.zero, Quaternion.Euler(0f, i * 45f, 0f)));
                detector.Evaluate(snapshot, 0.1f);
            }

            DetectionResult result = detector.Evaluate(snapshot, 0.1f);

            Assert.AreEqual(540f, result.Measurement, 1f);
            Assert.IsTrue(result.IsSatisfied);
        }
    }
}
