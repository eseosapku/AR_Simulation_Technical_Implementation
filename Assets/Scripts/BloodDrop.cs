using System;
using UnityEngine;

namespace FirstResponse
{
    public class BloodDrop : MonoBehaviour, IPoolable
    {
        [SerializeField] private float lifetime = 1.2f;
        [SerializeField] private float startSize = 0.012f;

        private Action<BloodDrop> finished;
        private Vector3 velocity;
        private float age;

        public void Init(Action<BloodDrop> onFinished)
        {
            finished = onFinished;
        }

        public void OnSpawn()
        {
            age = 0f;
            velocity = new Vector3(UnityEngine.Random.Range(-0.08f, 0.08f), UnityEngine.Random.Range(0.05f, 0.2f), UnityEngine.Random.Range(-0.08f, 0.08f));
            transform.localScale = Vector3.one * startSize;
        }

        public void OnDespawn()
        {
            velocity = Vector3.zero;
        }

        private void Update()
        {
            age += Time.deltaTime;
            velocity += Physics.gravity * (0.3f * Time.deltaTime);
            transform.position += velocity * Time.deltaTime;
            transform.localScale = Vector3.one * (startSize * (1f - age / lifetime));
            if (age >= lifetime && finished != null)
            {
                finished(this);
            }
        }
    }
}
