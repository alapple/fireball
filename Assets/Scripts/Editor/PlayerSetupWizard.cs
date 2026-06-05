using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Fireball.Player;
using Fireball.Weapons;
using Fireball.ScriptableObjects;
using System.Collections.Generic;

namespace Fireball.Editor
{
    public class PlayerSetupWizard : EditorWindow
    {
        [MenuItem("Tools/Fireball/Setup Player")]
        public static void ShowWindow()
        {
            GetWindow<PlayerSetupWizard>("Player Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Fireball Player Setup", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Create/Setup Player"))
            {
                SetupPlayer();
            }
        }

        private static void SetupPlayer()
        {
            // 1. Create Player Root
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = new GameObject("Player");
                player.tag = "Player";
            }

            // 2. Add Components
            CharacterController cc = GetOrAddComponent<CharacterController>(player);
            PlayerInput input = GetOrAddComponent<PlayerInput>(player);
            PlayerController pc = GetOrAddComponent<PlayerController>(player);
            PlayerHealth health = GetOrAddComponent<PlayerHealth>(player);

            // 3. Setup Camera
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.transform.SetParent(player.transform);
                camObj.transform.localPosition = new Vector3(0, 0.8f, 0);
                cam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }
            
            // Assign camera to controller
            var pcSerialized = new SerializedObject(pc);
            pcSerialized.FindProperty("cameraTransform").objectReferenceValue = cam.transform;
            pcSerialized.ApplyModifiedProperties();

            // 4. Setup Input
            InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (actions != null)
            {
                input.actions = actions;
                input.defaultControlScheme = "Keyboard&Mouse";
                input.notificationBehavior = PlayerNotifications.BroadcastMessages;
            }

            // 5. Setup Health/Stats
            PlayerStats stats = AssetDatabase.LoadAssetAtPath<PlayerStats>("Assets/ScriptableObjects/PlayerStats.asset");
            if (stats != null)
            {
                var healthSerialized = new SerializedObject(health);
                healthSerialized.FindProperty("playerStats").objectReferenceValue = stats;
                healthSerialized.ApplyModifiedProperties();
            }

            // 6. Setup Weapons
            GameObject weaponManagerObj = null;
            Transform wmTransform = cam.transform.Find("WeaponManager");
            if (wmTransform != null) weaponManagerObj = wmTransform.gameObject;
            else
            {
                weaponManagerObj = new GameObject("WeaponManager");
                weaponManagerObj.transform.SetParent(cam.transform);
                weaponManagerObj.transform.localPosition = new Vector3(0, -0.3f, 0.5f);
                weaponManagerObj.transform.localRotation = Quaternion.identity;
            }

            WeaponDualWieldManager wdm = GetOrAddComponent<WeaponDualWieldManager>(weaponManagerObj);
            
            // Setup Hands
            SetupHand(weaponManagerObj.transform, "LeftHand", typeof(ChampagneFlamethrower), "Assets/ScriptableObjects/ChampaineData.asset", stats);
            SetupHand(weaponManagerObj.transform, "RightHand", typeof(MoonshineMolotov), "Assets/ScriptableObjects/MoonshineData.asset", stats);

            // Link Weapons to Manager
            List<BottleWeapon> weapons = new List<BottleWeapon>(weaponManagerObj.GetComponentsInChildren<BottleWeapon>(true));
            var wdmSerialized = new SerializedObject(wdm);
            var allWeaponsProp = wdmSerialized.FindProperty("allWeapons");
            allWeaponsProp.ClearArray();
            for (int i = 0; i < weapons.Count; i++)
            {
                allWeaponsProp.InsertArrayElementAtIndex(i);
                allWeaponsProp.GetArrayElementAtIndex(i).objectReferenceValue = weapons[i];
            }
            wdmSerialized.FindProperty("playerController").objectReferenceValue = pc;
            wdmSerialized.ApplyModifiedProperties();

            Selection.activeGameObject = player;
            Debug.Log("Player Setup Complete!");
        }

