using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HoldfastAR.Core
{
    public static class InputHelper
    {
        public static bool TryGetWorldTap(out Vector2 screenPosition)
        {
            screenPosition = default;
#if ENABLE_INPUT_SYSTEM
            Pointer pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return false;
            screenPosition = pointer.position.ReadValue();
            return !IsOverUI(-1);
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase != TouchPhase.Began) return false;
                screenPosition = touch.position;
                return !IsOverUI(touch.fingerId);
            }
            if (!Input.GetMouseButtonDown(0)) return false;
            screenPosition = Input.mousePosition;
            return !IsOverUI(-1);
#endif
        }

        private static bool IsOverUI(int pointerId)
        {
            EventSystem es = EventSystem.current;
            if (es == null) return false;
            return pointerId >= 0 ? es.IsPointerOverGameObject(pointerId) : es.IsPointerOverGameObject();
        }
    }
}
