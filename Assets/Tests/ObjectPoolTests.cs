using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class ObjectPoolTests
    {
        private GameObject template;
        private GameObject parent;
        private BloodDrop prefab;

        [SetUp]
        public void Create()
        {
            template = new GameObject("DropTemplate");
            prefab = template.AddComponent<BloodDrop>();
            parent = new GameObject("Pool");
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(parent);
            Object.DestroyImmediate(template);
        }

        [Test]
        public void PrewarmCreatesIdleDrops()
        {
            ObjectPool<BloodDrop> pool = new ObjectPool<BloodDrop>(prefab, parent.transform, 5);

            Assert.AreEqual(5, pool.IdleCount);
            Assert.AreEqual(5, pool.CreatedCount);
        }

        [Test]
        public void ReleasedDropIsReusedNotRecreated()
        {
            ObjectPool<BloodDrop> pool = new ObjectPool<BloodDrop>(prefab, parent.transform, 1);
            BloodDrop first = pool.Get(Vector3.zero, Quaternion.identity);
            pool.Release(first);
            BloodDrop second = pool.Get(Vector3.one, Quaternion.identity);

            Assert.AreSame(first, second);
            Assert.AreEqual(1, pool.CreatedCount);
            Assert.AreEqual(Vector3.one, second.transform.position);
        }

        [Test]
        public void ReleaseHidesAndReturnsTheDrop()
        {
            ObjectPool<BloodDrop> pool = new ObjectPool<BloodDrop>(prefab, parent.transform, 1);
            BloodDrop drop = pool.Get(Vector3.zero, Quaternion.identity);
            pool.Release(drop);
            pool.Release(drop);

            Assert.IsFalse(drop.gameObject.activeSelf);
            Assert.AreEqual(1, pool.IdleCount);
        }

        [Test]
        public void EmptyPoolGrowsWhenNeeded()
        {
            ObjectPool<BloodDrop> pool = new ObjectPool<BloodDrop>(prefab, parent.transform, 0);
            pool.Get(Vector3.zero, Quaternion.identity);
            pool.Get(Vector3.zero, Quaternion.identity);

            Assert.AreEqual(2, pool.CreatedCount);
        }
    }
}
