using UnityEngine;
using UnityEngine.UI;

namespace FirstResponse
{
    public class SafetyNoticeScreen : UIScreen
    {
        [SerializeField] private Button understandButton;

        public override ScreenId Id => ScreenId.SafetyNotice;

        private void Awake()
        {
            understandButton.onClick.AddListener(Acknowledge);
        }

        public void Acknowledge()
        {
            Go(ScreenId.Setup);
        }
    }
}
