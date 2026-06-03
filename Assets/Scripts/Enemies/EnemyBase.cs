using UnityEngine;
using UnityEngine.AI;
using Fireball.Core;

namespace Fireball.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        [SerializeField] protected float maxHealth = 50f;
        [SerializeField] protected float moveSpeed = 3.5f;
        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float attackCooldown = 1.5f;
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected int goldValue = 10;

        protected float currentHealth;
        protected NavMeshAgent agent;
        protected Transform player;
        protected float lastAttackTime;

        protected SquadRole currentRole = SquadRole.None;
        protected Vector3 targetPosition;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        protected virtual void OnEnable()
        {
            AISquadManager.Instance?.RegisterEnemy(this);
        }

        protected virtual void OnDisable()
        {
            AISquadManager.Instance?.UnregisterEnemy(this);
        }

        public void AssignRole(SquadRole role, Vector3 pos)
        {
            currentRole = role;
            targetPosition = pos;
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;
        }

        protected virtual void Update()
        {
            if (player == null) return;

            float distance = Vector3.Distance(transform.position, player.position);
            HandleBehavior(distance);
        }

        protected abstract void HandleBehavior(float distanceToPlayer);

        public virtual void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            if (player != null && player.TryGetComponent(out Fireball.Player.PlayerHealth playerHealth))
            {
                // Access stats through player health
                // We'll assume the player has a way to get gold added
                // For simplicity, let's find the ShopManager if it exists or just add to the ScriptableObject directly if we can find it.
                // Better: Let's assume the ScriptableObject is a singleton-like asset or we find it.
                // For now, let's just use the direct addition if we can get a reference.
            }
            
            // Actually, let's use a simpler approach: Find any ShopManager and add gold there.
            ShopManager shop = FindFirstObjectByType<ShopManager>();
            if (shop != null)
            {
                shop.AddGold(goldValue);
            }

            Destroy(gameObject);
        }
    }
}
