using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoldfastAR.UI
{
    public class SpawnMarkers
    {
        private const int PoolSize = 8;
        private const float Lifetime = 2.5f;
        private const float EdgeMargin = 90f;
        private const float ArrowSpriteAngle = 135f;

        private class Marker
        {
            public Transform Target;
            public float TimeLeft;
            public Image Pin;
            public Image Arrow;
        }

        private readonly List<Marker> _markers = new List<Marker>();
        private RectTransform _root;

        public void Build(RectTransform parent)
        {
            _root = UIFactory.Rect("Spawn Markers", parent).Stretch();
            for (int i = 0; i < PoolSize; i++)
            {
                Image pin = UIFactory.Image(_root, "Pin", UITheme.Accent, UISkin.Marker);
                pin.rectTransform.Place(new Vector2(0.5f, 0.5f), new Vector2(70, 70));
                Image arrow = UIFactory.Image(_root, "Arrow", UITheme.Accent, UISkin.Arrow);
                arrow.rectTransform.Place(new Vector2(0.5f, 0.5f), new Vector2(64, 64));
                pin.gameObject.SetActive(false);
                arrow.gameObject.SetActive(false);
                _markers.Add(new Marker { Pin = pin, Arrow = arrow });
            }
        }

        public void Track(Transform target)
        {
            Marker free = null;
            foreach (Marker m in _markers)
                if (m.Target == null || m.TimeLeft <= 0f) { free = m; break; }
            if (free == null) free = _markers[0];
            free.Target = target;
            free.TimeLeft = Lifetime;
        }

        public void Clear()
        {
            foreach (Marker m in _markers) Release(m);
        }

        public void Tick(float deltaTime, Camera cam)
        {
            if (cam == null) return;
            Rect area = _root.rect;
            foreach (Marker m in _markers)
            {
                if (m.Target == null || m.TimeLeft <= 0f)
                {
                    Release(m);
                    continue;
                }

                m.TimeLeft -= deltaTime;
                float alpha = Mathf.Clamp01(m.TimeLeft / 0.5f);
                Vector3 screen = cam.WorldToScreenPoint(m.Target.position + Vector3.up * 0.25f);
                bool inFront = screen.z > 0f;
                Vector2 local;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_root, new Vector2(screen.x, screen.y), null, out local);
                local -= area.center;

                float halfW = area.width * 0.5f - EdgeMargin;
                float halfH = area.height * 0.5f - EdgeMargin;
                bool onScreen = inFront && Mathf.Abs(local.x) < halfW && Mathf.Abs(local.y) < halfH;

                if (onScreen)
                {
                    m.Arrow.gameObject.SetActive(false);
                    m.Pin.gameObject.SetActive(true);
                    m.Pin.rectTransform.anchoredPosition = local;
                    m.Pin.rectTransform.localScale = Vector3.one * (1f + 0.15f * Mathf.Sin(Time.time * 10f));
                    m.Pin.color = WithAlpha(UITheme.Accent, alpha);
                    continue;
                }

                Vector2 dir = inFront ? local : -local;
                if (dir.sqrMagnitude < 0.0001f) dir = Vector2.down;
                dir.Normalize();
                float reach = Mathf.Min(halfW / Mathf.Max(Mathf.Abs(dir.x), 0.0001f), halfH / Mathf.Max(Mathf.Abs(dir.y), 0.0001f));

                m.Pin.gameObject.SetActive(false);
                m.Arrow.gameObject.SetActive(true);
                m.Arrow.rectTransform.anchoredPosition = dir * reach;
                m.Arrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - ArrowSpriteAngle);
                m.Arrow.color = WithAlpha(UITheme.Accent, alpha);
            }
        }

        private static void Release(Marker m)
        {
            m.Target = null;
            m.TimeLeft = 0f;
            m.Pin.gameObject.SetActive(false);
            m.Arrow.gameObject.SetActive(false);
        }

        private static Color WithAlpha(Color c, float a) => new Color(c.r, c.g, c.b, a);
    }
}
