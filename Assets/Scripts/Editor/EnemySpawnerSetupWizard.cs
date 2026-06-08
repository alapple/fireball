using UnityEngine;
using UnityEditor;
using Fireball.Enemies;

namespace Fireball.Editor
{
    public class EnemySpawnerSetupWizard : EditorWindow
    {
        private GameObject enemyPrefab;

        [MenuItem("Tools/Fireball/Setup Enemy Spawner")]
        public static void ShowWindow()
        {
            GetWindow<EnemySpawnerSetupWizard>("Enemy Spawner Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Fireball Enemy Spawner Setup", EditorStyles.boldLabel);
            
            enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab", enemyPrefab, typeof(GameObject), false);

            if (GUILayout.Button("Create Spawner"))
            {
                CreateSpawner();
            }
        }

        private void CreateSpawner()
        {
            if (enemyPrefab == null)
            {
                Debug.LogError("Please assign an Enemy Prefab first!");
                return;
            }

            GameObject spawnerObj = new GameObject("EnemySpawner_" + enemyPrefab.name);
            EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();

            // Use SerializedObject to set the private [SerializeField] fields
            SerializedObject spawnerSerialized = new SerializedObject(spawner);
            spawnerSerialized.FindProperty("enemyPrefab").objectReferenceValue = enemyPrefab;
            
            // Set default stats
            spawnerSerialized.FindProperty("maxHealth").floatValue = 50f;
            spawnerSerialized.FindProperty("moveSpeed").floatValue = 3.5f;
            spawnerSerialized.FindProperty("attackRange").floatValue = 2f;
            spawnerSerialized.FindProperty("attackCooldown").floatValue = 1.5f;
            spawnerSerialized.FindProperty("damage").floatValue = 10f;
            spawnerSerialized.FindProperty("goldValue").intValue = 10;
            spawnerSerialized.FindProperty("spawnInterval").floatValue = 5f;
            spawnerSerialized.FindProperty("maxActiveSpawns").intValue = 3;
            spawnerSerialized.FindProperty("maxTotalSpawns").intValue = 20;

            spawnerSerialized.ApplyModifiedProperties();

            Selection.activeGameObject = spawnerObj;
            Debug.Log($"Created spawner for {enemyPrefab.name}");
        }
    }
}
