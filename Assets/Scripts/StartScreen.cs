using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class StartScreen : UIScreen
    {
        [SerializeField] Button startButton;
        [SerializeField] Button howButton;
        [SerializeField] Button historyButton;

        public override ScreenId Id => ScreenId.Start;

        void Awake()
        {
            startButton.onClick.AddListener(() => Go(ScreenId.ScenarioSelect));
            howButton.onClick.AddListener(() => Go(ScreenId.HowItWorks));
            historyButton.onClick.AddListener(() => Go(ScreenId.History));
        }
    }
}
