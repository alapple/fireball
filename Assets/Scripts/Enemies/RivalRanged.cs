using UnityEngine;

namespace Fireball.Enemies
{
    public class RivalRanged : EnemyBase
    {
        [Header("Ranged Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Vector3 rotationOffset = new Vector3(90, 0, 0);
        [SerializeField] private float preferredDistance = 8f;
        [SerializeField] private float shootRange = 12f;
        [SerializeField] private float strafeSpeed = 2f;

        private Vector3 strafeDir;
        private float nextStrafeTime;

        protected override void HandleBehavior(float distanceToPlayer)
        {
            if (player == null) return;

            // Update Animator speed
            if (animator != null && agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }

            // Movement Logic
            if (currentRole == SquadRole.Strafe)
            {
                if (distanceToPlayer > shootRange)
                {
                    // Too far! Get closer even if in strafe mode
                    if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(player.position);
                    }
                }
                else
                {
                    ExecuteStrafe();
                }
            }
            else
            {
                if (distanceToPlayer < preferredDistance * 0.8f)
                {
                    // Too close! Back up
                    Vector3 dirFromPlayer = (transform.position - player.position).normalized;
                    Vector3 retreatPos = player.position + dirFromPlayer * preferredDistance;
                    if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(retreatPos);
                    }
                }
                else if (distanceToPlayer > preferredDistance * 1.2f)
                {
                    // Too far! Get closer
                    if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(player.position);
                    }
                }
                else
                {
                    // In the sweet spot
                    if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh) agent.isStopped = true;
                }
            }

            // Shooting Logic
            if (distanceToPlayer <= shootRange)
            {
                RotateTowards(player.position);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    PrepareShoot();
                }
            }
            else
            {
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh) agent.isStopped = false;
            }
        }

        private void RotateTowards(Vector3 target)
        {
            Vector3 dir = (target - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
            }
        }

        private void ExecuteStrafe()
        {
            if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

            agent.isStopped = false;

            if (Time.time >= nextStrafeTime)
            {
                strafeDir = Random.value > 0.5f ? transform.right : -transform.right;
                nextStrafeTime = Time.time + Random.Range(1f, 3f);
            }

            Vector3 strafePos = transform.position + strafeDir * strafeSpeed;
            agent.SetDestination(strafePos);
        }

        private void PrepareShoot()
        {
            lastAttackTime = Time.time;
            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }
            else
            {
                // Fallback if no animator
                PerformShoot();
            }
        }

        // THIS METHOD IS CALLED BY ANIMATION EVENT
        public void PerformShoot()
        {
            if (projectilePrefab != null && firePoint != null)
            {
                Debug.Log($"{name} fired a projectile!");
                
                // 1. Calculate the actual flight path towards the player
                Vector3 targetDir = firePoint.forward;
                if (player != null)
                {
                    targetDir = ((player.position + Vector3.up * 0.5f) - firePoint.position).normalized;
                }

                // 2. Spawn the projectile
                Debug.DrawRay(firePoint.position, targetDir * shootRange, Color.yellow, 2f);
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(targetDir));
                
                // 3. Setup the projectile script and LOCK the flight path
                EnemyProjectile projScript = projectile.GetComponent<EnemyProjectile>();
                if (projScript == null) projScript = projectile.AddComponent<EnemyProjectile>();
                projScript.Initialize(targetDir, damage);

                // 4. APPLY VISUAL OFFSET (This only changes the rotation, not the path)
                projectile.transform.Rotate(rotationOffset);
            }
        }
    }
}
