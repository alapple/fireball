# Unity Editor Setup Instructions (Handover)

This implementation plan is for a human developer or an AI agent with Unity Editor access to assemble the scripts created by the CLI agent into a functional game.

## Prerequisites
- Open the project in Unity (Unity 6.4 architecture).
- Ensure the **Input System** package is installed and active.
- Ensure **TextMeshPro** is imported.
- Ensure **NavMesh Navigation** package is installed.

## 1. Scene Setup
Create three new scenes in `Assets/Scenes/`:
- `MainMenu`: The entry point.
- `LoadingScene`: The transition screen.
- `Level1`: The primary gameplay floor.
Add all three to **Build Settings** -> **Scenes in Build**.

## 2. Global ScriptableObjects
Create an `Assets/ScriptableObjects` folder.
- Right-click -> **Create** -> **Fireball** -> **Weapon Data**.
- Name one `ChampagneData` (Stats: high fermentation, low damage, knockback).
- Name one `MoonshineData` (Stats: low ammo, high fire damage, projectile prefab).

## 3. UI Implementation
### Main Menu
- Create a **Canvas**.
- Add buttons: `Play`, `Settings`, `Quiz`.
- Add GameObjects/Panels: `SettingsPanel`, `QuizPanel`.
- Attach `MainMenuUI.cs` to a `MenuManager` object.
- Link all button/panel references in the inspector.
### Loading Screen
- Create a **Canvas**.
- Add a **Slider** (`ProgressBar`) and a **TextMeshPro - Text** (`TipText`).
- Attach `LoadingScreenManager.cs` to a `LoadingManager` object.
- Link references. Set `Target Scene Name` to `Level1`.

## 4. Player Assembly
- Create a Capsule in `Level1` named `Player`.
- Attach a `CharacterController`.
- Attach `PlayerController.cs`, `PlayerHealth.cs`, and `PlayerInput`.
- Set `PlayerInput` behavior to **Invoke Unity Events** or **Send Messages**.
- Create a child Camera. Link it to `PlayerController`'s `CameraTransform`.
- Create a child `WeaponManager` object and attach `WeaponDualWieldManager.cs`.
- Create two children for weapons: `LeftHand` and `RightHand`.
    - `LeftHand`: Attach `ChampagneFlamethrower.cs`. Link `ChampagneData`.
    - `RightHand`: Attach `MoonshineMolotov.cs`. Link `MoonshineData`.

## 5. Enemy & AI
- Bake a **NavMesh** for `Level1`.
- Create a GameObject named `SquadManager` and attach `AISquadManager.cs`. This script acts as the central brain for coordinating groups.
- Create an Enemy prefab with a `NavMeshAgent`.
- Attach `RivalMelee.cs` or `RivalRanged.cs`.
- Ensure the Player has the `Player` tag so enemies can find them.
- Set the enemy layer to a specific layer and update the `PlayerHealth`'s `EnemyLayer` mask.
- **Melee Tactics:** Enemies will automatically form Shield Walls (damage reduction from front) or Flank the player based on the SquadManager's logic.
- **Ranged Tactics:** Enemies will now strafe (move side-to-side) while shooting to dodge attacks.

## 6. Visual Effects (VFX)
For detailed instructions on setting up particle systems for the foam, explosions, and fermentation effects, please refer to the **`VFX_GUIDE.md`** file. It contains the exact values for the Unity Particle System modules.

## 7. Shop Interaction
- Create a Cube at the end of the level, set it to **Is Trigger**.
- Create a **Shop Canvas** with `ShopPanel` and upgrade buttons.
- Attach `ShopManager.cs` to the canvas.
- Create a simple script to call `shopManager.OpenShop()` when the player enters the trigger.
