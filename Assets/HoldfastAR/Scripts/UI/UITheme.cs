using UnityEngine;

namespace HoldfastAR.UI
{
    /// <summary>Shared colours and sizes so every panel looks consistent.</summary>
    public static class UITheme
    {
        public static readonly Color Accent = new Color(0.16f, 0.9f, 1f);
        public static readonly Color AccentDark = new Color(0.05f, 0.35f, 0.45f, 0.95f);
        public static readonly Color Panel = new Color(0.02f, 0.07f, 0.1f, 0.88f);
        public static readonly Color Card = new Color(0.02f, 0.06f, 0.09f, 0.97f);   // full-screen menus: near-opaque so bright planes don't show through
        public static readonly Color Dim = new Color(0f, 0f, 0f, 0.6f);
        public static readonly Color Text = new Color(0.95f, 0.98f, 1f);
        public static readonly Color TextMuted = new Color(0.65f, 0.78f, 0.82f);
        public static readonly Color Danger = new Color(1f, 0.25f, 0.2f);
        public static readonly Color Success = new Color(0.35f, 1f, 0.55f);
        public static readonly Color Gold = new Color(1f, 0.82f, 0.25f);
        public static readonly Color ButtonNeutral = new Color(0.12f, 0.2f, 0.25f, 0.95f);

        public const int TitleSize = 110;
        public const int HeadingSize = 64;
        public const int BodySize = 42;
        public const int SmallSize = 32;
    }
}
