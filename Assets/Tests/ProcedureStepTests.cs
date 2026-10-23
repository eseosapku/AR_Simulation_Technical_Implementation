using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class ProcedureStepTests
    {
        private ScenarioConfig config;
        private StepContext context;

        private static ChoiceStep Intervention()
        {
            string[] options = { "Direct pressure", "Tourniquet" };
            return new ChoiceStep("Intervention", "Choose", "Pick one", "HandStop", options, 0, 1);
        }

        [SetUp]
        public void Create()
        {
            config = ScriptableObject.CreateInstance<ScenarioConfig>();
            context = new StepContext(config, config.CreateBleeding(), new MarkerSnapshot());
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(config);
        }

        [Test]
        public void RightFirstTimeScoresFull()
        {
            ChoiceStep step = Intervention();
            step.Enter(context);

            Assert.IsTrue(step.Choose(0));
            Assert.AreEqual(1f, step.Finish().score);
        }

        [Test]
        public void WrongThenRightScoresHalfAndIsLogged()
        {
            ChoiceStep step = Intervention();
            step.Enter(context);
            step.Choose(1);
            step.Choose(0);

            StepResult result = step.Finish();

            Assert.AreEqual(0.5f, result.score);
            Assert.Contains("wrong_Tourniquet", result.events);
        }

        [Test]
        public void CriticalChoiceIsRecorded()
        {
            ChoiceStep step = Intervention();
            step.Enter(context);
            step.Choose(1);

            Assert.AreEqual(1, context.CriticalErrors.Count);
        }

        [Test]
        public void TapStepNeedsEveryTap()
        {
            TapStep step = new TapStep("SecureScene", "Secure", "Tap hazards", "ShieldExclamation", 2);
            step.Enter(context);
            step.Tap();

            Assert.IsFalse(step.IsComplete);
            step.Tap();
            Assert.IsTrue(step.IsComplete);
        }

        [Test]
        public void TimeStopsWhenTheStepIsDone()
        {
            TapStep step = new TapStep("Call", "Call", "Dial 112", "Volume", 1);
            step.Enter(context);
            step.Tick(context, 2f);
            step.Tap();
            step.Tick(context, 5f);

            Assert.AreEqual(2f, step.Finish().durationSec);
        }

        [Test]
        public void UnfinishedStepScoresZero()
        {
            ChoiceStep step = Intervention();
            step.Enter(context);

            Assert.AreEqual(0f, step.Finish().score);
        }
    }
}
