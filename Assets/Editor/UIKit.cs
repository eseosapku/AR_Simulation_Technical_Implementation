using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse.Editor
{
    public static class UIKit
    {
        public static readonly Color Ink = Hex("#0F172A");
        public static readonly Color Muted = Hex("#64748B");
        public static readonly Color Red = Hex("#E5484D");
        public static readonly Color Teal = Hex("#0E9F8A");
        public static readonly Color Amber = Hex("#E79A16");
        public static readonly Color Paper = Hex("#FAFBFD");

        public static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }

        public static Color Fade(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Sprite Load(string path)
        {
            var sprite = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
            if (sprite != null)
            {
                return sprite;
            }

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
        }

        public static Sprite Shape(string name) => Load($"Assets/Sprites/{name}.png");

        public static Sprite Glyph(string name) => Load($"Assets/Icons/{name}.png");

        public static RectTransform NewScreen<T>(string name) where T : UIScreen
        {
            var manager = Object.FindAnyObjectByType<UIManager>(FindObjectsInactive.Include);
            if (manager == null)
            {
                throw new System.InvalidOperationException("No UIManager in this scene. Open Assets/Scenes/MainScene first.");
            }

            var canvas = manager.transform;
            var old = canvas.Find(name);
            if (old != null)
            {
                Undo.DestroyObjectImmediate(old.gameObject);
            }

            var root = Stretch(Rect(name, canvas));
            root.gameObject.AddComponent<T>();
            Undo.RegisterCreatedObjectUndo(root.gameObject, "Build " + name);
            EditorSceneManager.MarkSceneDirty(root.gameObject.scene);
            Selection.activeGameObject = root.gameObject;
            return root;
        }

        public static void Assign(Object target, string field, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        public static RectTransform Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
            return rect;
        }

        public static RectTransform Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        public static Image Image(string name, Transform parent, Sprite sprite, Color color, float radius = 64f)
        {
            var image = Rect(name, parent).gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = sprite != null && sprite.border != Vector4.zero ? UnityEngine.UI.Image.Type.Sliced : UnityEngine.UI.Image.Type.Simple;
            image.pixelsPerUnitMultiplier = 64f / radius;
            image.raycastTarget = false;
            return image;
        }

        public static TextMeshProUGUI Text(string name, Transform parent, string text, float size, Color color, bool bold = false)
        {
            var label = Rect(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
            var font = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] { "Assets/Fonts" });
            if (font.Length > 0)
            {
                label.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(font[0]));
            }

            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            label.raycastTarget = false;
            return label;
        }

        public static Image Icon(string name, Transform parent, string glyph, Color color, float size)
        {
            var icon = Image(name, parent, Glyph(glyph), color);
            icon.preserveAspect = true;
            icon.rectTransform.sizeDelta = new Vector2(size, size);
            return icon;
        }
    }
}
