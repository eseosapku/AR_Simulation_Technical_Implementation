using System;

namespace FirstResponse
{
    public static class EventBus<T>
    {
        static Action<T> handlers;

        public static void Subscribe(Action<T> handler)
        {
            handlers -= handler;
            handlers += handler;
        }

        public static void Unsubscribe(Action<T> handler)
        {
            handlers -= handler;
        }

        public static void Publish(T message)
        {
            handlers?.Invoke(message);
        }

        public static void Clear()
        {
            handlers = null;
        }
    }
}
