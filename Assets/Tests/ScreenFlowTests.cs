using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class ScreenFlowTests
    {
        GameObject host;
        ScreenId requested;
        ScenarioId chosen;

        void OnRequested(ScreenRequested message) => requested = message.Screen;

        void OnChosen(ScenarioChosen message) => chosen = message.Scenario;

        [SetUp]
        public void Create()
        {
            EventBus<ScreenRequested>.Clear();
            EventBus<ScenarioChosen>.Clear();
            EventBus<ScreenRequested>.Subscribe(OnRequested);
            EventBus<ScenarioChosen>.Subscribe(OnChosen);
            requested = ScreenId.Paused;
            chosen = ScenarioId.Forearm;
            host = new GameObject("Screen");
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(host);
            EventBus<ScreenRequested>.Clear();
            EventBus<ScenarioChosen>.Clear();
        }

        [Test]
        public void ChoosingAScenarioAnnouncesIt()
        {
            host.AddComponent<ScenarioSelectScreen>().Choose(ScenarioId.Thigh);

            Assert.AreEqual(ScenarioId.Thigh, chosen);
        }

        [Test]
        public void ChoosingAScenarioMovesToTheSafetyNotice()
        {
            host.AddComponent<ScenarioSelectScreen>().Choose(ScenarioId.Forearm);

            Assert.AreEqual(ScreenId.SafetyNotice, requested);
        }

        [Test]
        public void AcknowledgingSafetyMovesToArSetup()
        {
            host.AddComponent<SafetyNoticeScreen>().Acknowledge();

            Assert.AreEqual(ScreenId.Setup, requested);
        }

        [Test]
        public void ClosingHowItWorksReturnsToStart()
        {
            host.AddComponent<HowItWorksScreen>().Close();

            Assert.AreEqual(ScreenId.Start, requested);
        }
    }
}
