using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class CompletionScreen : UIScreen
    {
        [SerializeField] private TextMeshProUGUI outcomeLabel;
        [SerializeField] private Image outcomeBadge;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private Image scoreRing;
        [SerializeField] private TextMeshProUGUI headlineLabel;
        [SerializeField] private TextMeshProUGUI timeLabel;
        [SerializeField] private TextMeshProUGUI bloodLabel;
        [SerializeField] private TextMeshProUGUI errorsLabel;
        [SerializeField] private TextMeshProUGUI tipLabel;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Color passColor;
        [SerializeField] private Color failColor;

        private SessionRecord record;
        private string tip;

        public override ScreenId Id => ScreenId.Completion;

        private void Awake()
        {
            retryButton.onClick.AddListener(() => Go(ScreenId.Simulation));
            menuButton.onClick.AddListener(() => Go(ScreenId.Start));
        }

        private void OnEnable()
        {
            EventBus<SessionFinished>.Subscribe(OnFinished);
        }

        private void OnDisable()
        {
            EventBus<SessionFinished>.Unsubscribe(OnFinished);
        }

        private void OnFinished(SessionFinished message)
        {
            record = message.Record;
            tip = message.Tip;
        }

        protected override void OnShown()
        {
            if (record == null)
            {
                return;
            }

            Color tint = record.Passed ? passColor : failColor;
            outcomeLabel.text = record.outcome;
            outcomeLabel.color = tint;
            outcomeBadge.color = new Color(tint.r, tint.g, tint.b, 0.15f);
            scoreLabel.text = Mathf.RoundToInt(record.scorePercent).ToString();
            scoreRing.fillAmount = record.scorePercent / 100f;
            scoreRing.color = tint;
            headlineLabel.text = Headline(record);
            timeLabel.text = TimeText(record.timeToControlSec);
            bloodLabel.text = Mathf.RoundToInt(record.bloodLostMl) + " ml";
            errorsLabel.text = ErrorCount(record).ToString();
            tipLabel.text = tip;
        }

        public static string Headline(SessionRecord session)
        {
            if (session.timeToControlSec > 0f)
            {
                return session.Passed ? "Bleeding controlled" : "Controlled, with mistakes";
            }

            return "The casualty was lost";
        }

        public static string TimeText(float seconds)
        {
            return seconds > 0f ? Mathf.RoundToInt(seconds) + " s" : "Not controlled";
        }

        public static int ErrorCount(SessionRecord session)
        {
            int errors = session.criticalErrors.Count;
            foreach (StepResult step in session.steps)
            {
                foreach (string entry in step.events)
                {
                    if (entry.StartsWith("wrong_") || entry == "early_release")
                    {
                        errors++;
                    }
                }
            }

            return errors;
        }
    }
}
