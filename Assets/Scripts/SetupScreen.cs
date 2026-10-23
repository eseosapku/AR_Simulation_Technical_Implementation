using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public enum SetupAction
    {
        None,
        Confirm,
        Begin
    }

    public class SetupScreen : UIScreen
    {
        [SerializeField] private TextMeshProUGUI statusLabel;
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI detailLabel;
        [SerializeField] private TextMeshProUGUI primaryLabel;
        [SerializeField] private Button primaryButton;
        [SerializeField] private Button moveButton;

        private PlacementState state = PlacementState.Scanning;

        public override ScreenId Id => ScreenId.Setup;

        private void Awake()
        {
            primaryButton.onClick.AddListener(PressPrimary);
            moveButton.onClick.AddListener(() => EventBus<PlacementRequested>.Publish(new PlacementRequested(PlacementAction.Move)));
        }

        private void OnEnable()
        {
            EventBus<PlacementChanged>.Subscribe(OnPlacement);
        }

        private void OnDisable()
        {
            EventBus<PlacementChanged>.Unsubscribe(OnPlacement);
        }

        protected override void OnShown()
        {
            Render();
        }

        private void OnPlacement(PlacementChanged message)
        {
            state = message.State;
            Render();
        }

        private void Render()
        {
            (string status, string title, string detail) copy = Describe(state);
            statusLabel.text = copy.status;
            titleLabel.text = copy.title;
            detailLabel.text = copy.detail;
            SetupAction action = PrimaryAction(state);
            primaryButton.gameObject.SetActive(action != SetupAction.None);
            primaryLabel.text = action == SetupAction.Begin ? "Begin" : "Confirm";
            moveButton.gameObject.SetActive(state == PlacementState.Confirmed);
        }

        private void PressPrimary()
        {
            SetupAction action = PrimaryAction(state);
            if (action == SetupAction.Confirm)
            {
                EventBus<PlacementRequested>.Publish(new PlacementRequested(PlacementAction.Confirm));
            }
            else if (action == SetupAction.Begin)
            {
                Go(ScreenId.Simulation);
            }
        }

        public static SetupAction PrimaryAction(PlacementState state)
        {
            switch (state)
            {
                case PlacementState.Previewing:
                    return SetupAction.Confirm;
                case PlacementState.Confirmed:
                    return SetupAction.Begin;
                default:
                    return SetupAction.None;
            }
        }

        public static (string status, string title, string detail) Describe(PlacementState state)
        {
            switch (state)
            {
                case PlacementState.Aiming:
                    return ("Floor found", "Tap the floor", "to place the scene");
                case PlacementState.Previewing:
                    return ("Floor found", "Happy with this spot?", "Tap elsewhere to move it");
                case PlacementState.Confirmed:
                    return ("Scene anchored", "Walk around it", "Then begin the scenario");
                default:
                    return ("Looking for the floor", "Move your phone slowly", "Point it at the ground");
            }
        }
    }
}
