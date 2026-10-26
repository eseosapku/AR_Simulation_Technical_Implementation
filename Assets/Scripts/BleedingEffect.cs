using UnityEngine;

namespace FirstResponse
{
    public class BleedingEffect : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulation;
        [SerializeField] private ARTrackingManager tracking;
        [SerializeField] private Transform fallbackWound;
        [SerializeField] private BloodDrop dropPrefab;
        [SerializeField] private int prewarm = 40;
        [SerializeField] private float dropsPerMl = 0.8f;

        private const float WoundHeight = 0.07f;
        private ObjectPool<BloodDrop> pool;
        private float owed;
        private bool woundSeen;

        public ObjectPool<BloodDrop> Pool => pool;

        private void Awake()
        {
            pool = new ObjectPool<BloodDrop>(dropPrefab, transform, prewarm);
        }

        private void Update()
        {
            BleedingModel bleeding = simulation.Bleeding;
            if (bleeding == null || bleeding.IsOver || Time.timeScale == 0f)
            {
                return;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetHeartbeat(bleeding.LossFraction);
            }

            owed += bleeding.FlowRate * dropsPerMl * Time.deltaTime;
            Vector3 wound = WoundPosition();
            while (owed >= 1f)
            {
                owed -= 1f;
                BloodDrop drop = pool.Get(wound, Quaternion.identity);
                drop.Init(Return);
            }
        }

        public void SetWound(Transform wound)
        {
            fallbackWound = wound;
            woundSeen = false;
        }

        private Vector3 WoundPosition()
        {
            MarkerSnapshot markers = tracking.Snapshot;
            if (fallbackWound == null && markers.IsVisible(MarkerId.Wound))
            {
                woundSeen = true;
            }

            if (woundSeen)
            {
                return markers.Position(MarkerId.Wound) + markers.Rotation(MarkerId.Wound) * Vector3.up * WoundHeight;
            }

            return fallbackWound != null ? fallbackWound.position : transform.position;
        }

        private void Return(BloodDrop drop)
        {
            pool.Release(drop);
        }
    }
}
