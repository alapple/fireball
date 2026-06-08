using UnityEngine;

namespace Fireball.Weapons
{
    public class MoonshineMolotov : BottleWeapon
    {
        [Header("Molotov Settings")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private float throwForce = 15f;

        protected override void Fire()
        {
            if (weaponData.projectilePrefab == null) return;

            currentAmmo -= 20f; // High cost per throw

            GameObject projectile = Instantiate(weaponData.projectilePrefab, firePoint.position + firePoint.forward * 0.5f, firePoint.rotation);
            Collider projectileCollider = projectile.GetComponent<Collider>();
            if (projectileCollider != null)
            {
                Collider[] playerColliders = transform.root.GetComponentsInChildren<Collider>();
                foreach (Collider playerCollider in playerColliders)
                {
                    Physics.IgnoreCollision(projectileCollider, playerCollider);
                }
            }
            if (projectile.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(firePoint.forward * throwForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }
}
