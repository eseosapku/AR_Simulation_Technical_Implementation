using TMPro;
using UnityEditor;
using UnityEngine;

namespace FirstResponse.Editor
{
    public static class SafetyNoticeBuilder
    {
        [MenuItem("First Response/Build Safety Notice")]
        public static void Build()
        {
            var root = UIKit.NewScreen<SafetyNoticeScreen>("SafetyNoticeScreen");
            UIWidgets.Backdrop(root);

            var card = UIWidgets.Glass("Card", root, 76f);
            card.anchorMin = new Vector2(0f, 0.5f);
            card.anchorMax = new Vector2(1f, 0.5f);
            card.pivot = new Vector2(0.5f, 0.5f);
            card.offsetMin = new Vector2(70f, -560f);
            card.offsetMax = new Vector2(-70f, 560f);

            var chip = UIWidgets.Chip("Chip", card, "ShieldExclamation", UIKit.Amber, UIKit.Hex("#FCF0D9"), 200f);
            UIKit.Place(chip, new Vector2(0.5f, 1f), new Vector2(0f, -90f), chip.sizeDelta);

            Line(card, "Title", "Before you begin", 68f, UIKit.Ink, true, -340f, 100f);
            Line(card, "Warning", "Never tighten a tourniquet on a real person. Practise only on your rolled-towel limb.", 46f, UIKit.Muted, false, -460f, 220f);
            Line(card, "Disclaimer", "Training aid only. Not a certification.", 38f, UIKit.Muted, false, -700f, 70f);

            var understand = UIWidgets.Button("UnderstandButton", card, "I understand", null, true);
            var rect = (RectTransform)understand.transform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(70f, 80f);
            rect.offsetMax = new Vector2(-70f, 250f);

            UIKit.Assign(root.GetComponent<SafetyNoticeScreen>(), "understandButton", understand);
        }

        static void Line(Transform card, string name, string text, float size, Color color, bool bold, float top, float height)
        {
            var label = UIKit.Text(name, card, text, size, color, bold);
            label.alignment = TextAlignmentOptions.Top;
            var rect = label.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(80f, top - height);
            rect.offsetMax = new Vector2(-80f, top);
        }
    }
}
