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

        public override void StartFire()
        {
            base.StartFire();
            if (currentAmmo > 0 && foamParticles != null)
            {
                foamParticles.Play();
            }
        }

        public override void StopFire()
        {
            base.StopFire();
            if (foamParticles != null)
            {
                foamParticles.Stop();
            }
        }

        protected override void Update()
        {
            base.Update();
            if (isFiring && currentAmmo <= 0 && foamParticles != null)
            {
                foamParticles.Stop();
            }
        }

        protected override void Fire()
        {
            currentAmmo -= 1f; // Consumes ammo per "shot" in the continuous stream

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, range, hitLayers);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player")) continue; // Don't damage player

                Debug.Log($"Weapon hit: {hit.collider.name}");

                if (hit.collider.TryGetComponent(out IDamageable damageable))
                {
                    Debug.Log($"Applying damage to: {hit.collider.name}");
                    damageable.TakeDamage(weaponData.damage * Time.deltaTime * 10f); // Scale damage
                }

                if (hit.collider.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce(transform.forward * weaponData.knockbackForce, ForceMode.Impulse);
                }
            }
        }
    }
}
