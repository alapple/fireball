using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Fireball.Enemies
{
    public enum SquadRole { None, Charge, ShieldWall, Flank, Strafe }

    public class AISquadManager : MonoBehaviour
    {
        public static AISquadManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float updateInterval = 2f;
        [SerializeField] private float shieldWallSpacing = 1.5f;
        [SerializeField] private float flankAngle = 120f;

        private List<EnemyBase> activeEnemies = new List<EnemyBase>();
        private Transform player;
        private float nextUpdateTime;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        public void RegisterEnemy(EnemyBase enemy)
        {
            if (!activeEnemies.Contains(enemy))
                activeEnemies.Add(enemy);
        }

        public void UnregisterEnemy(EnemyBase enemy)
        {
            activeEnemies.Remove(enemy);
        }

        private void Update()
        {
            if (Time.time >= nextUpdateTime)
            {
                AssignTactics();
                nextUpdateTime = Time.time + updateInterval;
            }
        }

        private void AssignTactics()
        {
            if (player == null) return;

            var meleeEnemies = activeEnemies.OfType<RivalMelee>().ToList();
            var rangedEnemies = activeEnemies.OfType<RivalRanged>().ToList();

            // Coordinate Melee: 2-3 form a shield wall, others flank
            int shieldWallCount = Mathf.Min(meleeEnemies.Count, 3);
            
            for (int i = 0; i < meleeEnemies.Count; i++)
            {
                if (i < shieldWallCount)
                {
                    Vector3 formationPos = CalculateShieldWallPos(i, shieldWallCount);
                    meleeEnemies[i].AssignRole(SquadRole.ShieldWall, formationPos);
                }
                else
                {
                    Vector3 flankPos = CalculateFlankPos(i);
                    meleeEnemies[i].AssignRole(SquadRole.Flank, flankPos);
                }
            }

            // Coordinate Ranged: All strafe independently
            foreach (var ranged in rangedEnemies)
            {
                ranged.AssignRole(SquadRole.Strafe, Vector3.zero);
            }
        }

        private Vector3 CalculateShieldWallPos(int index, int total)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, dirToPlayer);
            
            float offset = (index - (total - 1) / 2f) * shieldWallSpacing;
            return player.position - dirToPlayer * 4f + right * offset;
        }

        private Vector3 CalculateFlankPos(int index)
        {
            float angle = (index % 2 == 0 ? flankAngle : -flankAngle) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * 6f;
            return player.position + offset;
        }
    }
}
