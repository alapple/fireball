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

        public void Configure(float health, float speed, float range, float cooldown, float damage, int gold)
        {
            maxHealth = health;
            currentHealth = maxHealth;
            moveSpeed = speed;
            attackRange = range;
            attackCooldown = cooldown;
            this.damage = damage;
            goldValue = gold;

            if (agent != null)
            {
                agent.speed = moveSpeed;
            }
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
            Debug.Log($"{name} received {amount} damage. Current health: {currentHealth - amount}");
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected System.Action<EnemyBase> onDeathCallback;

        public void SetDeathCallback(System.Action<EnemyBase> callback)
        {
            onDeathCallback = callback;
        }

        protected virtual void Die()
        {
            onDeathCallback?.Invoke(this);

            // Actually, let's use a simpler approach: Find any ShopManager and add gold there.
            ShopManager shop = FindObjectOfType<ShopManager>();
            if (shop != null)
            {
                shop.AddGold(goldValue);
            }

            Destroy(gameObject);
        }
    }
}
