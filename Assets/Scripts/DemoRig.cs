using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public readonly struct PartHome
    {
        public readonly Transform Part;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;

        public PartHome(Transform part)
        {
            Part = part;
            Position = part.localPosition;
            Rotation = part.localRotation;
            Scale = part.localScale;
        }

        public bool IsValid => Part != null;

        public void Restore()
        {
            Part.localPosition = Position;
            Part.localRotation = Rotation;
            Part.localScale = Scale;
        }
    }

    public class KitPlayer
    {
        private static readonly string[] Parts = { "Pad", "Strip", "Band", "Tails", "KnotHalf", "Stick", "KnotFull", "TieLeft", "TieRight", "TimeTag", "Pen", "Sleeve" };
        private readonly Dictionary<string, PartHome> homes = new Dictionary<string, PartHome>();
        private GameObject root;

        public Transform Root => root == null ? null : root.transform;

        public Transform Focus { get; private set; }

        public void Spawn(GameObject model, Transform parent, Material template)
        {
            Clear();
            root = Object.Instantiate(model, parent);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            ModelPaint.Apply(root, template);
            foreach (string partName in Parts)
            {
                Transform part = ModelPaint.FindEnding(root.transform, partName);
                if (part != null)
                {
                    homes[partName] = new PartHome(part);
                }
            }

            Focus = ModelPaint.FindEnding(root.transform, "Wound");
            Reset();
        }

        public void Reset()
        {
            foreach (PartHome home in homes.Values)
            {
                home.Restore();
                home.Part.gameObject.SetActive(false);
            }
        }

        public bool IsShowing(string partName)
        {
            return homes.TryGetValue(partName, out PartHome home) && home.Part.gameObject.activeSelf;
        }

        public IEnumerator Play(string clip)
        {
            switch (clip)
            {
                case "Pressure":
                    yield return KitMotion.Drop(Home("Pad"), 0.2f);
                    break;
                case "Cloth":
                    yield return KitMotion.Drop(Home("Strip"), 0.15f);
                    break;
                case "Wrap":
                    KitMotion.Hide(Home("Strip"));
                    yield return KitMotion.Wrap(Home("Band"));
                    yield return KitMotion.Pop(Home("Tails"));
                    break;
                case "Knot":
                    yield return KitMotion.Pop(Home("KnotHalf"));
                    break;
                case "Stick":
                    yield return KitMotion.Drop(Home("Stick"), 0.2f);
                    yield return KitMotion.Pop(Home("KnotFull"));
                    break;
                case "Twist":
                    yield return KitMotion.Twist(Home("Stick"), Home("Band"));
                    break;
                case "Lock":
                    yield return KitMotion.Pop(Home("TieLeft"));
                    yield return KitMotion.Pop(Home("TieRight"));
                    break;
                case "Mark":
                    yield return KitMotion.Sweep(Home("Pen"));
                    yield return KitMotion.Pop(Home("TimeTag"));
                    break;
                case "Bandage":
                    yield return KitMotion.Grow(Home("Sleeve"));
                    break;
            }
        }

        public void Clear()
        {
            if (root != null)
            {
                Object.Destroy(root);
            }

            root = null;
            Focus = null;
            homes.Clear();
        }

        private PartHome Home(string partName)
        {
            return homes.TryGetValue(partName, out PartHome home) ? home : default;
        }
    }
}
