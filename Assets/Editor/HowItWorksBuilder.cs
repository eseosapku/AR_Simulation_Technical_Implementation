using TMPro;
using UnityEditor;
using UnityEngine;

namespace FirstResponse.Editor
{
    public static class HowItWorksBuilder
    {
        static readonly string[,] Steps =
        {
            { "Print the markers", "Four squares, 6 cm each" },
            { "Build the practice limb", "Roll a towel, tape markers on" },
            { "Scan the floor", "Move your phone slowly" },
            { "Follow the prompts", "Your hands do the work" }
        };

        [MenuItem("First Response/Build How It Works")]
        public static void Build()
        {
            var root = UIKit.NewScreen<HowItWorksScreen>("HowItWorksScreen");
            UIWidgets.Backdrop(root);
            var back = UIWidgets.Back(root);

            var title = UIKit.Text("Title", root, "How it works", 76f, UIKit.Ink, true);
            UIKit.Place(title.rectTransform, new Vector2(0f, 1f), new Vector2(70f, -380f), new Vector2(940f, 110f));

            var card = UIWidgets.Glass("Steps", root, 76f);
            card.anchorMin = new Vector2(0f, 1f);
            card.anchorMax = new Vector2(1f, 1f);
            card.pivot = new Vector2(0.5f, 1f);
            card.offsetMin = new Vector2(70f, -560f - 4 * 230f - 40f);
            card.offsetMax = new Vector2(-70f, -560f);

            for (var i = 0; i < 4; i++)
            {
                Step(card, i, Steps[i, 0], Steps[i, 1]);
            }

            var done = UIWidgets.Button("DoneButton", root, "Got it", null, true);
            var rect = (RectTransform)done.transform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(70f, 160f);
            rect.offsetMax = new Vector2(-70f, 330f);

            var screen = root.GetComponent<HowItWorksScreen>();
            UIKit.Assign(screen, "backButton", back);
            UIKit.Assign(screen, "doneButton", done);
        }

        static void Step(Transform card, int index, string heading, string detail)
        {
            var top = -40f - index * 230f;
            var badge = UIKit.Image("Number" + (index + 1), card, UIKit.Shape("RoundedPanel"), UIKit.Hex("#FDE8E8"), 40f);
            UIKit.Place(badge.rectTransform, new Vector2(0f, 1f), new Vector2(56f, top - 30f), new Vector2(130f, 130f));
            var number = UIKit.Text("Label", badge.transform, (index + 1).ToString(), 56f, UIKit.Red, true);
            number.alignment = TextAlignmentOptions.Center;
            UIKit.Stretch(number.rectTransform);

            var title = UIKit.Text("Title" + (index + 1), card, heading, 52f, UIKit.Ink, true);
            UIKit.Place(title.rectTransform, new Vector2(0f, 1f), new Vector2(230f, top - 32f), new Vector2(640f, 72f));
            var sub = UIKit.Text("Detail" + (index + 1), card, detail, 42f, UIKit.Muted);
            UIKit.Place(sub.rectTransform, new Vector2(0f, 1f), new Vector2(230f, top - 104f), new Vector2(640f, 64f));
        }
    }
}
