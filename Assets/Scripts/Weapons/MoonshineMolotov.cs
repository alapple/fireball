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

            GameObject projectile = Instantiate(weaponData.projectilePrefab, firePoint.position, firePoint.rotation);
            if (projectile.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForce(firePoint.forward * throwForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }
}
