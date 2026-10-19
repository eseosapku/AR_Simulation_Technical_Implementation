using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class SafetyNoticeScreenTests
    {
        GameObject host;
        ScreenId requested;

        void OnRequested(ScreenRequested message) => requested = message.Screen;

        [SetUp]
        public void Create()
        {
            EventBus<ScreenRequested>.Clear();
            EventBus<ScreenRequested>.Subscribe(OnRequested);
            requested = ScreenId.Start;
            host = new GameObject("SafetyNotice");
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(host);
            EventBus<ScreenRequested>.Clear();
        }

        [Test]
        public void AcknowledgingMovesToArSetup()
        {
            host.AddComponent<SafetyNoticeScreen>().Acknowledge();

            Assert.AreEqual(ScreenId.Setup, requested);
        }
    }
}
