using UnityEngine;
using System.Collections.Generic;
using Fireball.Enemies;

namespace Fireball.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject enemyPrefab;

        [Header("Stats Overrides")]
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private int goldValue = 10;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 5f;
        [SerializeField] private int maxActiveSpawns = 3; // Soft limit
        [SerializeField] private int maxTotalSpawns = 20; // Hard limit
        
        private float nextSpawnTime;
        private List<EnemyBase> activeEnemies = new List<EnemyBase>();
        private int totalSpawnedCount = 0;

        private void Update()
        {
            if (Time.time >= nextSpawnTime && CanSpawn())
            {
                SpawnEnemy();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }

        private bool CanSpawn()
        {
            return activeEnemies.Count < maxActiveSpawns && totalSpawnedCount < maxTotalSpawns;
        }

        private void SpawnEnemy()
        {
            if (enemyPrefab != null)
            {
                // FIND A SAFE POSITION ON THE NAVMESH
                Vector3 spawnPos = transform.position;
                if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out UnityEngine.AI.NavMeshHit hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
                {
                    spawnPos = hit.position;
                }
                else
                {
                    Debug.LogWarning($"Spawner {name} could not find NavMesh at {transform.position}. Enemy might float!");
                }

                GameObject newEnemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                
                // Ensure it has the base AI script (Melee by default if missing)
                EnemyBase enemy = newEnemyObj.GetComponent<EnemyBase>();
                if (enemy == null)
                {
                    Debug.Log($"Adding RivalMelee to {newEnemyObj.name} because it was missing an AI script.");
                    enemy = newEnemyObj.AddComponent<RivalMelee>();
                }

                // Ensure it has a NavMeshAgent
                UnityEngine.AI.NavMeshAgent agent = newEnemyObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent == null)
                {
                    agent = newEnemyObj.AddComponent<UnityEngine.AI.NavMeshAgent>();
                }
                
                agent.speed = moveSpeed;
                agent.acceleration = 12f;
                agent.angularSpeed = 360f;
                agent.stoppingDistance = 1.2f;
                agent.enabled = true; // Ensure it's on

                // Ensure it has a Collider for hit detection
                if (newEnemyObj.GetComponent<Collider>() == null)
                {
                    var col = newEnemyObj.AddComponent<CapsuleCollider>();
                    col.center = new Vector3(0, 1, 0);
                    col.height = 2f;
                }

                enemy.Configure(maxHealth, moveSpeed, attackRange, attackCooldown, damage, goldValue);
                enemy.SetDeathCallback(OnEnemyDied);
                
                activeEnemies.Add(enemy);
                totalSpawnedCount++;
            }
        }

        private void OnEnemyDied(EnemyBase enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }
        }
    }
}
