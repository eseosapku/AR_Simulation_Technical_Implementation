using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class HistoryScreen : UIScreen
    {
        public const int Shown = 5;

        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI emptyLabel;
        [SerializeField] private GameObject[] rows;
        [SerializeField] private TextMeshProUGUI[] titles;
        [SerializeField] private TextMeshProUGUI[] dates;
        [SerializeField] private TextMeshProUGUI[] scores;
        [SerializeField] private Image[] badges;
        [SerializeField] private Image[] marks;
        [SerializeField] private Sprite passSprite;
        [SerializeField] private Sprite failSprite;
        [SerializeField] private Color passColor;
        [SerializeField] private Color failColor;

        public override ScreenId Id => ScreenId.History;

        private void Awake()
        {
            backButton.onClick.AddListener(() => Go(ScreenId.Start));
        }

        protected override void OnShown()
        {
            IReadOnlyList<SessionRecord> sessions = SessionStorage.Device.LoadRecent(Shown);
            emptyLabel.gameObject.SetActive(sessions.Count == 0);
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i].SetActive(i < sessions.Count);
                if (i < sessions.Count)
                {
                    Fill(i, sessions[i]);
                }
            }
        }

        private void Fill(int index, SessionRecord session)
        {
            titles[index].text = ScenarioName(session.scenarioId);
            dates[index].text = WhenText(session.endedAt, DateTime.Now);
            scores[index].text = Mathf.RoundToInt(session.scorePercent).ToString();
            marks[index].sprite = session.Passed ? passSprite : failSprite;
            marks[index].color = session.Passed ? passColor : failColor;
            Color soft = session.Passed ? passColor : failColor;
            soft.a = 0.15f;
            badges[index].color = soft;
        }

        public static string ScenarioName(string scenarioId)
        {
            return scenarioId == nameof(ScenarioId.Thigh) ? "Thigh wound" : "Forearm cut";
        }

        public static string WhenText(string endedAt, DateTime now)
        {
            if (!DateTime.TryParse(endedAt, null, DateTimeStyles.RoundtripKind, out DateTime ended))
            {
                return "Unknown date";
            }

            DateTime local = ended.ToLocalTime();
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
