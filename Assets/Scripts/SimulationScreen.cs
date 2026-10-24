using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class SimulationScreen : UIScreen
    {
        [SerializeField] private SimulationManager simulation;
        [SerializeField] private TextMeshProUGUI stepLabel;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI instructionLabel;
        [SerializeField] private Image stepIcon;
        [SerializeField] private Sprite[] icons;
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private TextMeshProUGUI[] choiceLabels;
        [SerializeField] private Button tapButton;
        [SerializeField] private TextMeshProUGUI tapLabel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        public override ScreenId Id => ScreenId.Simulation;

        private void Awake()
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int option = i;
                choiceButtons[i].onClick.AddListener(() => simulation.Choose(option));
            }

            tapButton.onClick.AddListener(OnTap);
            pauseButton.onClick.AddListener(() => SetPaused(true));
            resumeButton.onClick.AddListener(() => SetPaused(false));
            restartButton.onClick.AddListener(Restart);
            menuButton.onClick.AddListener(Quit);
        }

        private void OnEnable()
        {
            EventBus<StepStarted>.Subscribe(OnStepStarted);
        }

        private void OnDisable()
        {
            EventBus<StepStarted>.Unsubscribe(OnStepStarted);
        }

        protected override void OnShown()
        {
            SetPaused(false);
        }

        private void OnStepStarted(StepStarted message)
        {
            ProcedureStep step = message.Step;
            stepLabel.text = "Step " + (message.Index + 1) + " / " + message.Count;
            titleLabel.text = step.Title;
            instructionLabel.text = step.Instruction;
            stepIcon.sprite = FindIcon(step.Icon);
            ChoiceStep choice = step as ChoiceStep;
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                bool used = choice != null && i < choice.Options.Length;
                choiceButtons[i].gameObject.SetActive(used);
                if (used)
                {
                    choiceLabels[i].text = choice.Options[i];
                }
            }

            tapButton.gameObject.SetActive(step is TapStep);
            RefreshTapLabel();
        }

        private void OnTap()
        {
            simulation.Tap();
            RefreshTapLabel();
        }

        private void RefreshTapLabel()
        {
            if (simulation.CurrentStep is TapStep tap)
            {
                tapLabel.text = tap.Remaining > 1 ? "Done (" + tap.Remaining + " left)" : "Done";
            }
        }

        private Sprite FindIcon(string iconName)
        {
            foreach (Sprite icon in icons)
            {
                if (icon != null && icon.name == iconName)
                {
                    return icon;
                }
            }

            return stepIcon.sprite;
        }

        private void SetPaused(bool paused)
        {
            pausePanel.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
        }

        private void Restart()
        {
            SetPaused(false);
            simulation.Begin();
        }

        private void Quit()
        {
            SetPaused(false);
            simulation.Stop();
            Go(ScreenId.Start);
        }
    }
}