        private static void SetupHand(Transform parent, string name, System.Type weaponType, string dataPath, PlayerStats stats)
        {
            Transform hand = parent.Find(name);
            if (hand == null)
            {
                hand = new GameObject(name).transform;
                hand.SetParent(parent);
                hand.localPosition = (name == "LeftHand") ? new Vector3(-0.4f, -0.2f, 0.4f) : new Vector3(0.4f, -0.2f, 0.4f);
            }

            // Add Visual Hand Placeholder
            Transform handVisual = hand.Find("HandVisual");
            if (handVisual == null)
            {
                GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                capsule.name = "HandVisual";
                handVisual = capsule.transform;
                handVisual.SetParent(hand);
                handVisual.localPosition = Vector3.zero;
                handVisual.localRotation = Quaternion.Euler(90, 0, 0);
                handVisual.localScale = new Vector3(0.15f, 0.2f, 0.15f);
                DestroyImmediate(capsule.GetComponent<Collider>());
            }

            // Add Bottle Model for Champagne
            if (weaponType == typeof(ChampagneFlamethrower))
            {
                Transform bottleModel = hand.Find("BottleModel");
                if (bottleModel == null)
                {
                    GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/3d/Bottle of Champagne/Bottle_of_Champagne_01.obj");
                    if (modelPrefab != null)
                    {
                        GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
                        model.name = "BottleModel";
                        bottleModel = model.transform;
                        bottleModel.SetParent(hand);
                        bottleModel.localPosition = new Vector3(0, 0.2f, 0.1f);
                        bottleModel.localRotation = Quaternion.Euler(-90, 0, 0);
                        bottleModel.localScale = Vector3.one * 0.5f;
                    }
                }
            }

            BottleWeapon weapon = (BottleWeapon)GetOrAddComponent(hand.gameObject, weaponType);
            WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(dataPath);
            
            var weaponSerialized = new SerializedObject(weapon);
            weaponSerialized.FindProperty("weaponData").objectReferenceValue = data;
            weaponSerialized.FindProperty("playerStats").objectReferenceValue = stats;
            
            // Special setup for Champagne (Particle System)
            if (weaponType == typeof(ChampagneFlamethrower))
            {
                ParticleSystem ps = hand.GetComponentInChildren<ParticleSystem>();
                if (ps == null)
                {
                    GameObject psObj = new GameObject("FoamParticles");
                    psObj.transform.SetParent(hand);
                    psObj.transform.localPosition = Vector3.zero;
                    psObj.transform.localRotation = Quaternion.identity;
                    ps = psObj.AddComponent<ParticleSystem>();
                    
                    // Main Module
                    var main = ps.main;
                    main.duration = 1.0f;
                    main.loop = true;
                    main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.7f);
                    main.startSpeed = new ParticleSystem.MinMaxCurve(15f, 22f);
                    main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f); // Variety of bubble sizes
                    main.gravityModifier = 0.8f; // More weight to the foam
                    main.simulationSpace = ParticleSystemSimulationSpace.World;
                    
                    // Emission
                    var emission = ps.emission;
                    emission.rateOverTime = 150f; // Denser foam

                    // Shape
                    var shape = ps.shape;
                    shape.shapeType = ParticleSystemShapeType.Cone;
                    shape.angle = 2.5f; // Even tighter stream
                    shape.radius = 0.02f;

                    // Color over Lifetime (Pure White to Slight Yellow/Cream)
                    var colorOverLifetime = ps.colorOverLifetime;
                    colorOverLifetime.enabled = true;
                    Gradient gradient = new Gradient();
                    gradient.SetKeys(
                        new GradientColorKey[] { 
                            new GradientColorKey(Color.white, 0.0f), 
                            new GradientColorKey(new Color(0.95f, 0.95f, 0.8f), 0.5f), // Creamy foam
                            new GradientColorKey(Color.white, 1.0f) 
                        },
                        new GradientAlphaKey[] { 
                            new GradientAlphaKey(1.0f, 0.0f), 
                            new GradientAlphaKey(1.0f, 0.7f), 
                            new GradientAlphaKey(0.0f, 1.0f) 
                        }
                    );
                    colorOverLifetime.color = gradient;

                    // Size over Lifetime (Blobs grow as they travel)
                    var sizeOverLifetime = ps.sizeOverLifetime;
                    sizeOverLifetime.enabled = true;
                    AnimationCurve curve = new AnimationCurve();
                    curve.AddKey(0.0f, 0.1f);
                    curve.AddKey(0.2f, 0.8f);
                    curve.AddKey(1.0f, 1.2f);
                    sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);

                    // Rotation over Lifetime (Bubbles spin)
                    var rot = ps.rotationOverLifetime;
                    rot.enabled = true;
                    rot.z = new ParticleSystem.MinMaxCurve(-180, 180);

                    // Renderer: MESH MODE for Low Poly look
                    var renderer = ps.GetComponent<ParticleSystemRenderer>();
                    renderer.renderMode = ParticleSystemRenderMode.Mesh;
                    renderer.mesh = AssetDatabase.GetBuiltinExtraResource<Mesh>("Sphere.fbx");
                    renderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat"); // Opaque white
                    renderer.minParticleSize = 0.001f;
                    renderer.maxParticleSize = 1.0f;
                }
                weaponSerialized.FindProperty("foamParticles").objectReferenceValue = ps;
                weaponSerialized.FindProperty("hitLayers").intValue = -1; // Everything
            }

            // Special setup for Moonshine (Fire Point)
            if (weaponType == typeof(MoonshineMolotov))
            {
                Transform firePoint = hand.Find("FirePoint");
                if (firePoint == null)
                {
                    firePoint = new GameObject("FirePoint").transform;
                    firePoint.SetParent(hand);
                    firePoint.localPosition = Vector3.forward * 0.2f;
                }
                weaponSerialized.FindProperty("firePoint").objectReferenceValue = firePoint;
            }

            weaponSerialized.ApplyModifiedProperties();
        }

        private static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null) component = obj.AddComponent<T>();
            return component;
        }

        private static Component GetOrAddComponent(GameObject obj, System.Type type)
        {
            Component component = obj.GetComponent(type);
            if (component == null) component = obj.AddComponent(type);
            return component;
        }
    }
}
