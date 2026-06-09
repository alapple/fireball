using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Fireball.Enemies;

namespace Fireball.Editor
{
    public class EnemyPrefabCreator : EditorWindow
    {
        private GameObject modelFBX;
        private string prefabName = "RivalMelee_Knight";

        [MenuItem("Tools/Fireball/Enemy Prefab Creator")]
        public static void ShowWindow()
        {
            GetWindow<EnemyPrefabCreator>("Enemy Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Convert FBX to Enemy Prefab", EditorStyles.boldLabel);
            
            modelFBX = (GameObject)EditorGUILayout.ObjectField("Model FBX", modelFBX, typeof(GameObject), false);
            prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);

            if (GUILayout.Button("Create Melee Enemy Prefab"))
            {
                CreatePrefab();
            }
        }

        private void CreatePrefab()
        {
            if (modelFBX == null)
            {
                Debug.LogError("Please assign a Model FBX!");
                return;
            }

            // 1. Instantiate the FBX in the scene temporarily
            GameObject enemyObj = (GameObject)PrefabUtility.InstantiatePrefab(modelFBX);
            enemyObj.name = prefabName;

            // 2. Add NavMeshAgent
            NavMeshAgent agent = enemyObj.GetComponent<NavMeshAgent>();
            if (agent == null) agent = enemyObj.AddComponent<NavMeshAgent>();
            agent.speed = 4f;
            agent.acceleration = 12f;
            agent.angularSpeed = 360f;
            agent.stoppingDistance = 1.2f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            // 3. Add Collider
            CapsuleCollider col = enemyObj.GetComponent<CapsuleCollider>();
            if (col == null) col = enemyObj.AddComponent<CapsuleCollider>();
            col.center = new Vector3(0, 1, 0);
            col.radius = 0.5f;
            col.height = 2f;

            // 4. Add AI Script
            RivalMelee melee = enemyObj.GetComponent<RivalMelee>();
            if (melee == null) melee = enemyObj.AddComponent<RivalMelee>();

            // 5. Save as Prefab
            string path = "Assets/Prefabs/" + prefabName + ".prefab";
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            PrefabUtility.SaveAsPrefabAsset(enemyObj, path);
            
            // 6. Cleanup scene
            DestroyImmediate(enemyObj);

            Debug.Log("Successfully created Enemy Prefab at: " + path);
            AssetDatabase.Refresh();
        }
    }
}
