using UnityEngine;
using Fireball.Core;

namespace Fireball.Weapons
{
    public class ChampagneFlamethrower : BottleWeapon
    {
        [Header("Flamethrower Settings")]
        [SerializeField] private ParticleSystem foamParticles;
        [SerializeField] private float range = 5f;
        [SerializeField] private float radius = 1.5f;
        [SerializeField] private LayerMask hitLayers;

        protected override void Update()
        {
            // Call base.Update to ensure fermentation (ammo recharge) logic runs!
            base.Update();
            
            if (isFiring && currentAmmo > 0)
            {
                if (foamParticles != null && !foamParticles.isPlaying)
                {
                    foamParticles.Play();
                }
                ApplyFoamDamage();
            }
            else
            {
                if (foamParticles != null && foamParticles.isPlaying)
                {
                    foamParticles.Stop();
                }
            }
        }

        private void ApplyFoamDamage()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, range, hitLayers);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player")) continue;

                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(weaponData.damage * Time.deltaTime);
                }

                if (hit.collider.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce(transform.forward * weaponData.knockbackForce * Time.deltaTime, ForceMode.Acceleration);
                }
            }
        }

        protected override void Fire()
        {
            // Continuous ammo consumption based on fire rate
            float consumptionRate = weaponData.fireRate > 0 ? (1f / weaponData.fireRate) : 10f;
            currentAmmo -= consumptionRate * Time.deltaTime;
        }

        protected override void AttemptFire()
        {
            // If we are firing, just call Fire to consume ammo. 
            // The actual effect is in Update.
            if (isFiring && currentAmmo > 0)
            {
                Fire();
            }
        }
    }
}
