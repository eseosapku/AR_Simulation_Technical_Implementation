using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class PhysicalStepTests
    {
        private ScenarioConfig config;
        private StepContext context;
        private MarkerSnapshot markers;

        private void Show(MarkerId id, Vector3 position)
        {
            markers.Set(id, true, new Pose(position, Quaternion.identity));
        }

        [SetUp]
        public void Create()
        {
            EventBus<BleedingChanged>.Clear();
            config = ScriptableObject.CreateInstance<ScenarioConfig>();
            config.pressureSeconds = 10f;
            markers = new MarkerSnapshot();
            context = new StepContext(config, config.CreateBleeding(), markers);
            Show(MarkerId.Wound, new Vector3(0f, 0f, 0.3f));
            Show(MarkerId.Joint, Vector3.zero);
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(config);
        }

        [Test]
        public void HoldingPressureLongEnoughCompletesTheStep()
        {
            PressureStep step = new PressureStep();
            step.Enter(context);
            markers.Set(MarkerId.Wound, false, default);
            step.Tick(context, 4f);

            Assert.AreEqual(BleedingState.PressureApplied, context.Bleeding.State);
            step.Tick(context, 7f);
            Assert.IsTrue(step.IsComplete);
            Assert.AreEqual(BleedingState.PressureInsufficient, context.Bleeding.State);
        }

        [Test]
        public void LiftingEarlyIsPenalised()
        {
            PressureStep step = new PressureStep();
            step.Enter(context);
            markers.Set(MarkerId.Wound, false, default);
            step.Tick(context, 2f);
            Show(MarkerId.Wound, new Vector3(0f, 0f, 0.3f));
            step.Tick(context, 0.1f);
            markers.Set(MarkerId.Wound, false, default);
            step.Tick(context, 11f);

            StepResult result = step.Finish();
            Assert.Contains("early_release", result.events);
            Assert.AreEqual(0.75f, result.score);
        }

        [Test]
        public void TourniquetThenWindlassControlsTheBleed()
        {
            context.Bleeding.PressureStarted();
            context.Bleeding.PressureHeld();
            TourniquetStep place = new TourniquetStep();
            place.Enter(context);
            Show(MarkerId.Tourniquet, new Vector3(0f, 0f, 0.36f));
            place.Tick(context, 1.1f);

            Assert.IsTrue(place.IsComplete);
            WindlassStep twist = new WindlassStep();
            twist.Enter(context);
            for (int i = 0; i <= 12; i++)
            {
                markers.Set(MarkerId.Windlass, true, new Pose(Vector3.zero, Quaternion.Euler(0f, i * 45f, 0f)));
                twist.Tick(context, 0.5f);
            }

            Assert.IsTrue(twist.IsComplete);
            Assert.AreEqual(BleedingState.Controlled, context.Bleeding.State);
        }
    }
}
