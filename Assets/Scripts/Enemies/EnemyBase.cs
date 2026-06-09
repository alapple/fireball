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
        protected Animator animator;
        protected Transform player;
        protected float lastAttackTime;

        protected SquadRole currentRole = SquadRole.None;
        protected Vector3 targetPosition;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>(); // Characters often have animator on a child model
            if (animator == null) animator = GetComponent<Animator>();

            if (agent != null)
            {
                agent.speed = moveSpeed;
                agent.isStopped = false;
            }
            
            FindPlayer();
        }

        protected virtual void OnEnable()
        {
            if (AISquadManager.Instance == null)
            {
                Debug.LogWarning($"{name} enabled but AISquadManager.Instance is null.");
            }
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
            if (player == null)
            {
                FindPlayer();
                if (player == null) return;
            }

            float distance = Vector3.Distance(transform.position, player.position);

            // NAVMESH FALLBACK SYSTEM
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                HandleBehavior(distance);
            }
            else
            {
                // Simple 'Slide towards player' fallback so the game doesn't break without NavMesh
                if (distance > attackRange)
                {
                    Vector3 moveDir = (player.position - transform.position).normalized;
                    moveDir.y = 0;
                    transform.position += moveDir * moveSpeed * Time.deltaTime;
                    
                    if (moveDir != Vector3.zero)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 10f);
                    
                    if (animator != null) animator.SetFloat("Speed", moveSpeed);
                }
                else
                {
                    if (animator != null) animator.SetFloat("Speed", 0);
                    HandleBehavior(distance); // Still allow attack logic
                }
            }
        }

        private void FindPlayer()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
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

        protected System.Action<EnemyBase> onDeathCallback;

        public void SetDeathCallback(System.Action<EnemyBase> callback)
        {
            onDeathCallback = callback;
        }

        protected virtual void Die()
        {
            onDeathCallback?.Invoke(this);
            ShopManager shop = FindObjectOfType<ShopManager>();
            if (shop != null) shop.AddGold(goldValue);
            Destroy(gameObject);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * attackRange);
        }
    }
}
