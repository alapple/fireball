using UnityEngine;
using Fireball.Core;

namespace Fireball.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 25f; // Faster arrows feel better
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 5f;

        private Vector3 moveDirection;
        private bool directionSet = false;

        public void Initialize(Vector3 dir, float damageAmount)
        {
            moveDirection = dir.normalized;
            damage = Mathf.Max(damageAmount, 1f); // Ensure at least 1 damage
            directionSet = true;
            Debug.Log($"Projectile initialized with damage: {damage}");
        }

        public void SetDirection(Vector3 dir)
        {
            moveDirection = dir.normalized;
            directionSet = true;
            if (damage <= 0) damage = 10f; // Fallback
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
            if (!directionSet)
            {
                moveDirection = transform.forward;
            }
        }

        private void Update()
        {
            float step = speed * Time.deltaTime;
            Vector3 nextPos = transform.position + moveDirection * step;

            // Visualize the path in Scene view
            Debug.DrawLine(transform.position, nextPos, Color.red);

            // Use Raycast to detect collisions between current and next position (prevents tunneling)
            if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, step + 0.1f))
            {
                HandleCollision(hit.collider);
            }

            transform.position = nextPos;
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other);
        }

        private void HandleCollision(Collider other)
        {
            // Ignore other enemies
            if (other.CompareTag("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                Debug.Log($"Projectile hit Player: {other.name}");
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                    Debug.Log($"Applied {damage} damage to Player.");
                }
                else
                {
                    // Check parent in case the collider is on a child object
                    IDamageable parentDamageable = other.GetComponentInParent<IDamageable>();
                    if (parentDamageable != null)
                    {
                        parentDamageable.TakeDamage(damage);
                        Debug.Log($"Applied {damage} damage to Player (via parent).");
                    }
                    else
                    {
                        Debug.LogWarning($"Hit Player ({other.name}) but could not find IDamageable component! Current Tags: {other.tag}");
                    }
                }
                Destroy(gameObject);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Default") || other.CompareTag("Environment"))
            {
                Debug.Log($"Projectile hit environment: {other.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
                Destroy(gameObject);
            }
        }
    }
}
