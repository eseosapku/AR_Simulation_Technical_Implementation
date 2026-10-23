using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class HowItWorksScreen : UIScreen
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Button doneButton;

        public override ScreenId Id => ScreenId.HowItWorks;

        private void Awake()
        {
            backButton.onClick.AddListener(Close);
            doneButton.onClick.AddListener(Close);
        }

        public void Close()
        {
            Go(ScreenId.Start);
        }
    }
}
