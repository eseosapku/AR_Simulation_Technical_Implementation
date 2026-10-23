using NUnit.Framework;

namespace FirstResponse.Tests
{
    public class BleedingModelTests
    {
        private static BleedingModel Thigh() => new BleedingModel(25f, 1500f, false);

        private static BleedingModel Forearm() => new BleedingModel(5f, 1500f, true);

        [SetUp]
        public void Reset()
        {
            EventBus<BleedingChanged>.Clear();
        }

        [Test]
        public void UntreatedWoundLosesBloodOverTime()
        {
            BleedingModel model = Thigh();
            model.Tick(2f);

            Assert.AreEqual(50f, model.TotalLoss, 0.01f);
            Assert.AreEqual(BleedingState.Uncontrolled, model.State);
        }

        [Test]
        public void PressureSlowsTheBleeding()
        {
            BleedingModel model = Thigh();
            model.PressureStarted();

            Assert.AreEqual(BleedingState.PressureApplied, model.State);
            Assert.Less(model.FlowRate, 25f);
        }

        [Test]
        public void LiftingPressureEarlyRestartsTheBleeding()
        {
            BleedingModel model = Thigh();
            model.PressureStarted();
            model.PressureReleased();

            Assert.AreEqual(BleedingState.Uncontrolled, model.State);
            Assert.AreEqual(25f, model.FlowRate);
        }

        [Test]
        public void HeldPressureControlsAForearmCut()
        {
            BleedingModel model = Forearm();
            model.PressureStarted();
            model.PressureHeld();
            model.Tick(10f);

            Assert.AreEqual(BleedingState.Controlled, model.State);
            Assert.AreEqual(0f, model.TotalLoss);
        }

        [Test]
        public void ThighWoundNeedsATourniquetAfterPressure()
        {
            BleedingModel model = Thigh();
            model.PressureStarted();
            model.PressureHeld();

            Assert.AreEqual(BleedingState.PressureInsufficient, model.State);
            Assert.Greater(model.FlowRate, 0f);
        }

        [Test]
        public void TighteningTheWindlassStopsTheFlow()
        {
            BleedingModel model = Thigh();
            model.PressureStarted();
            model.PressureHeld();
            model.TourniquetPlaced();
            model.Tighten(0.5f);
            float halfway = model.FlowRate;
            model.Tighten(1f);

            Assert.Less(halfway, 15f);
            Assert.AreEqual(BleedingState.Controlled, model.State);
            Assert.AreEqual(0f, model.FlowRate);
        }

        [Test]
        public void TourniquetIsIgnoredOnAControllableBleed()
        {
            BleedingModel model = Forearm();
            model.TourniquetPlaced();

            Assert.AreEqual(BleedingState.Uncontrolled, model.State);
        }

        [Test]
        public void ReachingTheFatalLossEndsTheScenario()
        {
            BleedingModel model = Thigh();
            BleedingState announced = BleedingState.Uncontrolled;
            EventBus<BleedingChanged>.Subscribe(m => announced = m.State);
            model.Tick(100f);

            Assert.AreEqual(BleedingState.Exsanguinated, model.State);
            Assert.AreEqual(BleedingState.Exsanguinated, announced);
            Assert.AreEqual(1f, model.LossFraction);
            Assert.IsTrue(model.IsOver);
        }
    }
}
