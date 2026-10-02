using HoldfastAR.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public enum UISkin
    {
        None,
        Panel,
        Button,
        Bar,
        Crosshair,
        Arrow,
        Marker
    }

    public static class UIFactory
    {
        private const float SliceScale = 0.55f;

        private static Font _font;
        private static Sprite _panel;
        private static Sprite _button;
        private static Sprite _bar;
        private static Sprite _crosshair;
        private static Sprite _arrow;
        private static Sprite _marker;

        public static Font Font
        {
            get
            {
                if (_font != null) return _font;
                _font = Resources.Load<Font>("Fonts/KenneyFuture");
                if (_font == null) _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                return _font;
            }
        }

        public static Sprite SkinSprite(UISkin skin)
        {
            switch (skin)
            {
                case UISkin.Panel: return _panel != null ? _panel : (_panel = Resources.Load<Sprite>("UI/Panel"));
                case UISkin.Button: return _button != null ? _button : (_button = Resources.Load<Sprite>("UI/Button"));
                case UISkin.Bar: return _bar != null ? _bar : (_bar = Resources.Load<Sprite>("UI/Bar"));
                case UISkin.Crosshair: return _crosshair != null ? _crosshair : (_crosshair = Resources.Load<Sprite>("UI/Crosshair"));
                case UISkin.Arrow: return _arrow != null ? _arrow : (_arrow = Resources.Load<Sprite>("UI/Arrow"));
                case UISkin.Marker: return _marker != null ? _marker : (_marker = Resources.Load<Sprite>("UI/Marker"));
                default: return null;
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

        public static RectTransform Place(this RectTransform rt, Vector2 anchor, Vector2 size, Vector2 offset = default)
        {
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.sizeDelta = size;
            rt.anchoredPosition = offset;
            return rt;
        }

        public static Image Image(Transform parent, string name, Color color, UISkin skin = UISkin.None)
        {
            RectTransform rt = Rect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            Sprite sprite = SkinSprite(skin);
            if (sprite != null)
            {
                img.sprite = sprite;
                bool sliced = sprite.border.sqrMagnitude > 0f;
                img.type = sliced ? UnityEngine.UI.Image.Type.Sliced : UnityEngine.UI.Image.Type.Simple;
                img.pixelsPerUnitMultiplier = SliceScale;
                if (!sliced) img.preserveAspect = true;
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
            text.fontStyle = style == FontStyle.Bold ? FontStyle.Normal : style;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        public static Button Button(Transform parent, string label, Color color, UnityAction onClick, int fontSize = UITheme.BodySize)
        {
            Image bg = Image(parent, label + " Button", color, UISkin.Button);
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
            text.rectTransform.offsetMin = new Vector2(12, 8);
            text.rectTransform.offsetMax = new Vector2(-12, 0);
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

        private static Sprite _shade;

        public static Image Shade(Transform parent, string name, Color top, Color middle, Color bottom)
        {
            Image img = Image(parent, name, Color.white);
            img.sprite = ShadeSprite(top, middle, bottom);
            img.rectTransform.Stretch();
            return img;
        }

        private static Sprite ShadeSprite(Color top, Color middle, Color bottom)
        {
            if (_shade != null) return _shade;
            var tex = new Texture2D(1, 64, TextureFormat.RGBA32, false) { name = "Shade", wrapMode = TextureWrapMode.Clamp };
            for (int y = 0; y < 64; y++)
            {
                float t = y / 63f;
                tex.SetPixel(0, y, t < 0.5f ? Color.Lerp(bottom, middle, t * 2f) : Color.Lerp(middle, top, (t - 0.5f) * 2f));
            }
            tex.Apply();
            _shade = Sprite.Create(tex, new Rect(0, 0, 1, 64), new Vector2(0.5f, 0.5f));
            return _shade;
        }

        public static Text Glow(this Text text, Color color, float size = 3f)
        {
            var outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(size, -size);
            return text;
        }

        public static Text SingleLine(this Text text, int minSize)
        {
            text.resizeTextForBestFit = true;
            text.resizeTextMaxSize = text.fontSize;
            text.resizeTextMinSize = Mathf.Min(minSize, text.fontSize);
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

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
