using HoldfastAR.Audio;
using HoldfastAR.Combat;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Player
{
    /// <summary>
    /// First-person blaster. Fires pooled projectiles from the camera toward the
    /// crosshair (screen centre) while the on-screen FIRE button is held.
    /// </summary>
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private ProjectilePool pool;
        [SerializeField] private Transform muzzle;
        [SerializeField] private float fireRate = 6f;          // shots per second
        [SerializeField] private float projectileSpeed = 6f;   // m/s (AR scale)
        [SerializeField] private float damage = 1f;            // 1 damage = 1 "bullet" of enemy health
        [SerializeField] private float recoilDistance = 0.015f;

        private float _nextShot;
        private Vector3 _muzzleRestPosition;

        public bool TriggerHeld { get; set; }
        public bool CanFire { get; set; }

        private void Awake()
        {
            if (muzzle != null) _muzzleRestPosition = muzzle.localPosition;
        }

        private void Update()
        {
            if (muzzle != null)
                muzzle.localPosition = Vector3.Lerp(muzzle.localPosition, _muzzleRestPosition, 18f * Time.deltaTime);

            if (!CanFire || !TriggerHeld || Time.time < _nextShot) return;
            _nextShot = Time.time + 1f / fireRate;
            Fire();
        }

        private void Fire()
        {
            Transform cam = transform;
            Vector3 origin = muzzle != null ? muzzle.position : cam.position + cam.forward * 0.15f;
            Vector3 aimPoint = cam.position + cam.forward * 8f;   // converge on the crosshair
            pool.Fire(origin, aimPoint - origin, projectileSpeed, damage, Team.Player);

            AudioManager.Instance?.Play(SoundId.PlayerShoot, 0.06f);
            if (muzzle != null) muzzle.localPosition = _muzzleRestPosition - Vector3.forward * recoilDistance;
        }

        public void ResetWeapon()
        {
            TriggerHeld = false;
            _nextShot = 0f;
        }
    }
}
