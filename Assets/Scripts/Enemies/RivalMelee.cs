using UnityEngine;
using Fireball.Core;

namespace Fireball.Enemies
{
    public class RivalMelee : EnemyBase
    {
        [Header("Melee Settings")]
        [SerializeField] private float flankingDistance = 4f;
        [SerializeField] private float shieldWallDamageMultiplier = 0.3f;

        protected override void HandleBehavior(float distanceToPlayer)
        {
            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                agent.isStopped = false;
                ExecuteSquadTactic();
            }
        }

        private void ExecuteSquadTactic()
        {
            if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

            switch (currentRole)
            {
                case SquadRole.ShieldWall:
                    agent.speed = moveSpeed * 0.5f;
                    agent.SetDestination(targetPosition);
                    break;
                case SquadRole.Flank:
                    agent.speed = moveSpeed * 1.2f;
                    agent.SetDestination(targetPosition);
                    break;
                default:
                    agent.speed = moveSpeed;
                    agent.SetDestination(player.position);
                    break;
            }
        }

        public override void TakeDamage(float amount)
        {
            if (currentRole == SquadRole.ShieldWall)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dirToPlayer);
                if (dot > 0.5f) // Hit from the front
                {
                    amount *= shieldWallDamageMultiplier;
                }
            }
            base.TakeDamage(amount);
        }

        private void Attack()
        {
            lastAttackTime = Time.time;
            if (player.TryGetComponent(out IDamageable playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
