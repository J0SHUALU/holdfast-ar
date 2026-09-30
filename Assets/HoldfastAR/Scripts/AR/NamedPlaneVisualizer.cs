using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace HoldfastAR.AR
{
    /// <summary>
    /// Custom replacement for Unity's default ARPlaneMeshVisualizer.
    /// Builds its own textured mesh from the plane boundary and tiles the
    /// "JOSHUA CHUKWUEBUKA MOSES" tracker texture across it in real-world metres.
    /// The mesh only renders while its plane is actually being tracked, so the
    /// tracker appears only when a plane has been detected.
    /// </summary>
    [RequireComponent(typeof(ARPlane), typeof(MeshFilter), typeof(MeshRenderer))]
    public class NamedPlaneVisualizer : MonoBehaviour
    {
        [Tooltip("Real-world size (metres) of one texture tile, i.e. one copy of the name.")]
        [SerializeField] private float tileSize = 0.6f;
        [SerializeField] private float heightOffset = 0.002f;

        /// <summary>Global switch used to hide every tracker after the arena is placed.</summary>
        public static bool VisibilityEnabled { get; set; } = true;

        private ARPlane _plane;
        private MeshRenderer _renderer;
        private LineRenderer _outline;
        private Mesh _mesh;

        private void Awake()
        {
            _plane = GetComponent<ARPlane>();
            _renderer = GetComponent<MeshRenderer>();
            _outline = GetComponent<LineRenderer>();
            _mesh = new Mesh { name = "Named Plane Mesh" };
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            _renderer.enabled = false;
            if (_outline != null) _outline.enabled = false;
        }

        private void OnEnable() => _plane.boundaryChanged += OnBoundaryChanged;
        private void OnDisable() => _plane.boundaryChanged -= OnBoundaryChanged;

        private void OnDestroy()
        {
            if (_mesh != null) Destroy(_mesh);
        }

        private void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs args) => Rebuild(_plane.boundary);

        private void Update()
        {
            bool visible = VisibilityEnabled
                           && _plane.trackingState == TrackingState.Tracking
                           && _plane.subsumedBy == null
                           && _mesh.vertexCount > 0;

            if (_renderer.enabled != visible) _renderer.enabled = visible;
            if (_outline != null && _outline.enabled != visible) _outline.enabled = visible;
        }

        private void Rebuild(NativeArray<Vector2> boundary)
        {
            int n = boundary.Length;
            if (n < 3)
            {
                _mesh.Clear();
                return;
            }

            // Unity treats clockwise triangles (seen from above) as front-facing.
            // ARCore boundaries are convex, so a triangle fan from the centroid is enough.
            bool counterClockwise = SignedArea(boundary) > 0f;

            var vertices = new Vector3[n + 1];
            var uvs = new Vector2[n + 1];
            var triangles = new int[n * 3];

            Vector2 centroid = Vector2.zero;
            for (int i = 0; i < n; i++) centroid += boundary[i];
            centroid /= n;

            vertices[0] = new Vector3(centroid.x, heightOffset, centroid.y);
            uvs[0] = ToUv(centroid);
            for (int i = 0; i < n; i++)
            {
                Vector2 p = boundary[counterClockwise ? n - 1 - i : i];
                vertices[i + 1] = new Vector3(p.x, heightOffset, p.y);
                uvs[i + 1] = ToUv(p);
            }

            for (int i = 0; i < n; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % n + 1;
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.uv = uvs;
            _mesh.triangles = triangles;
            var colors = new Color32[n + 1];
            for (int i = 0; i < colors.Length; i++) colors[i] = new Color32(255, 255, 255, 255);
            _mesh.colors32 = colors;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            if (_outline != null)
            {
                _outline.useWorldSpace = false;
                _outline.loop = true;
                _outline.positionCount = n;
                for (int i = 0; i < n; i++) _outline.SetPosition(i, vertices[i + 1]);
            }
        }

        // Plane-space metres -> texture space; the name is centred on the plane origin.
        private Vector2 ToUv(Vector2 planePoint) => planePoint / tileSize + new Vector2(0.5f, 0.5f);

        private static float SignedArea(NativeArray<Vector2> points)
        {
            float area = 0f;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 a = points[i];
                Vector2 b = points[(i + 1) % points.Length];
                area += a.x * b.y - b.x * a.y;
            }
            return area * 0.5f;
        }
    }
}
