using NUnit.Framework;

namespace FirstResponse.Tests
{
    public class PlacementFlowTests
    {
        private PlacementFlow flow;

        [SetUp]
        public void Create()
        {
            flow = new PlacementFlow();
        }

        [Test]
        public void StartsScanningAndWaitsForAFloor()
        {
            Assert.AreEqual(PlacementState.Scanning, flow.State);
            Assert.IsFalse(flow.PlanesFound(0));
            Assert.IsFalse(flow.Tapped(true));
            Assert.AreEqual(PlacementState.Scanning, flow.State);
        }

        [Test]
        public void FindingAFloorLetsTheUserAim()
        {
            Assert.IsTrue(flow.PlanesFound(2));
            Assert.AreEqual(PlacementState.Aiming, flow.State);
        }

        [Test]
        public void TappingOffTheFloorDoesNothing()
        {
            flow.PlanesFound(1);

            Assert.IsFalse(flow.Tapped(false));
            Assert.AreEqual(PlacementState.Aiming, flow.State);
        }

        [Test]
        public void TapThenConfirmLocksTheScene()
        {
            flow.PlanesFound(1);
            flow.Tapped(true);

            Assert.IsTrue(flow.Confirm());
            Assert.AreEqual(PlacementState.Confirmed, flow.State);
            Assert.IsFalse(flow.Tapped(true));
        }

        [Test]
        public void CannotConfirmBeforePlacing()
        {
            flow.PlanesFound(1);

            Assert.IsFalse(flow.Confirm());
        }

        [Test]
        public void MoveUnlocksAConfirmedScene()
        {
            flow.PlanesFound(1);
            flow.Tapped(true);
            flow.Confirm();

            Assert.IsTrue(flow.Move());
            Assert.AreEqual(PlacementState.Previewing, flow.State);
        }
    }
}
