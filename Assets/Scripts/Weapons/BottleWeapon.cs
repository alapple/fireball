using UnityEngine;
using Fireball.ScriptableObjects;

namespace Fireball.Weapons
{
    public abstract class BottleWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected WeaponData weaponData;

        [Header("VFX")]
        [SerializeField] protected ParticleSystem fermentationEffect;
        [SerializeField] protected ParticleSystem emptySmokeEffect;
        
        protected float currentAmmo;
        protected bool isFiring;
        protected float lastFireTime;
        protected bool wasEmpty;

        public float CurrentAmmo => currentAmmo;
        public float MaxAmmo => weaponData != null ? weaponData.maxAmmo : 0;
        public bool IsEmpty => currentAmmo <= 0;

        protected virtual void Start()
        {
            if (weaponData != null)
            {
                currentAmmo = weaponData.maxAmmo;
            }

            if (fermentationEffect != null) fermentationEffect.Stop();
        }

        protected virtual void Update()
        {
            HandleFermentation();
            if (isFiring)
            {
                AttemptFire();
            }

            CheckEmptyState();
        }

        private void HandleFermentation()
        {
            if (!isFiring && currentAmmo < weaponData.maxAmmo)
            {
                currentAmmo = Mathf.Min(weaponData.maxAmmo, currentAmmo + weaponData.fermentationRate * Time.deltaTime);
                
                if (fermentationEffect != null && !fermentationEffect.isPlaying)
                    fermentationEffect.Play();
            }
            else
            {
                if (fermentationEffect != null && fermentationEffect.isPlaying)
                    fermentationEffect.Stop();
            }
        }

        private void CheckEmptyState()
        {
            if (IsEmpty && !wasEmpty)
            {
                if (emptySmokeEffect != null) emptySmokeEffect.Play();
                wasEmpty = true;
            }
            else if (!IsEmpty)
            {
                wasEmpty = false;
            }
        }

        public virtual void StartFire() => isFiring = true;
        public virtual void StopFire() => isFiring = false;

        protected virtual void AttemptFire()
        {
            if (currentAmmo > 0 && Time.time >= lastFireTime + weaponData.fireRate)
            {
                Fire();
                lastFireTime = Time.time;
            }
        }

        protected abstract void Fire();
    }
}
