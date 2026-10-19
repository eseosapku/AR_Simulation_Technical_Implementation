using System.Collections.Generic;
using UnityEngine;

namespace FirstResponse
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] ScreenId firstScreen = ScreenId.Start;

        readonly Dictionary<ScreenId, UIScreen> screens = new Dictionary<ScreenId, UIScreen>();

        public ScreenId Current { get; private set; }

        void Awake()
        {
            foreach (var screen in GetComponentsInChildren<UIScreen>(true))
            {
                screens[screen.Id] = screen;
            }
        }

        void OnEnable()
        {
            EventBus<ScreenRequested>.Subscribe(OnScreenRequested);
        }

        void OnDisable()
        {
            EventBus<ScreenRequested>.Unsubscribe(OnScreenRequested);
        }

        void Start()
        {
            ShowOnly(firstScreen);
        }

        void OnScreenRequested(ScreenRequested message)
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

            foreach (var pair in screens)
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
