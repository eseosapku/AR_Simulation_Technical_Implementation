using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class UIScreenTests
    {
        class FakeScreen : UIScreen
        {
            public int ShownCount;
            public int HiddenCount;

            public override ScreenId Id => ScreenId.Start;

            protected override void OnShown() => ShownCount++;

            protected override void OnHidden() => HiddenCount++;
        }

        GameObject host;
        FakeScreen screen;
        CanvasGroup group;

        [SetUp]
        public void Create()
        {
            host = new GameObject("Screen");
            screen = host.AddComponent<FakeScreen>();
            group = host.GetComponent<CanvasGroup>();
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(host);
        }

        [Test]
        public void ShowMakesScreenVisibleAndTouchable()
        {
            screen.Hide();
            screen.Show();

            Assert.IsTrue(screen.IsVisible);
            Assert.AreEqual(1f, group.alpha);
            Assert.IsTrue(group.interactable);
            Assert.IsTrue(group.blocksRaycasts);
        }

        [Test]
        public void HideMakesScreenInvisibleAndUntouchable()
        {
            screen.Show();
            screen.Hide();

            Assert.IsFalse(screen.IsVisible);
            Assert.AreEqual(0f, group.alpha);
            Assert.IsFalse(group.interactable);
            Assert.IsFalse(group.blocksRaycasts);
        }

        [Test]
        public void ShowAndHideRunTheirHooksOnce()
        {
            screen.Show();
            screen.Hide();

            Assert.AreEqual(1, screen.ShownCount);
            Assert.AreEqual(1, screen.HiddenCount);
        }
    }
}
