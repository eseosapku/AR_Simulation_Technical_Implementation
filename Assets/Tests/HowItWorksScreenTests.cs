using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class HowItWorksScreenTests
    {
        GameObject host;
        ScreenId requested;

        void OnRequested(ScreenRequested message) => requested = message.Screen;

        [SetUp]
        public void Create()
        {
            EventBus<ScreenRequested>.Clear();
            EventBus<ScreenRequested>.Subscribe(OnRequested);
            requested = ScreenId.Completion;
            host = new GameObject("HowItWorks");
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(host);
            EventBus<ScreenRequested>.Clear();
        }

        [Test]
        public void ClosingReturnsToStart()
        {
            host.AddComponent<HowItWorksScreen>().Close();

            Assert.AreEqual(ScreenId.Start, requested);
        }
    }
}
