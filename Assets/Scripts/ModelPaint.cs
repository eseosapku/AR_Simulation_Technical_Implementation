using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public static class ModelPaint
    {
        public static void Apply(GameObject root, Material template)
        {
            if (template == null)
            {
                return;
            }

            Dictionary<Material, Material> made = new Dictionary<Material, Material>();
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    Material source = materials[i];
                    if (source == null)
                    {
                        continue;
                    }

                    if (!made.TryGetValue(source, out Material copy))
                    {
                        copy = new Material(template) { name = source.name, color = BaseColor(source) };
                        copy.mainTexture = BaseTexture(source);
                        made.Add(source, copy);
                    }

                    materials[i] = copy;
                }

                renderer.sharedMaterials = materials;
            }
        }

        public static Color BaseColor(Material source)
        {
            if (source.HasProperty("baseColorFactor"))
            {
                return source.GetColor("baseColorFactor");
            }

            return source.HasProperty("_BaseColor") ? source.GetColor("_BaseColor") : source.color;
        }

        public static Texture BaseTexture(Material source)
        {
            if (source.HasProperty("baseColorTexture"))
            {
                return source.GetTexture("baseColorTexture");
            }

            return source.HasProperty("_BaseMap") ? source.GetTexture("_BaseMap") : null;
        }

        public static Transform FindEnding(Transform root, string suffix)
        {
            if (root.name.EndsWith(suffix))
            {
                return root;
            }

            foreach (Transform child in root)
            {
                Transform found = FindEnding(child, suffix);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
