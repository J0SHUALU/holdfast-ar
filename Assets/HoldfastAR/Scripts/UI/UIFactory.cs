using HoldfastAR.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    /// <summary>
    /// Helper that builds uGUI widgets from code, so panels stay short and every
    /// button/text shares one look. Uses Unity's built-in font, no extra assets needed.
    /// </summary>
    public static class UIFactory
    {
        private static Font _font;
        private static Sprite _rounded;

        public static Font Font => _font != null ? _font : (_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        /// <summary>A 9-sliced rounded rectangle built at runtime.</summary>
        public static Sprite Rounded
        {
            get
            {
                if (_rounded != null) return _rounded;
                const int size = 64, radius = 22;
                var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "Rounded UI" };
                var pixels = new Color32[size * size];
                for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(0, Mathf.Max(radius - x, x - (size - 1 - radius)));
                    float dy = Mathf.Max(0, Mathf.Max(radius - y, y - (size - 1 - radius)));
                    float a = Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255));
                }
                tex.SetPixels32(pixels);
                tex.Apply();
                _rounded = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
                    SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
                return _rounded;
            }
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        public static RectTransform Stretch(this RectTransform rt, float padding = 0f)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(padding, padding);
            rt.offsetMax = new Vector2(-padding, -padding);
            return rt;
        }

        /// <summary>Anchors at a normalised point with a fixed pixel size.</summary>
        public static RectTransform Place(this RectTransform rt, Vector2 anchor, Vector2 size, Vector2 offset = default)
        {
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.sizeDelta = size;
            rt.anchoredPosition = offset;
            return rt;
        }

        public static Image Image(Transform parent, string name, Color color, bool rounded = false)
        {
            RectTransform rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            if (rounded)
            {
                img.sprite = Rounded;
                img.type = UnityEngine.UI.Image.Type.Sliced;
            }
            img.raycastTarget = false;
            return img;
        }

        public static Text Text(Transform parent, string name, string value, int size, Color color,
            TextAnchor alignment = TextAnchor.MiddleCenter, FontStyle style = FontStyle.Normal)
        {
            RectTransform rt = Rect(name, parent);
            var text = rt.gameObject.AddComponent<Text>();
            text.font = Font;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = style;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        public static Button Button(Transform parent, string label, Color color, UnityAction onClick, int fontSize = UITheme.BodySize)
        {
            Image bg = Image(parent, label + " Button", color, rounded: true);
            bg.raycastTarget = true;
            var button = bg.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            colors.fadeDuration = 0.05f;
            button.colors = colors;
            button.onClick.AddListener(() => AudioManager.Instance?.Play(SoundId.UIClick));
            if (onClick != null) button.onClick.AddListener(onClick);

            Text text = Text(bg.transform, "Label", label, fontSize, UITheme.Text, style: FontStyle.Bold);
            text.rectTransform.Stretch();
            return button;
        }

        public static VerticalLayoutGroup VerticalLayout(RectTransform rt, float spacing, int padding = 0)
        {
            var layout = rt.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return layout;
        }

        /// <summary>
        /// Keeps a heading on one line: the font shrinks (down to <paramref name="minSize"/>)
        /// until the text fits the width instead of wrapping onto a second line.
        /// </summary>
        public static Text SingleLine(this Text text, int minSize)
        {
            text.resizeTextForBestFit = true;
            text.resizeTextMaxSize = text.fontSize;
            text.resizeTextMinSize = Mathf.Min(minSize, text.fontSize);
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        /// <summary>Sets the preferred height of an element inside a layout group.</summary>
        public static T Height<T>(this T component, float height) where T : Component
        {
            LayoutElement le = component.GetComponent<LayoutElement>();
            if (le == null) le = component.gameObject.AddComponent<LayoutElement>();
            le.preferredHeight = height;
            le.minHeight = height;
            return component;
        }
    }
}
