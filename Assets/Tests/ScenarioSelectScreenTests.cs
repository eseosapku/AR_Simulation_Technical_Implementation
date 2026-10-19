using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class ScenarioSelectScreenTests
    {
        GameObject host;
        ScenarioSelectScreen screen;
        ScenarioId chosen;
        ScreenId requested;

        void OnChosen(ScenarioChosen message) => chosen = message.Scenario;

        void OnRequested(ScreenRequested message) => requested = message.Screen;

        [SetUp]
        public void Create()
        {
            EventBus<ScenarioChosen>.Clear();
            EventBus<ScreenRequested>.Clear();
            EventBus<ScenarioChosen>.Subscribe(OnChosen);
            EventBus<ScreenRequested>.Subscribe(OnRequested);
            chosen = ScenarioId.Forearm;
            requested = ScreenId.Start;
            host = new GameObject("ScenarioSelect");
            screen = host.AddComponent<ScenarioSelectScreen>();
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(host);
            EventBus<ScenarioChosen>.Clear();
            EventBus<ScreenRequested>.Clear();
        }

        [Test]
        public void ChoosingAnnouncesTheScenario()
        {
            screen.Choose(ScenarioId.Thigh);

            Assert.AreEqual(ScenarioId.Thigh, chosen);
        }

        [Test]
        public void ChoosingMovesToTheSafetyNotice()
        {
            screen.Choose(ScenarioId.Forearm);

            Assert.AreEqual(ScreenId.SafetyNotice, requested);
        }
    }
}
