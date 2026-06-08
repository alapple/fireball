using UnityEngine;
using System.Collections;

namespace Fireball.Weapons
{
    public class MolotovProjectile : MonoBehaviour
    {
        [SerializeField] private GameObject fireZonePrefab;
        [SerializeField] private GameObject explosionEffect;
        
        private Collider col;

        private void Awake()
        {
            col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
                StartCoroutine(EnableColliderAfterDelay(0.1f));
            }
        }

        private IEnumerator EnableColliderAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (col != null) col.enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Explode();
        }

        private void Explode()
        {
            if (fireZonePrefab != null)
            {
                // Spawn fire zone slightly above impact point
                Instantiate(fireZonePrefab, transform.position + Vector3.up * 0.1f, Quaternion.Euler(90f, 0f, 0f));
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
