using UnityEngine;

namespace Fireball.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Weapon Data", menuName = "Fireball/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName;
        public float maxAmmo = 100f;
        public float fermentationRate = 10f; // Ammo per second when not firing
        public float fireRate = 0.1f;
        public float damage = 5f;
        public float knockbackForce = 10f;
        public GameObject projectilePrefab; // For Molotov
    }
}
