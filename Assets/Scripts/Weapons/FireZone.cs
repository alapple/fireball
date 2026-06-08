using UnityEngine;
using Fireball.Core;

namespace Fireball.Weapons
{
    public class FireZone : MonoBehaviour
    {
        [SerializeField] private float damagePerSecond = 10f;
        [SerializeField] private float duration = 5f;
        [SerializeField] private float radius = 3f;

        private void Start()
        {
            Destroy(gameObject, duration);
        }

        private void Update()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (var col in colliders)
            {
                // Only damage if it's NOT the player
                if (!col.CompareTag("Player") && col.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
