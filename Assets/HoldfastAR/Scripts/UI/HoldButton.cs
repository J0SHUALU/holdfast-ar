using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoldfastAR.UI
{
    public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event Action<bool> HeldChanged;

        public bool IsHeld { get; private set; }

        public void OnPointerDown(PointerEventData eventData) => SetHeld(true);
        public void OnPointerUp(PointerEventData eventData) => SetHeld(false);
        public void OnPointerExit(PointerEventData eventData) => SetHeld(false);

        private void OnDisable() => SetHeld(false);

        private void SetHeld(bool held)
        {
            if (IsHeld == held) return;
            IsHeld = held;
            transform.localScale = held ? Vector3.one * 0.92f : Vector3.one;
            HeldChanged?.Invoke(held);
        }
    }
}
