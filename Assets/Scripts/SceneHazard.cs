using UnityEngine;

namespace FirstResponse
{
    public readonly struct HazardSecured
    {
        public readonly string Description;

        public HazardSecured(string description)
        {
            Description = description;
        }
    }

    public abstract class SceneHazard : MonoBehaviour
    {
        private const float PulseSpeed = 4f;
        private const float PulseAmount = 0.06f;

        private Vector3 baseScale;
        private Collider body;

        public bool IsSecured { get; private set; }

        public abstract string Description { get; }

        private void Start()
        {
            baseScale = transform.localScale;
            body = GetComponent<Collider>();
        }

        private void Update()
        {
            if (IsSecured)
            {
                return;
            }

            transform.localScale = baseScale * (1f + Mathf.Sin(Time.time * PulseSpeed) * PulseAmount);
            if (ScreenTap.TryGet(out Vector2 point) && Hit(point))
            {
                Secure();
            }
        }

        public void Secure()
        {
            if (IsSecured)
            {
                return;
            }

            IsSecured = true;
            if (baseScale != Vector3.zero)
            {
                transform.localScale = baseScale;
            }

            OnSecured();
            EventBus<HazardSecured>.Publish(new HazardSecured(Description));
        }

        protected abstract void OnSecured();

        protected void StandUpright()
        {
            transform.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, 0f);
        }

        private bool Hit(Vector2 point)
        {
            Camera view = Camera.main;
            if (view == null || body == null)
            {
                return false;
            }

            Ray ray = view.ScreenPointToRay(point);
            return Physics.Raycast(ray, out RaycastHit hit, 10f) && hit.collider == body;
        }
    }
}
