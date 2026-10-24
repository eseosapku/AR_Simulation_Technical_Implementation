using UnityEngine;

namespace FirstResponse
{
    public static class ModelPose
    {
        public static float FitScale(Vector3 boundsSize, float targetSize)
        {
            float largest = Mathf.Max(boundsSize.x, Mathf.Max(boundsSize.y, boundsSize.z));
            return largest > 0.0001f ? targetSize / largest : 1f;
        }

        public static void Fit(Transform body, float targetSize, Transform ground)
        {
            if (!WorldBounds(body.gameObject, out Bounds bounds))
            {
                return;
            }

            body.localScale *= FitScale(bounds.size, targetSize);
            if (WorldBounds(body.gameObject, out Bounds scaled))
            {
                body.position += Vector3.up * (ground.position.y - scaled.min.y);
            }
        }

        public static bool PlayAnimation(GameObject model, string clipHint)
        {
            Animation legacy = model.GetComponentInChildren<Animation>();
            if (legacy == null || string.IsNullOrEmpty(clipHint))
            {
                return false;
            }

            foreach (AnimationState state in legacy)
            {
                if (state.name.ToLowerInvariant().Contains(clipHint.ToLowerInvariant()))
                {
                    state.wrapMode = WrapMode.ClampForever;
                    legacy.Play(state.name);
                    return true;
                }
            }

            return false;
        }

        public static void AddCollider(GameObject model)
        {
            if (!WorldBounds(model, out Bounds bounds))
            {
                return;
            }

            BoxCollider box = model.AddComponent<BoxCollider>();
            box.center = model.transform.InverseTransformPoint(bounds.center);
            Vector3 scale = model.transform.lossyScale;
            box.size = new Vector3(bounds.size.x / scale.x, bounds.size.y / scale.y, bounds.size.z / scale.z) * 1.4f;
        }

        public static bool WorldBounds(GameObject model, out Bounds bounds)
        {
            bounds = default;
            bool found = false;
            foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>())
            {
                if (!found)
                {
                    bounds = renderer.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return found;
        }
    }
}
