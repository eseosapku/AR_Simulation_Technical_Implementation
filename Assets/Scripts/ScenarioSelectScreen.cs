using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class ScenarioSelectScreen : UIScreen
    {
        [SerializeField] Button backButton;
        [SerializeField] Button forearmCard;
        [SerializeField] Button thighCard;

        public override ScreenId Id => ScreenId.ScenarioSelect;

        void Awake()
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
