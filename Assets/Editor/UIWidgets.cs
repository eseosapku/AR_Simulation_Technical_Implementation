using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse.Editor
{
    public static class UIWidgets
    {
        public static RectTransform Glass(string name, Transform parent, float radius)
        {
            var root = UIKit.Rect(name, parent);
            var shadow = UIKit.Image("Shadow", root, UIKit.Shape("SoftShadow"), UIKit.Fade(Color.white, 0.55f), radius);
            UIKit.Stretch(shadow.rectTransform, -36f);
            shadow.rectTransform.anchoredPosition = new Vector2(0f, -18f);
            UIKit.Stretch(UIKit.Image("Fill", root, UIKit.Shape("RoundedPanel"), UIKit.Fade(Color.white, 0.6f), radius).rectTransform);
            UIKit.Stretch(UIKit.Image("Rim", root, UIKit.Shape("RoundedRim"), UIKit.Fade(Color.white, 0.95f), radius).rectTransform);
            return root;
        }

        public static Button Clickable(RectTransform root)
        {
            var target = root.Find("Fill").GetComponent<Image>();
            target.raycastTarget = true;
            var button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = target;
            var colors = button.colors;
            colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
            button.colors = colors;
            return button;
        }

        public static Button Button(string name, Transform parent, string label, string glyph, bool primary, float radius = 56f)
        {
            RectTransform root;
            if (primary)
            {
                root = UIKit.Rect(name, parent);
                UIKit.Stretch(UIKit.Image("Fill", root, UIKit.Shape("RoundedPanel"), UIKit.Red, radius).rectTransform);
            }
            else
            {
                root = Glass(name, parent, radius);
            }

            var button = Clickable(root);
            var tint = primary ? Color.white : UIKit.Ink;
            var content = UIKit.Stretch(UIKit.Rect("Content", root));
            var row = content.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.childAlignment = TextAnchor.MiddleCenter;
            row.spacing = 22f;
            row.childControlWidth = false;
            row.childControlHeight = false;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = false;

            if (!primary && glyph != null)
            {
                UIKit.Icon("Icon", content, glyph, tint, 56f);
            }

            var text = UIKit.Text("Label", content, label, 46f, tint, true);
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, 80f);

            if (primary && glyph != null)
            {
                UIKit.Icon("Icon", content, glyph, tint, 52f);
            }

            return button;
        }

        public static void Backdrop(Transform screen)
        {
            UIKit.Stretch(UIKit.Image("Backdrop", screen, null, UIKit.Paper).rectTransform);
        }

        public static RectTransform Chip(string name, Transform parent, string glyph, Color tint, Color soft, float size)
        {
            var chip = UIKit.Image(name, parent, UIKit.Shape("RoundedPanel"), soft, size * 0.32f).rectTransform;
            chip.sizeDelta = new Vector2(size, size);
            UIKit.Icon("Icon", chip, glyph, tint, size * 0.55f);
            return chip;
        }

        public static RectTransform Pill(string name, Transform parent, string label, Color text, Color soft)
        {
            var pill = UIKit.Image(name, parent, UIKit.Shape("RoundedPanel"), soft, 46f).rectTransform;
            var caption = UIKit.Text("Label", pill, label, 38f, text, true);
            caption.alignment = TextAlignmentOptions.Center;
            caption.textWrappingMode = TextWrappingModes.NoWrap;
            UIKit.Stretch(caption.rectTransform);
            pill.sizeDelta = new Vector2(caption.preferredWidth + 70f, 92f);
            return pill;
        }

        public static Button Back(Transform screen)
        {
            var back = Glass("BackButton", screen, 40f);
            UIKit.Place(back, new Vector2(0f, 1f), new Vector2(70f, -170f), new Vector2(124f, 124f));
            UIKit.Icon("Icon", back, "ChevronLeft", UIKit.Ink, 60f);
            return Clickable(back);
        }
    }
}
