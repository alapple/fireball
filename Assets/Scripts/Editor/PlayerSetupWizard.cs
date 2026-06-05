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
            GUILayout.Label("Fireball Player Setup (Finalized)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("This will setup the Player with 3D models, Glossy Foam, and Camera-bound aiming.", MessageType.Info);
            
            if (GUILayout.Button("Create/Setup Player"))
            {
                SetupPlayer();
            }
        }

        private static void SetupPlayer()
        {
            // 1. Create/Find Player Root
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = new GameObject("Player");
                player.tag = "Player";
            }

            // 2. Add Core Components
            GetOrAddComponent<CharacterController>(player);
            PlayerInput input = GetOrAddComponent<PlayerInput>(player);
            PlayerController pc = GetOrAddComponent<PlayerController>(player);
            PlayerHealth health = GetOrAddComponent<PlayerHealth>(player);

            // 3. Setup Camera (Required for Aiming)
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.transform.SetParent(player.transform);
                camObj.transform.localPosition = new Vector3(0, 0.8f, 0);
                cam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }
            
            var pcSerialized = new SerializedObject(pc);
            pcSerialized.FindProperty("cameraTransform").objectReferenceValue = cam.transform;
            pcSerialized.ApplyModifiedProperties();

            // 4. Setup Input (Broadcast for weapons)
            InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            if (actions != null)
            {
                input.actions = actions;
                input.defaultControlScheme = "Keyboard&Mouse";
                input.notificationBehavior = PlayerNotifications.BroadcastMessages;
            }

            // 5. Setup Health & Stats
            PlayerStats stats = AssetDatabase.LoadAssetAtPath<PlayerStats>("Assets/ScriptableObjects/PlayerStats.asset");
            if (stats != null)
            {
                var healthSerialized = new SerializedObject(health);
                healthSerialized.FindProperty("playerStats").objectReferenceValue = stats;
                healthSerialized.ApplyModifiedProperties();
            }

            // 6. Setup Weapon Manager (Parented to Camera for Vertical Aiming)
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
            
            // 7. Setup Hands (Left = Champagne, Right = Moonshine)
            SetupHand(weaponManagerObj.transform, "LeftHand", typeof(ChampagneFlamethrower), "Assets/ScriptableObjects/ChampaineData.asset", stats);
            SetupHand(weaponManagerObj.transform, "RightHand", typeof(MoonshineMolotov), "Assets/ScriptableObjects/MoonshineData.asset", stats);

            // 8. Link Manager to Inventory
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
            Debug.Log("Player Setup Finalized with Glossy Mesh Foam!");
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

            // Visual Hand Placeholder (Capsule)
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

            // 3D Model Setup
            Transform bottleModel = hand.Find("BottleModel");
            if (bottleModel == null)
            {
                string modelPath = (weaponType == typeof(ChampagneFlamethrower)) 
                    ? "Assets/3d/Bottle of Champagne/Bottle_of_Champagne_01.obj" 
                    : "Assets/3d/Molotov/Molotov.fbx";

                GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (modelPrefab != null)
                {
                    GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
                    model.name = "BottleModel";
                    bottleModel = model.transform;
                    bottleModel.SetParent(hand);
                    
                    if (weaponType == typeof(ChampagneFlamethrower))
                    {
                        bottleModel.localPosition = new Vector3(0, 0.2f, 0.1f);
                        bottleModel.localRotation = Quaternion.Euler(-90, 0, 0);
                        bottleModel.localScale = Vector3.one * 0.5f;
                    }
                    else // Molotov
                    {
                        bottleModel.localPosition = new Vector3(0, 0.15f, 0.1f);
                        bottleModel.localRotation = Quaternion.Euler(0, 0, 0);
                        bottleModel.localScale = Vector3.one * 1.0f; // FBX might have different scale than OBJ
                        
                        // Apply Glossy Material to Molotov
                        var renderer = model.GetComponentInChildren<MeshRenderer>();
                        if (renderer != null)
                        {
                            Material glossMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/ChampagneGlossy.mat");
                            if (glossMat != null) renderer.material = glossMat;
                        }
                    }
                }
            }

            BottleWeapon weapon = (BottleWeapon)GetOrAddComponent(hand.gameObject, weaponType);
            WeaponData data = AssetDatabase.LoadAssetAtPath<WeaponData>(dataPath);
            
            var weaponSerialized = new SerializedObject(weapon);
            weaponSerialized.FindProperty("weaponData").objectReferenceValue = data;
            weaponSerialized.FindProperty("playerStats").objectReferenceValue = stats;

            // Champagne Specifics
            if (weaponType == typeof(ChampagneFlamethrower))
            {
                SetupChampagneVFX(hand, weaponSerialized);
            }

            // Moonshine Specifics
            if (weaponType == typeof(MoonshineMolotov))
            {
                SetupMoonshineVisuals(hand, weaponSerialized);
            }

            weaponSerialized.ApplyModifiedProperties();
        }

        private static void SetupMoonshineVisuals(Transform hand, SerializedObject weaponSerialized)
        {
            // Fire Point (at the neck)
            Transform firePoint = hand.Find("FirePoint");
            if (firePoint != null) 
            {
                Debug.Log("Skipping Moonshine FirePoint setup: already exists.");
                return; 
            }

            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(hand);
            firePoint.localPosition = new Vector3(0, 0.45f, 0.1f);
            weaponSerialized.FindProperty("firePoint").objectReferenceValue = firePoint;

            // Lit Wick Effect
            GameObject wickObj = new GameObject("LitWick");
            wickObj.transform.SetParent(firePoint);
            wickObj.transform.localPosition = Vector3.zero;
            ParticleSystem ps = wickObj.AddComponent<ParticleSystem>();
            
            var main = ps.main;
            main.startLifetime = 0.5f;
            main.startSpeed = 0.5f;
            main.startSize = 0.2f;
            main.gravityModifier = -0.2f;
            
            var emission = ps.emission;
            emission.rateOverTime = 15f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(Color.red, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLifetime.color = grad;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Particle.mat");
        }

        private static void SetupChampagneVFX(Transform hand, SerializedObject weaponSerialized)
        {
            ParticleSystem ps = hand.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                Debug.Log("Skipping Champagne VFX setup: ParticleSystem already exists. Keeping your manual changes!");
                return;
            }

            GameObject psObj = new GameObject("FoamParticles");
            psObj.transform.SetParent(hand);
            psObj.transform.localPosition = Vector3.zero;
            psObj.transform.localRotation = Quaternion.identity;
            ps = psObj.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.duration = 1.0f;
            main.loop = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.7f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(15f, 22f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
            main.gravityModifier = 0.8f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            
            var emission = ps.emission;
            emission.rateOverTime = 150f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 2.5f;
            shape.radius = 0.02f;

            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(Color.white, 0.0f), 
                    new GradientColorKey(new Color(0.95f, 0.95f, 0.8f), 0.5f), 
                    new GradientColorKey(Color.white, 1.0f) 
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(1.0f, 0.0f), 
                    new GradientAlphaKey(1.0f, 0.7f), 
                    new GradientAlphaKey(0.0f, 1.0f) 
                }
            );
            colorOverLifetime.color = gradient;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.1f);
            curve.AddKey(0.2f, 0.8f);
            curve.AddKey(1.0f, 1.2f);
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);

            var rot = ps.rotationOverLifetime;
            rot.enabled = true;
            rot.z = new ParticleSystem.MinMaxCurve(-180, 180);

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Mesh;
            renderer.mesh = AssetDatabase.GetBuiltinExtraResource<Mesh>("Sphere.fbx");
            
            // Material Creation
            string matPath = "Assets/Settings/ChampagneGlossy.mat";
            Material glossMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (glossMat == null)
            {
                Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
                if (litShader == null) litShader = Shader.Find("Standard");
                glossMat = new Material(litShader);
                glossMat.color = new Color(1f, 1f, 0.95f);
                if (litShader.name.Contains("Universal"))
                {
                    glossMat.SetFloat("_Smoothness", 0.95f);
                    glossMat.SetFloat("_Metallic", 0.0f);
                }
                else
                {
                    glossMat.SetFloat("_Glossiness", 0.95f);
                }
                AssetDatabase.CreateAsset(glossMat, matPath);
            }
            renderer.material = glossMat;
            renderer.minParticleSize = 0.001f;
            renderer.maxParticleSize = 1.0f;

            weaponSerialized.FindProperty("foamParticles").objectReferenceValue = ps;
            weaponSerialized.FindProperty("hitLayers").intValue = -1; // Everything
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
