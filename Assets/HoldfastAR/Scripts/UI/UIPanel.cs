using UnityEngine;

namespace HoldfastAR.UI
{
    /// <summary>
    /// Abstract base for every screen. UIManager treats all panels the same way
    /// (Build/Show/Hide/Tick) while each subclass builds and updates its own widgets.
    /// </summary>
    public abstract class UIPanel
    {
        protected RectTransform Root { get; private set; }
        protected GameManager Game { get; private set; }
        protected UIManager Manager { get; private set; }

        public bool IsVisible => Root != null && Root.gameObject.activeSelf;

        public void Build(RectTransform parent, UIManager manager, GameManager game)
        {
            Manager = manager;
            Game = game;
            Root = UIFactory.Rect(GetType().Name, parent).Stretch();
            OnBuild();
            Root.gameObject.SetActive(false);
        }

        public void Show()
        {
            Root.gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            if (!IsVisible) return;
            OnHide();
            Root.gameObject.SetActive(false);
        }

        public virtual void Tick(float deltaTime) { }

        protected abstract void OnBuild();
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}
