using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Fireball.Enemies
{
    public class InfiniteSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject[] enemyPrefabs;
        
        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnRadius = 15f;
        [SerializeField] private float maxSpawnRadius = 30f;
        [SerializeField] private float spawnInterval = 3f;
        [SerializeField] private int maxActiveEnemies = 20;

        private Transform player;
        private float nextSpawnTime;
        private List<EnemyBase> activeEnemies = new List<EnemyBase>();

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (player == null) return;

            // Clean up dead enemies from list
            activeEnemies.RemoveAll(e => e == null);

            if (Time.time >= nextSpawnTime && activeEnemies.Count < maxActiveEnemies)
            {
                SpawnEnemyAroundPlayer();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }

        private void SpawnEnemyAroundPlayer()
        {
            if (enemyPrefabs.Length == 0) return;

            // Pick a random direction
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y) * distance;

            // Ensure the spawn position is on the NavMesh
            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject newEnemy = Instantiate(prefab, hit.position, Quaternion.identity);
                
                if (newEnemy.TryGetComponent(out EnemyBase enemy))
                {
                    activeEnemies.Add(enemy);
                }
            }
        }
    }
}
