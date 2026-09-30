using System;
using System.Collections.Generic;
using HoldfastAR.Audio;
using HoldfastAR.Core;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace HoldfastAR.AR
{
    /// <summary>
    /// Handles horizontal plane detection and tap-to-place.
    /// Only ONE arena can ever be placed: once placed, later taps are ignored,
    /// plane detection is switched off and the name trackers are hidden.
    /// The arena is parented to an ARAnchor so it stays locked to the real floor.
    /// </summary>
    public class ARPlacementController : MonoBehaviour
    {
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private ARAnchorManager anchorManager;
        [SerializeField] private GameObject arenaPrefab;
        [SerializeField] private bool stopPlaneDetectionAfterPlacement = true;

        private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

        private bool _listening;
        private Camera _camera;

        public event Action<Transform> ArenaPlaced;

        public Transform Arena { get; private set; }
        public bool IsPlaced => Arena != null;
        public PlaneSpawnArea SpawnArea { get; } = new PlaneSpawnArea();

        public int TrackedPlaneCount
        {
            get
            {
                if (planeManager == null) return 0;
                int count = 0;
                foreach (ARPlane plane in planeManager.trackables)
                    if (plane.trackingState == TrackingState.Tracking && plane.subsumedBy == null) count++;
                return count;
            }
        }

        private void Awake()
        {
            if (raycastManager == null) raycastManager = FindAnyObjectByType<ARRaycastManager>();
            if (planeManager == null) planeManager = FindAnyObjectByType<ARPlaneManager>();
            if (anchorManager == null) anchorManager = FindAnyObjectByType<ARAnchorManager>();
            _camera = Camera.main;

            if (planeManager != null) planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
        }

        /// <summary>Start listening for a placement tap (no-op if already placed).</summary>
        public void BeginPlacement()
        {
            if (IsPlaced) return;
            NamedPlaneVisualizer.VisibilityEnabled = true;
            if (planeManager != null)
            {
                planeManager.enabled = true;
                planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
            }
            _listening = true;
        }

        public void CancelPlacement() => _listening = false;

        // LateUpdate so the EventSystem has already processed this frame's touch,
        // which makes the "is this tap on a UI button?" check reliable.
        private void LateUpdate()
        {
            if (!_listening || IsPlaced || raycastManager == null) return;
            if (!InputHelper.TryGetWorldTap(out Vector2 screenPoint)) return;

            if (raycastManager.Raycast(screenPoint, Hits, TrackableType.PlaneWithinPolygon))
            {
                // Hits are sorted by distance; take the nearest upward-facing plane.
                foreach (ARRaycastHit hit in Hits)
                {
                    ARPlane plane = planeManager != null ? planeManager.GetPlane(hit.trackableId) : null;
                    if (plane != null && plane.alignment != PlaneAlignment.HorizontalUp) continue;
                    Place(hit.pose, plane);
                    break;
                }
            }
        }

        private void Place(Pose hitPose, ARPlane plane)
        {
            _listening = false;

            // Face the arena toward the player (yaw only).
            Vector3 toCamera = _camera != null ? _camera.transform.position - hitPose.position : Vector3.forward;
            toCamera.y = 0f;
            Quaternion rotation = toCamera.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(toCamera) : hitPose.rotation;
            var pose = new Pose(hitPose.position, rotation);

            ARAnchor anchor = null;
            if (plane != null && anchorManager != null) anchor = anchorManager.AttachAnchor(plane, pose);
            if (anchor == null)
            {
                // Fallback: a free-standing anchor at the hit pose.
                var anchorGo = new GameObject("Arena Anchor");
                anchorGo.transform.SetPositionAndRotation(pose.position, pose.rotation);
                anchor = anchorGo.AddComponent<ARAnchor>();
            }

            GameObject arena = arenaPrefab != null ? Instantiate(arenaPrefab, anchor.transform) : new GameObject("Arena");
            arena.transform.SetParent(anchor.transform, false);
            arena.transform.localPosition = Vector3.zero;
            arena.transform.localRotation = Quaternion.identity;
            Arena = arena.transform;

            if (planeManager != null)
            {
                var planes = new List<ARPlane>();
                foreach (ARPlane p in planeManager.trackables) planes.Add(p);
                SpawnArea.Capture(planes, Arena);
            }

            if (stopPlaneDetectionAfterPlacement && planeManager != null)
            {
                planeManager.requestedDetectionMode = PlaneDetectionMode.None;
                planeManager.enabled = false;
            }
            NamedPlaneVisualizer.VisibilityEnabled = false;

            if (AudioManager.Instance != null) AudioManager.Instance.PlayAt(SoundId.Place, pose.position, 0f);
            ArenaPlaced?.Invoke(Arena);
        }
    }
}
