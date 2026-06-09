using UnityEngine;
using UnityEditor;
using Fireball.ScriptableObjects;
using Fireball.Weapons;
using Fireball.UI;
using TMPro;

namespace Fireball.Editor
{
    public class UISetupWizard : EditorWindow
    {
        [MenuItem("Tools/Fireball/Setup UI and Popups")]
        public static void ShowWindow()
        {
            GetWindow<UISetupWizard>("UI Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Fireball UI & Damage Popup Setup", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This tool will setup the Player HUD and Damage Popup Manager without affecting player mechanics.", MessageType.Info);
            
            if (GUILayout.Button("Setup UI and Popups"))
            {
                SetupUI();
            }
        }

        private static void SetupUI()
        {
            // 0. Setup Tags
            CreateTag("Enemy");

            // 1. Find Dependencies
            PlayerStats stats = AssetDatabase.LoadAssetAtPath<PlayerStats>("Assets/ScriptableObjects/PlayerStats.asset");
            WeaponDualWieldManager wdm = Object.FindAnyObjectByType<WeaponDualWieldManager>();
            
            if (stats == null)
            {
                Debug.LogError("UI Setup: Could not find PlayerStats at Assets/ScriptableObjects/PlayerStats.asset");
                return;
            }

            // 2. Setup/Create Damage Popup Prefab
            GameObject popupPrefab = CreateOrUpdatePopupPrefab();

            // 3. Setup Damage Popup Manager
            GameObject popupManagerObj = GameObject.Find("DamagePopupManager");
            if (popupManagerObj == null)
            {
                popupManagerObj = new GameObject("DamagePopupManager");
            }
            DamagePopupManager dpm = GetOrAddComponent<DamagePopupManager>(popupManagerObj);
            
            if (popupPrefab != null)
            {
                var dpmSerialized = new SerializedObject(dpm);
                dpmSerialized.FindProperty("_popupPrefab").objectReferenceValue = popupPrefab;
                dpmSerialized.ApplyModifiedProperties();
            }

            // 4. Setup Player HUD
            GameObject hudObj = GameObject.Find("PlayerHUD");
            if (hudObj == null)
            {
                hudObj = new GameObject("PlayerHUD");
            }
            
            var uiDoc = GetOrAddComponent<UnityEngine.UIElements.UIDocument>(hudObj);
            var uxml = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.VisualTreeAsset>("Assets/Scripts/UI/PlayerHUD.uxml");
            if (uxml != null) uiDoc.visualTreeAsset = uxml;

            var panelSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.PanelSettings>("Assets/UI Toolkit/PanelSettings.asset");
            if (panelSettings != null) uiDoc.panelSettings = panelSettings;

            PlayerHUD hudScript = GetOrAddComponent<PlayerHUD>(hudObj);
            var hudSerialized = new SerializedObject(hudScript);
            hudSerialized.FindProperty("playerStats").objectReferenceValue = stats;
            hudSerialized.FindProperty("weaponManager").objectReferenceValue = wdm;
            hudSerialized.ApplyModifiedProperties();

            // 5. Ensure MainCamera tag for Popups
            Camera cam = Camera.main;
            if (cam == null)
            {
                cam = Object.FindAnyObjectByType<Camera>();
                if (cam != null) cam.tag = "MainCamera";
            }

            Debug.Log("UI and Popups Setup Complete! DamagePopup prefab has been (re)generated.");
        }

        private static GameObject CreateOrUpdatePopupPrefab()
        {
            string prefabPath = "Assets/Prefabs/DamagePopup.prefab";
            
            // Create a temporary object to turn into a prefab
            GameObject tempObj = new GameObject("DamagePopup_Temp");
            
            // Add TMP (3D VERSION, NOT UI VERSION)
            TextMeshPro textMesh = tempObj.AddComponent<TextMeshPro>();
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = 6;
            textMesh.color = Color.yellow;
            textMesh.outlineWidth = 0.2f;
            textMesh.outlineColor = Color.black;
            textMesh.text = "0";

            // Add our script
            tempObj.AddComponent<DamagePopup>();

            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            // Save as Prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(tempObj, prefabPath);
            DestroyImmediate(tempObj);
            
            return prefab;
        }

        private static void CreateTag(string tagName)
        {
            var tagManagerObj = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (tagManagerObj == null || tagManagerObj.Length == 0) return;

            SerializedObject tagManager = new SerializedObject(tagManagerObj[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tagName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"UI Setup: Created missing tag '{tagName}'");
            }
        }

        private static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null) component = obj.AddComponent<T>();
            return component;
        }
    }
}
