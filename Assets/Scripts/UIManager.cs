using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private ScreenId firstScreen = ScreenId.Start;

        private readonly Dictionary<ScreenId, UIScreen> screens = new Dictionary<ScreenId, UIScreen>();

        public ScreenId Current { get; private set; }

        private void Awake()
        {
            foreach (UIScreen screen in GetComponentsInChildren<UIScreen>(true))
            {
                screens[screen.Id] = screen;
            }
        }

        private void OnEnable()
        {
            EventBus<ScreenRequested>.Subscribe(OnScreenRequested);
        }

        private void OnDisable()
        {
            EventBus<ScreenRequested>.Unsubscribe(OnScreenRequested);
        }

        private void Start()
        {
            ShowOnly(firstScreen);
        }

        private void OnScreenRequested(ScreenRequested message)
        {
            ShowOnly(message.Screen);
        }

        public void ShowOnly(ScreenId target)
        {
            if (!screens.ContainsKey(target))
            {
                Debug.LogWarning($"No screen built for {target}");
                return;
            }

            foreach (KeyValuePair<ScreenId, UIScreen> pair in screens)
            {
                if (pair.Key == target)
                {
                    pair.Value.Show();
                }
                else
                {
                    pair.Value.Hide();
                }
            }

            Current = target;
        }
    }
}
