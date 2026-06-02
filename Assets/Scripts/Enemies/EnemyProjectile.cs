using UnityEngine;
using Fireball.Core;

namespace Fireball.Enemies
{
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifetime = 5f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
