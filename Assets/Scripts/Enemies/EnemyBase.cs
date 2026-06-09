using UnityEngine;
using UnityEngine.AI;
using Fireball.Core;
using Fireball.UI;

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

            if (animator == null)
            {
                Debug.LogWarning($"{name} could not find an Animator component! Animations will not play.");
            }

            if (agent != null)
            {
                agent.speed = moveSpeed;
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                }
                else
                {
                    Debug.LogWarning($"{name} spawned but is not on a NavMesh. Movement will use fallback.");
                }
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

        private bool hasAttemptedWarp = false;

        protected virtual void Update()
        {
            if (player == null)
            {
                FindPlayer();
                if (player == null) return;
            }

            // ALWAYS visualize forward and range in Scene View for debugging
            Debug.DrawRay(transform.position + Vector3.up, transform.forward * attackRange, Color.cyan);

            float distance = Vector3.Distance(transform.position, player.position);

            // NAVMESH ONLY MOVEMENT
            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                HandleBehavior(distance);
            }
            else
            {
                // AUTO-WARP ATTEMPT: If we're off-mesh, try to find the nearest valid spot once
                if (!hasAttemptedWarp && agent != null && agent.enabled)
                {
                    if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                    {
                        Debug.Log($"{name} was off-mesh. Warping to nearest valid point at {hit.position}");
                        agent.Warp(hit.position);
                        hasAttemptedWarp = true;
                        return; // Wait for next frame
                    }
                    hasAttemptedWarp = true; // Only try once to avoid performance issues
                }

                // If still not on NavMesh, don't move.
                if (Time.frameCount % 120 == 0) // Don't spam
                {
                    Debug.LogWarning($"{name} is NOT on a NavMesh! Position: {transform.position}. Ensure your floor is marked 'Static' and Baked.");
                }
                
                if (animator != null) animator.SetFloat("Speed", 0);
            }
        }

        private void FindPlayer()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        protected abstract void HandleBehavior(float distanceToPlayer);

        private float _damageBuffer = 0f;
        private float _lastPopupTime = 0f;
        private const float POPUP_COOLDOWN = 0.15f;

        public virtual void TakeDamage(float amount)
        {
            currentHealth -= amount;
            _damageBuffer += amount;
            
            // Damage Popup Logic: Batch small damage (like flamethrower) 
            // but show large damage (like molotov) immediately.
            if (Time.time - _lastPopupTime >= POPUP_COOLDOWN || amount > 5f)
            {
                if (DamagePopupManager.Instance != null && _damageBuffer > 0.1f)
                {
                    DamagePopupManager.Instance.CreatePopup(transform.position + Vector3.up * 2f, _damageBuffer);
                    _damageBuffer = 0f;
                    _lastPopupTime = Time.time;
                }
            }

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
