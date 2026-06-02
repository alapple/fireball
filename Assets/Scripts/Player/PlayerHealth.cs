using UnityEngine;
using Fireball.Core;

namespace Fireball.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float swarmDecayThreshold = 3f; // Number of enemies to trigger decay
        [SerializeField] private float swarmDecayDamage = 10f; // Damage per second when swarmed
        [SerializeField] private float swarmDetectionRadius = 3f;
        [SerializeField] private LayerMask enemyLayer;

        private float currentHealth;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            CheckSwarmDecay();
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void CheckSwarmDecay()
        {
            Collider[] enemiesNearby = Physics.OverlapSphere(transform.position, swarmDetectionRadius, enemyLayer);
            if (enemiesNearby.Length >= swarmDecayThreshold)
            {
                TakeDamage(swarmDecayDamage * Time.deltaTime);
            }
        }

        private void Die()
        {
            Debug.Log("Player Died!");
            // Handle death (reload scene or game over UI)
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, swarmDetectionRadius);
        }
    }
}
