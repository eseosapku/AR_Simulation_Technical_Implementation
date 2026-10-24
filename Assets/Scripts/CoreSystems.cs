using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public static class EventBus<T>
    {
        private static Action<T> handlers;

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

    public interface IPoolable
    {
        void OnSpawn();

        void OnDespawn();
    }

    public class ObjectPool<T> where T : Component, IPoolable
    {
        private readonly Stack<T> idle = new Stack<T>();
        private readonly T prefab;
        private readonly Transform parent;

        public ObjectPool(T prefab, Transform parent, int prewarm)
        {
            this.prefab = prefab;
            this.parent = parent;
            for (int i = 0; i < prewarm; i++)
            {
                T item = Create();
                item.gameObject.SetActive(false);
                idle.Push(item);
            }
        }

        public int IdleCount => idle.Count;

        public int CreatedCount { get; private set; }

        public T Get(Vector3 position, Quaternion rotation)
        {
            T item = idle.Count > 0 ? idle.Pop() : Create();
            item.transform.SetPositionAndRotation(position, rotation);
            item.gameObject.SetActive(true);
            item.OnSpawn();
            return item;
        }

        public void Release(T item)
        {
            if (!item.gameObject.activeSelf)
            {
                return;
            }

            item.OnDespawn();
            item.gameObject.SetActive(false);
            idle.Push(item);
        }

        private T Create()
        {
            CreatedCount++;
            return UnityEngine.Object.Instantiate(prefab, parent);
        }
    }
}
