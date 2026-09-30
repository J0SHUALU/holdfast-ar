using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace HoldfastAR.AR
{
    /// <summary>
    /// Remembers the shape of every detected horizontal plane (in arena-local space)
    /// at the moment the arena is placed, and hands out random spawn points that lie
    /// on those real-world surfaces. Storing the polygons relative to the anchored
    /// arena keeps spawn points correct even if the anchor is refined later.
    /// </summary>
    public class PlaneSpawnArea
    {
        private struct Polygon
        {
            public Vector2[] Points; // arena-local x/z
            public float Height;     // arena-local y
        }

        private readonly List<Polygon> _polygons = new List<Polygon>();
        private Transform _arena;

        public int PolygonCount => _polygons.Count;

        public void Capture(IEnumerable<ARPlane> planes, Transform arena)
        {
            _arena = arena;
            _polygons.Clear();
            foreach (ARPlane plane in planes)
            {
                if (plane.alignment != UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp) continue;
                var boundary = plane.boundary;
                if (boundary.Length < 3) continue;

                var points = new Vector2[boundary.Length];
                float height = 0f;
                for (int i = 0; i < boundary.Length; i++)
                {
                    Vector3 world = plane.transform.TransformPoint(new Vector3(boundary[i].x, 0f, boundary[i].y));
                    Vector3 local = arena.InverseTransformPoint(world);
                    points[i] = new Vector2(local.x, local.z);
                    height += local.y;
                }
                _polygons.Add(new Polygon { Points = points, Height = height / boundary.Length });
            }
        }

        /// <summary>
        /// Random world position on a detected plane, inside a ring around the arena
        /// centre and at least <paramref name="minFromPlayer"/> metres from the player.
        /// Falls back to the arena's own plane height if no polygon is hit.
        /// </summary>
        public Vector3 GetSpawnPoint(float minRadius, float maxRadius, Vector3 playerPosition, float minFromPlayer)
        {
            Vector3 fallback = Vector3.zero;
            for (int attempt = 0; attempt < 25; attempt++)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                if (dir == Vector2.zero) dir = Vector2.right;
                Vector2 local = dir * Random.Range(minRadius, maxRadius);

                Vector3 candidate = _arena.TransformPoint(new Vector3(local.x, 0f, local.y));
                Vector3 flatToPlayer = playerPosition - candidate;
                flatToPlayer.y = 0f;
                if (flatToPlayer.magnitude < minFromPlayer) continue;
                if (attempt == 0 || fallback == Vector3.zero) fallback = candidate;

                foreach (Polygon poly in _polygons)
                {
                    if (!Contains(poly.Points, local)) continue;
                    return _arena.TransformPoint(new Vector3(local.x, poly.Height, local.y));
                }
            }
            return fallback != Vector3.zero ? fallback : _arena.TransformPoint(new Vector3(0f, 0f, maxRadius));
        }

        private static bool Contains(Vector2[] polygon, Vector2 p)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[j];
                if ((a.y > p.y) != (b.y > p.y) && p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y) + a.x)
                    inside = !inside;
            }
            return inside;
        }
    }
}
