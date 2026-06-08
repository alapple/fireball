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
        [SerializeField] private float attackRange = 2f;
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
                GameObject newEnemyObj = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                if (newEnemyObj.TryGetComponent(out EnemyBase enemy))
                {
                    enemy.Configure(maxHealth, moveSpeed, attackRange, attackCooldown, damage, goldValue);
                    enemy.SetDeathCallback(OnEnemyDied);
                    
                    activeEnemies.Add(enemy);
                    totalSpawnedCount++;
                }
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
