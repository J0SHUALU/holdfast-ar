using HoldfastAR.Audio;
using HoldfastAR.Combat;
using HoldfastAR.Core;
using UnityEngine;

namespace HoldfastAR.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private ProjectilePool pool;
        [SerializeField] private Transform muzzle;
        [SerializeField] private float fireRate = 6f;
        [SerializeField] private float projectileSpeed = 6f;
        [SerializeField] private float damage = 1f;
        [SerializeField] private float recoilDistance = 0.015f;

        private float _nextShot;
        private Vector3 _muzzleRestPosition;

        private bool _canFire;

        public bool TriggerHeld { get; set; }

        public bool CanFire
        {
            get => _canFire;
            set
            {
                _canFire = value;
                if (muzzle != null) muzzle.gameObject.SetActive(value);
            }
        }

        private void Awake()
        {
            if (muzzle != null) _muzzleRestPosition = muzzle.localPosition;
            CanFire = false;
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
            Vector3 origin = muzzle != null ? muzzle.position + muzzle.forward * 0.09f : cam.position + cam.forward * 0.15f;
            Vector3 aimPoint = cam.position + cam.forward * 8f;
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
