using UnityEngine;

namespace HoldfastAR.UI
{
    public abstract class UIPanel
    {
        protected RectTransform Root { get; private set; }
        protected RectTransform Backdrop { get; private set; }
        protected GameManager Game { get; private set; }
        protected UIManager Manager { get; private set; }

        public bool IsVisible => Root != null && Root.gameObject.activeSelf;

        public void Build(RectTransform parent, RectTransform backdropParent, UIManager manager, GameManager game)
        {
            Manager = manager;
            Game = game;
            Backdrop = UIFactory.Rect(GetType().Name + " Backdrop", backdropParent).Stretch();
            Root = UIFactory.Rect(GetType().Name, parent).Stretch();
            OnBuild();
            SetActive(false);
        }

        public void Show()
        {
            SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            if (!IsVisible) return;
            OnHide();
            SetActive(false);
        }

        private void SetActive(bool active)
        {
            Backdrop.gameObject.SetActive(active);
            Root.gameObject.SetActive(active);
        }

        public virtual void Tick(float deltaTime) { }

        protected abstract void OnBuild();
        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
    }
}
