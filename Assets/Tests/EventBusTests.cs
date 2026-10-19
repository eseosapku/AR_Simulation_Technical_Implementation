using NUnit.Framework;

namespace FirstResponse.Tests
{
    public class EventBusTests
    {
        readonly struct OtherMessage
        {
        }

        ScreenId received;
        int calls;

        void Record(ScreenRequested message)
        {
            received = message.Screen;
            calls++;
        }

        [SetUp]
        public void Reset()
        {
            EventBus<ScreenRequested>.Clear();
            EventBus<OtherMessage>.Clear();
            received = ScreenId.Start;
            calls = 0;
        }

        [Test]
        public void SubscriberReceivesPublishedMessage()
        {
            EventBus<ScreenRequested>.Subscribe(Record);

            EventBus<ScreenRequested>.Publish(new ScreenRequested(ScreenId.Completion));

            Assert.AreEqual(ScreenId.Completion, received);
            Assert.AreEqual(1, calls);
        }

        [Test]
        public void UnsubscribedHandlerIsNotCalled()
        {
            EventBus<ScreenRequested>.Subscribe(Record);
            EventBus<ScreenRequested>.Unsubscribe(Record);

            EventBus<ScreenRequested>.Publish(new ScreenRequested(ScreenId.History));

            Assert.AreEqual(0, calls);
        }

        [Test]
        public void SubscribingTwiceStillCallsOnce()
        {
            EventBus<ScreenRequested>.Subscribe(Record);
            EventBus<ScreenRequested>.Subscribe(Record);

            EventBus<ScreenRequested>.Publish(new ScreenRequested(ScreenId.Setup));

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void PublishingWithNoSubscribersIsSafe()
        {
            Assert.DoesNotThrow(() => EventBus<ScreenRequested>.Publish(new ScreenRequested(ScreenId.Start)));
        }

        [Test]
        public void MessageTypesAreKeptSeparate()
        {
            EventBus<ScreenRequested>.Subscribe(Record);

            EventBus<OtherMessage>.Publish(new OtherMessage());

            Assert.AreEqual(0, calls);
        }
    }
}
