using System.Collections;
using UnityEngine;

namespace FirstResponse
{
    public static class KitMotion
    {
        public const float Tight = 0.9f;

        public static Vector3 Loose(Vector3 tight)
        {
            return new Vector3(tight.x, tight.y / Tight, tight.z / Tight);
        }

        public static void Hide(PartHome home)
        {
            if (home.IsValid)
            {
                home.Part.gameObject.SetActive(false);
            }
        }

        public static IEnumerator Drop(PartHome home, float height)
        {
            if (!Show(home))
            {
                yield break;
            }

            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.6f)
            {
                home.Part.localPosition = home.Position + Vector3.up * height * (1f - Mathf.SmoothStep(0f, 1f, t));
                home.Part.localScale = home.Scale * Mathf.Min(1f, t * 4f);
                yield return null;
            }

            home.Restore();
        }

        public static IEnumerator Pop(PartHome home)
        {
            if (!Show(home))
            {
                yield break;
            }

            for (float t = 0f; t < 1f; t += Time.deltaTime / 0.35f)
            {
                home.Part.localScale = home.Scale * (1f + Mathf.Sin(t * Mathf.PI) * 0.25f) * t;
                yield return null;
            }

            home.Restore();
        }

        public static IEnumerator Wrap(PartHome band)
        {
            if (!Show(band))
            {
                yield break;
            }

            Vector3 loose = Loose(band.Scale);
            for (float t = 0f; t < 1f; t += Time.deltaTime / 1.4f)
            {
                float eased = Mathf.SmoothStep(0f, 1f, t);
                band.Part.localRotation = band.Rotation * Quaternion.AngleAxis(720f * (1f - eased), Vector3.right);
                band.Part.localScale = new Vector3(loose.x * eased, loose.y, loose.z) * Mathf.Max(0.05f, eased);
                yield return null;
            }

            band.Restore();
            band.Part.localScale = loose;
        }

        public static IEnumerator Twist(PartHome stick, PartHome band)
        {
            if (!stick.IsValid || !band.IsValid)
            {
                yield break;
            }

            Vector3 loose = Loose(band.Scale);
            for (float t = 0f; t < 1f; t += Time.deltaTime / 2.2f)
            {
                float eased = Mathf.SmoothStep(0f, 1f, t);
                stick.Part.localRotation = Quaternion.AngleAxis(-540f * (1f - eased), Vector3.up) * stick.Rotation;
                band.Part.localScale = Vector3.Lerp(loose, band.Scale, eased);
                yield return null;
            }

            stick.Restore();
            band.Restore();
        }

        public static IEnumerator Sweep(PartHome pen)
        {
            if (!Show(pen))
            {
                yield break;
            }

            for (float t = 0f; t < 1f; t += Time.deltaTime / 1.2f)
            {
                pen.Part.localPosition = pen.Position + Vector3.right * Mathf.Lerp(-0.05f, 0.05f, t) + Vector3.up * Mathf.Abs(Mathf.Sin(t * Mathf.PI * 4f)) * 0.01f;
                yield return null;
            }

            pen.Restore();
            pen.Part.gameObject.SetActive(false);
        }

        public static IEnumerator Grow(PartHome sleeve)
        {
            if (!Show(sleeve))
            {
                yield break;
            }

            for (float t = 0f; t < 1f; t += Time.deltaTime / 1.5f)
            {
                sleeve.Part.localScale = new Vector3(sleeve.Scale.x * Mathf.Max(0.02f, t), sleeve.Scale.y, sleeve.Scale.z);
                yield return null;
            }

            sleeve.Restore();
        }

        private static bool Show(PartHome home)
        {
            if (!home.IsValid)
            {
                return false;
            }

            home.Restore();
            home.Part.gameObject.SetActive(true);
            return true;
        }
    }
}
