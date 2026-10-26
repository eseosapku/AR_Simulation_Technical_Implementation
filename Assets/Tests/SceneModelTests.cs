using NUnit.Framework;
using UnityEngine;

namespace FirstResponse.Tests
{
    public class SceneModelTests
    {
        private GameObject host;

        [SetUp]
        public void Create()
        {
            EventBus<HazardSecured>.Clear();
            host = new GameObject("Hazard");
        }

        [TearDown]
        public void Destroy()
        {
            Object.DestroyImmediate(host);
            EventBus<HazardSecured>.Clear();
        }

        [Test]
        public void SecuringAHazardAnnouncesItOnce()
        {
            int heard = 0;
            EventBus<HazardSecured>.Subscribe(m => heard++);
            TrafficHazard cone = host.AddComponent<TrafficHazard>();
            cone.Secure();
            cone.Secure();

            Assert.IsTrue(cone.IsSecured);
            Assert.AreEqual(1, heard);
        }

        [Test]
        public void SecuredHazardsStandUpright()
        {
            host.transform.localRotation = Quaternion.Euler(90f, 45f, 0f);
            host.AddComponent<FuelLeakHazard>().Secure();

            Assert.AreEqual(0f, host.transform.localEulerAngles.x, 0.01f);
            Assert.AreEqual(45f, host.transform.localEulerAngles.y, 0.01f);
        }

        [Test]
        public void EachHazardDescribesItself()
        {
            GameObject other = new GameObject("Other");
            string fuel = other.AddComponent<FuelLeakHazard>().Description;
            Object.DestroyImmediate(other);

            Assert.AreNotEqual(host.AddComponent<TrafficHazard>().Description, fuel);
        }

        [Test]
        public void SceneIsPlacedOnTheFloorInFrontOfTheCamera()
        {
            Pose front = ModelPose.FrontOf(new Vector3(0f, 1.5f, 0f), new Vector3(0f, -0.5f, 1f), 0.2f, 1.3f);

            Assert.AreEqual(0f, front.position.x, 0.001f);
            Assert.AreEqual(0.2f, front.position.y, 0.001f);
            Assert.AreEqual(1.3f, front.position.z, 0.001f);
            Assert.AreEqual(-1f, (front.rotation * Vector3.forward).z, 0.001f);
        }

        [Test]
        public void ModelsAreScaledToTheirTargetSize()
        {
            Assert.AreEqual(0.5f, ModelPose.FitScale(new Vector3(1f, 4f, 2f), 2f), 0.001f);
            Assert.AreEqual(1f, ModelPose.FitScale(Vector3.zero, 2f));
        }
    }
}
