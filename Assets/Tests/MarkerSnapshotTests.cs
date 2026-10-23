using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class MarkerSnapshotTests
    {
        [Test]
        public void MarkersStartHidden()
        {
            MarkerSnapshot snapshot = new MarkerSnapshot();

            Assert.IsFalse(snapshot.IsVisible(MarkerId.Wound));
            Assert.IsFalse(snapshot.IsVisible(MarkerId.Joint));
        }

        [Test]
        public void DistanceIsMeasuredInCentimetres()
        {
            MarkerSnapshot snapshot = new MarkerSnapshot();
            snapshot.Set(MarkerId.Wound, true, new Pose(Vector3.zero, Quaternion.identity));
            snapshot.Set(MarkerId.Tourniquet, true, new Pose(new Vector3(0f, 0f, 0.06f), Quaternion.identity));

            Assert.AreEqual(6f, snapshot.DistanceCm(MarkerId.Wound, MarkerId.Tourniquet), 0.01f);
        }

        [Test]
        public void ImageNamesMapToMarkers()
        {
            Assert.IsTrue(MarkerSnapshot.TryParse("MarkerAWound", out MarkerId wound));
            Assert.AreEqual(MarkerId.Wound, wound);
            Assert.IsTrue(MarkerSnapshot.TryParse("MarkerCWindlass", out MarkerId windlass));
            Assert.AreEqual(MarkerId.Windlass, windlass);
            Assert.IsFalse(MarkerSnapshot.TryParse("SomethingElse", out _));
        }
    }
}
