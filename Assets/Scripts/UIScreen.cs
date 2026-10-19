using UnityEngine;

namespace FirstResponse
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIScreen : MonoBehaviour
    {
        CanvasGroup group;

        public abstract ScreenId Id { get; }

        public bool IsVisible => Group.alpha > 0f;

        CanvasGroup Group => group != null ? group : group = GetComponent<CanvasGroup>();

        public void Show()
        {
            SetVisible(true);
            OnShown();
        }

        public void Hide()
        {
            SetVisible(false);
            OnHidden();
        }

        protected virtual void OnShown()
        {
        }

        protected virtual void OnHidden()
        {
        }

        void SetVisible(bool visible)
        {
            Group.alpha = visible ? 1f : 0f;
            Group.interactable = visible;
            Group.blocksRaycasts = visible;
        }
    }
}
