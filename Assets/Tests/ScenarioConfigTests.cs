using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class ScenarioConfigTests
    {
        private ScenarioConfig config;

        [SetUp]
        public void Create()
        {
            config = ScriptableObject.CreateInstance<ScenarioConfig>();
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(config);
        }

        [Test]
        public void BleedingUsesTheConfiguredFlow()
        {
            config.baseFlowMlPerSec = 5f;
            BleedingModel model = config.CreateBleeding();
            model.Tick(1f);

            Assert.AreEqual(5f, model.TotalLoss, 0.01f);
        }

        [Test]
        public void EachScenarioGetsItsOwnFreshModel()
        {
            BleedingModel first = config.CreateBleeding();
            first.Tick(3f);

            Assert.AreEqual(0f, config.CreateBleeding().TotalLoss);
        }

        [Test]
        public void ThighStyleConfigNeedsATourniquet()
        {
            config.pressureIsEnough = false;

            Assert.IsTrue(config.NeedsTourniquet);
        }
    }
}
