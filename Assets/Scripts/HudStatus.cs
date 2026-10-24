using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class HudStatus : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulation;
        [SerializeField] private TextMeshProUGUI timerLabel;
        [SerializeField] private TextMeshProUGUI lossLabel;
        [SerializeField] private Image lossFill;
        [SerializeField] private GameObject progressGroup;
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI progressLabel;
        [SerializeField] private GameObject markerHint;
        [SerializeField] private Color safeColor;
        [SerializeField] private Color warningColor;
        [SerializeField] private Color dangerColor;

        private void Update()
        {
            BleedingModel bleeding = simulation.Bleeding;
            if (bleeding == null)
            {
                return;
            }

            timerLabel.text = Clock(simulation.Elapsed);
            lossLabel.text = Mathf.RoundToInt(bleeding.TotalLoss) + " ml";
            SetBar(lossFill, bleeding.LossFraction);
            lossFill.color = LossColor(bleeding.LossFraction);

            PhysicalStep physical = simulation.CurrentStep as PhysicalStep;
            progressGroup.SetActive(physical != null);
            markerHint.SetActive(physical != null && !physical.Latest.IsActive);
            if (physical != null)
            {
                SetBar(progressFill, physical.Latest.Progress);
                progressLabel.text = ProgressText(physical);
            }
        }

        private static void SetBar(Image fill, float fraction)
        {
            fill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(fraction), 1f);
        }

        private Color LossColor(float fraction)
        {
            if (fraction < 0.35f)
            {
                return safeColor;
            }

            return fraction < 0.7f ? warningColor : dangerColor;
        }

        public static string Clock(float seconds)
        {
            int whole = Mathf.FloorToInt(seconds);
            return (whole / 60).ToString("00") + ":" + (whole % 60).ToString("00");
        }

        private static string ProgressText(PhysicalStep step)
        {
            float measured = step.Latest.Measurement;
            if (step is PressureStep)
            {
                return "Held " + Mathf.FloorToInt(measured) + " s";
            }

            if (step is TourniquetStep)
            {
                return measured > 0f ? measured.ToString("0.0") + " cm from wound" : "Show markers A, B and D";
            }

            return Mathf.RoundToInt(measured) + " degrees turned";
        }
    }
}
