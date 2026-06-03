using UnityEngine;
using Fireball.Core;
using Fireball.ScriptableObjects;

namespace Fireball.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] private PlayerStats playerStats;

        [Header("Swarm Settings")]
        [SerializeField] private float swarmDecayThreshold = 3f; 
        [SerializeField] private float swarmDecayDamage = 10f; 
        [SerializeField] private float swarmDetectionRadius = 3f;
        [SerializeField] private LayerMask enemyLayer;

        public float CurrentHealth => playerStats.currentHealth;
        public float MaxHealth => playerStats.maxHealth;

        private void Start()
        {
            if (playerStats != null)
            {
                playerStats.currentHealth = playerStats.maxHealth;
            }
        }

        private void Update()
        {
            CheckSwarmDecay();
        }

        public void TakeDamage(float amount)
        {
            if (playerStats == null) return;

            // Apply armor reduction (e.g., each armor level reduces damage by 10%, max 50%)
            float reduction = Mathf.Min(playerStats.armorLevel * 0.1f, 0.5f);
            float finalDamage = amount * (1f - reduction);

            playerStats.currentHealth -= finalDamage;
            if (playerStats.currentHealth <= 0)
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
