using UnityEngine;
using Fireball.Core;

namespace Fireball.Enemies
{
    public class RivalMelee : EnemyBase
    {
        [Header("Melee Settings")]
        [SerializeField] private float chargeThreshold = 8f;
        [SerializeField] private float shieldWallDamageMultiplier = 0.3f;

        private int attackCounter = 0;

        protected override void HandleBehavior(float distanceToPlayer)
        {
            if (player == null) return;

            // Update Animator speed
            if (animator != null && agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }

            // USE HORIZONTAL DISTANCE for more "fair" feeling melee and matching the 2D Gizmo circle
            Vector3 playerPos = player.position;
            Vector3 myPos = transform.position;
            playerPos.y = 0;
            myPos.y = 0;
            float horizontalDistance = Vector3.Distance(myPos, playerPos);

            if (horizontalDistance <= attackRange)
            {
                // In strike range
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh) agent.isStopped = true;
                RotateTowards(player.position);

                // Only actually start the swing if we are facing the player
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                dirToPlayer.y = 0;
                float angleDot = Vector3.Dot(transform.forward, dirToPlayer);

                if (angleDot > 0.8f && Time.time >= lastAttackTime + attackCooldown)
                {
                    Attack(horizontalDistance);
                }
            }
            else
            {
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh) 
                {
                    agent.isStopped = false;

                    // AGGRESSIVE MELEE PATHFINDING
                    if (distanceToPlayer < chargeThreshold || currentRole == SquadRole.None || currentRole == SquadRole.Charge)
                    {
                        agent.speed = moveSpeed;
                        agent.SetDestination(player.position);
                    }
                    else
                    {
                        ExecuteSquadTactic();
                    }

                    // Smoothly look where we're going
                    if (agent.velocity.sqrMagnitude > 0.1f)
                    {
                        RotateTowards(transform.position + agent.velocity);
                    }
                }
            }
        }

        private void RotateTowards(Vector3 target)
        {
            Vector3 dir = (target - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 12f);
            }
        }

        private void ExecuteSquadTactic()
        {
            if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

            switch (currentRole)
            {
                case SquadRole.ShieldWall:
                    agent.speed = moveSpeed * 0.8f;
                    agent.SetDestination(targetPosition);
                    break;
                case SquadRole.Flank:
                    agent.speed = moveSpeed * 1.3f;
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
            // Shield wall protection from front
            if (currentRole == SquadRole.ShieldWall)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dirToPlayer);
                if (dot > 0.5f)
                {
                    amount *= shieldWallDamageMultiplier;
                }
            }
            base.TakeDamage(amount);
        }

        private void Attack(float dist)
        {
            lastAttackTime = Time.time;
            Debug.Log($"{name} ATTACK TRIGGERED! Horizontal Dist: {dist:F2}, Range: {attackRange}");
            
            if (animator != null)
            {
                // Sequence logic: 2 times Animation A, 1 time Animation B
                if (attackCounter < 2)
                {
                    animator.SetTrigger("AttackLight");
                    attackCounter++;
                }
                else
                {
                    animator.SetTrigger("AttackHeavy");
                    attackCounter = 0; // Reset sequence
                }
            }
            // Damage is now handled by PerformDamage() via Animation Event
        }

        // THIS METHOD IS CALLED BY ANIMATION EVENT
        public void PerformDamage()
        {
            if (player == null) return;

            // Use horizontal distance here too for consistency
            Vector3 playerPos = player.position;
            Vector3 myPos = transform.position;
            playerPos.y = 0;
            myPos.y = 0;
            float horizontalDistance = Vector3.Distance(myPos, playerPos);

            if (horizontalDistance <= attackRange + 0.5f) // Small buffer for fairness
            {
                Debug.Log($"{name} sword hit player for {damage} damage!");
                if (player.TryGetComponent(out IDamageable playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }
}
