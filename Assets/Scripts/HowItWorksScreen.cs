using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class HowItWorksScreen : UIScreen
    {
        [SerializeField] Button backButton;
        [SerializeField] Button doneButton;

        public override ScreenId Id => ScreenId.HowItWorks;

        void Awake()
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
