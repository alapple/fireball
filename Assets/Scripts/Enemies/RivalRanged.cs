using UnityEngine;

namespace Fireball.Enemies
{
    public class RivalRanged : EnemyBase
    {
        [Header("Ranged Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float preferredDistance = 8f;
        [SerializeField] private float strafeSpeed = 2f;

        private Vector3 strafeDir;
        private float nextStrafeTime;

        protected override void HandleBehavior(float distanceToPlayer)
        {
            if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

            if (currentRole == SquadRole.Strafe)
            {
                ExecuteStrafe();
            }
            else
            {
                if (distanceToPlayer <= preferredDistance)
                {
                    // Try to keep distance
                    Vector3 dirToPlayer = (transform.position - player.position).normalized;
                    Vector3 retreatPos = player.position + dirToPlayer * preferredDistance;
                    agent.SetDestination(retreatPos);
                }
                else
                {
                    agent.SetDestination(player.position);
                }
            }

            if (distanceToPlayer <= preferredDistance * 1.5f)
            {
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    Shoot();
                }
            }
        }

        private void ExecuteStrafe()
        {
            if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

            if (Time.time >= nextStrafeTime)
            {
                strafeDir = Random.value > 0.5f ? transform.right : -transform.right;
                nextStrafeTime = Time.time + Random.Range(1f, 3f);
            }

            Vector3 strafePos = transform.position + strafeDir * strafeSpeed;
            agent.SetDestination(strafePos);
        }

        private void Shoot()
        {
            lastAttackTime = Time.time;
            if (projectilePrefab != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                // Projectile logic should handle movement and damage
            }
        }
    }
}
