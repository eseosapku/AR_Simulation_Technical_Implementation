using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class HistoryScreen : UIScreen
    {
        public const int Shown = 5;

        [SerializeField] Button backButton;
        [SerializeField] TextMeshProUGUI emptyLabel;
        [SerializeField] GameObject[] rows;
        [SerializeField] TextMeshProUGUI[] titles;
        [SerializeField] TextMeshProUGUI[] dates;
        [SerializeField] TextMeshProUGUI[] scores;
        [SerializeField] Image[] badges;
        [SerializeField] Image[] marks;
        [SerializeField] Sprite passSprite;
        [SerializeField] Sprite failSprite;
        [SerializeField] Color passColor;
        [SerializeField] Color failColor;

        public override ScreenId Id => ScreenId.History;

        void Awake()
        {
            backButton.onClick.AddListener(() => Go(ScreenId.Start));
        }

        protected override void OnShown()
        {
            var sessions = SessionStorage.Device.LoadRecent(Shown);
            emptyLabel.gameObject.SetActive(sessions.Count == 0);
            for (var i = 0; i < rows.Length; i++)
            {
                rows[i].SetActive(i < sessions.Count);
                if (i < sessions.Count)
                {
                    Fill(i, sessions[i]);
                }
            }
        }

        void Fill(int index, SessionRecord session)
        {
            titles[index].text = ScenarioName(session.scenarioId);
            dates[index].text = WhenText(session.endedAt, DateTime.Now);
            scores[index].text = Mathf.RoundToInt(session.scorePercent).ToString();
            marks[index].sprite = session.Passed ? passSprite : failSprite;
            marks[index].color = session.Passed ? passColor : failColor;
            var soft = session.Passed ? passColor : failColor;
            soft.a = 0.15f;
            badges[index].color = soft;
        }

        public static string ScenarioName(string scenarioId)
        {
            return scenarioId == nameof(ScenarioId.Thigh) ? "Thigh wound" : "Forearm cut";
        }

        public static string WhenText(string endedAt, DateTime now)
        {
            if (!DateTime.TryParse(endedAt, null, DateTimeStyles.RoundtripKind, out var ended))
            {
                return "Unknown date";
            }

            var local = ended.ToLocalTime();
            if (local.Date == now.Date)
            {
                return "Today, " + local.ToString("HH:mm", CultureInfo.InvariantCulture);
            }

            if (local.Date == now.Date.AddDays(-1))
            {
                return "Yesterday";
            }

            return local.ToString("MMM d", CultureInfo.InvariantCulture);
        }
    }
}
