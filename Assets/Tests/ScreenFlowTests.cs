using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class ScreenFlowTests
    {
        private GameObject host;
        private ScreenId requested;
        private ScenarioId chosen;

        private void OnRequested(ScreenRequested message) => requested = message.Screen;

        private void OnChosen(ScenarioChosen message) => chosen = message.Scenario;

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

        private static string Stamp(int day, int hour, int minute)
        {
            return new System.DateTime(2026, 10, day, hour, minute, 0, System.DateTimeKind.Local).ToUniversalTime().ToString("o");
        }

        [Test]
        public void HistoryShowsFriendlyDates()
        {
            System.DateTime now = new System.DateTime(2026, 10, 21, 18, 0, 0, System.DateTimeKind.Local);

            Assert.AreEqual("Today, 10:06", HistoryScreen.WhenText(Stamp(21, 10, 6), now));
            Assert.AreEqual("Yesterday", HistoryScreen.WhenText(Stamp(20, 9, 0), now));
            Assert.AreEqual("Oct 17", HistoryScreen.WhenText(Stamp(17, 9, 0), now));
            Assert.AreEqual("Unknown date", HistoryScreen.WhenText("garbage", now));
        }

        [Test]
        public void HudClockShowsMinutesAndSeconds()
        {
            Assert.AreEqual("00:07", HudStatus.Clock(7.9f));
            Assert.AreEqual("01:12", HudStatus.Clock(72f));
        }

        [Test]
        public void HistoryShowsScenarioNames()
        {
            Assert.AreEqual("Thigh wound", HistoryScreen.ScenarioName("Thigh"));
            Assert.AreEqual("Forearm cut", HistoryScreen.ScenarioName("Forearm"));
        }
    }
}
