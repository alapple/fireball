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

        public void SetDirection(Vector3 dir)
        {
            moveDirection = dir.normalized;
            directionSet = true;
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
            // Move in the locked direction, regardless of how the model is rotated
            transform.position += moveDirection * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Default") || other.CompareTag("Environment"))
            {
                Destroy(gameObject);
            }
        }
    }
}
