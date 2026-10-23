using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class StartScreen : UIScreen
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button howButton;
        [SerializeField] private Button historyButton;

        public override ScreenId Id => ScreenId.Start;

        private void Awake()
        {
            startButton.onClick.AddListener(() => Go(ScreenId.ScenarioSelect));
            howButton.onClick.AddListener(() => Go(ScreenId.HowItWorks));
            historyButton.onClick.AddListener(() => Go(ScreenId.History));
        }
    }
}
