using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class ScenarioSelectScreen : UIScreen
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button forearmCard;
        [SerializeField] private Button thighCard;

        public override ScreenId Id => ScreenId.ScenarioSelect;

        private void Awake()
        {
            backButton.onClick.AddListener(() => Go(ScreenId.Start));
            forearmCard.onClick.AddListener(() => Choose(ScenarioId.Forearm));
            thighCard.onClick.AddListener(() => Choose(ScenarioId.Thigh));
        }

        public void Choose(ScenarioId scenario)
        {
            EventBus<ScenarioChosen>.Publish(new ScenarioChosen(scenario));
            Go(ScreenId.SafetyNotice);
        }
    }
}
