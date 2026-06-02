using UnityEngine;

namespace Fireball.Weapons
{
    public class MolotovProjectile : MonoBehaviour
    {
        [SerializeField] private GameObject fireZonePrefab;
        [SerializeField] private GameObject explosionEffect;

        private void OnCollisionEnter(Collision collision)
        {
            Explode();
        }

        private void Explode()
        {
            if (fireZonePrefab != null)
            {
                // Spawn fire zone slightly above impact point
                Instantiate(fireZonePrefab, transform.position + Vector3.up * 0.1f, Quaternion.identity);
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
