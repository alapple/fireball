# Unity Editor Setup Instructions (Handover)

This implementation plan is for a human developer or an AI agent with Unity Editor access to assemble the scripts created by the CLI agent into a functional game.

## Prerequisites
- Open the project in Unity (Unity 6.4 architecture).
- Ensure the **Input System** package is installed and active.
- Ensure the **UI Toolkit** package is installed (standard in Unity 6).
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

## 3. UI Implementation (UI Toolkit) - Detailed Steps
This project uses **UI Toolkit**. Instead of a Canvas, we use `UIDocument` components and `.uxml` files created in the **UI Builder**.

### A. General Workflow for all UI Screens:
1.  **Create UXML:** Right-click in `Assets/UI Toolkit/` -> **Create** -> **UI Toolkit** -> **UI Document**. Name it (e.g., `MainMenu.uxml`).
2.  **Open UI Builder:** Double-click the `.uxml` file.
3.  **Hierarchy & Naming:** 
    *   Drag elements (Buttons, Labels, etc.) from the **Library** to the **Hierarchy**.
    *   **CRITICAL:** Select each element and set its **Name** in the Inspector (top field). This name must match exactly what the script looks for (case-sensitive). Do *not* confuse this with the "Text" or "Label" property.
4.  **Scene Setup (WHERE TO ATTACH THE SCRIPTS):**
    *   In your Unity Scene, create a new Empty GameObject (Right-click in Hierarchy -> **Create Empty**).
    *   **Main Menu Scene:** Name the object `MainMenu_Manager`. Add a **UIDocument** AND the `MainMenuUI.cs` script to it.
    *   **Loading Scene:** Name the object `Loading_Manager`. Add a **UIDocument** AND the `LoadingScreenManager.cs` script to it.
    *   **Level1 Scene (for Shop):** Name the object `Shop_UI`. Add a **UIDocument** AND the `ShopManager.cs` script to it.
    *   **Crucial:** The C# script **MUST** be on the same GameObject as the **UIDocument** component, because the scripts use `GetComponent<UIDocument>()` to find the UI.

### B. Specific Screen Requirements:

#### Main Menu (`MainMenu.uxml`)
*   **VisualElement** named `SettingsPanel`: Set `Usage` -> `Display` to `Hidden` by default in UI Builder.
*   **VisualElement** named `QuizPanel`: Set `Usage` -> `Display` to `Hidden`.
*   **Button** named `PlayButton`.
*   **Button** named `SettingsButton`.
*   **Button** named `QuizButton`.
*   *Note: The `MainMenuUI.cs` script handles the logic of showing/hiding these panels.*

#### Loading Screen (`LoadingScreen.uxml`)
*   **ProgressBar** named `ProgressBar`.
*   **Label** named `TipLabel`.
*   *Note: In `LoadingScreenManager.cs`, the ProgressBar is updated from 0 to 100.*

#### Shop UI (`ShopUI.uxml`)
*   **VisualElement** named `ShopPanel`.
*   **Label** named `GoldLabel`.
*   **Button** named `FermentationButton`.
*   **Button** named `ArmorButton`.
*   *Note: Set the `ShopPanel` to `Display: Hidden` initially; the `ShopManager.cs` script will show it when the player enters the trigger.*

## 4. Player Assembly
- Create a Capsule in `Level1` named `Player`.
- **Add Native Components:** Click **Add Component** in the Inspector and search for:
    - **CharacterController** (Native Unity component for movement).
    - **Player Input** (From the Input System package).
- **Add Project Scripts:** Attach the following scripts from the `Assets/Scripts/Player/` folder:
    - `PlayerController.cs`
    - `PlayerHealth.cs`
- **Configuration:**
    - Set `PlayerInput` behavior to **Invoke Unity Events** or **Send Messages**.
    - Create a child Camera. Link it to `PlayerController`'s `CameraTransform`.
    - Create a child `WeaponManager` object and attach `WeaponDualWieldManager.cs`.
    - Create two children for weapons: `LeftHand` and `RightHand`.
        - `LeftHand`: Attach `ChampagneFlamethrower.cs`. Link `ChampagneData`.
        - `RightHand`: Attach `MoonshineMolotov.cs`. Link `MoonshineData`.

## 5. Enemy & AI
- Bake a **NavMesh** for `Level1` (Window -> AI -> Navigation).
- Create a GameObject named `SquadManager` and attach `AISquadManager.cs`.
- Create an Enemy prefab (e.g., a Cube or Capsule).
- **Add Native Component:** Click **Add Component** and add a **NavMeshAgent**.
- **Add Project Scripts:** Attach `RivalMelee.cs` or `RivalRanged.cs`.
- Ensure the Player has the `Player` tag so enemies can find them.
- Set the enemy layer to a specific layer and update the `PlayerHealth`'s `EnemyLayer` mask.
- **Melee Tactics:** Enemies will automatically form Shield Walls (damage reduction from front) or Flank the player based on the SquadManager's logic.
- **Ranged Tactics:** Enemies will now strafe (move side-to-side) while shooting to dodge attacks.

## 6. Visual Effects (VFX) Placement
Follow the settings in **`VFX_GUIDE.md`**, then place/save them as follows:

### A. Attached to Player (Child Objects)
These must be children of the weapon objects so they move with the player:
*   **Champagne Foam:** Create as a child of the `LeftHand` object. Position it at the "mouth" of the bottle. Link it to the `Foam System` field in the `ChampagneFlamethrower` script.
*   **Fermentation Bubbles:** Create as a child of **both** `LeftHand` and `RightHand`. Link to the `Recharge Particles` field in the weapon scripts.
*   **Empty Smoke:** Create as a child of **both** `LeftHand` and `RightHand`. Link to the `Empty Puff` field.

### B. VFX Prefabs (Instantiated)
These effects are created as prefabs and spawned during gameplay via script:
*   **Molotov Projectile:**
    *   Create a Sphere. Add **Rigidbody** and a **SphereCollider** (Ensure "Is Trigger" is **OFF** so it can collide with the floor).
    *   Attach `MolotovProjectile.cs`.
    *   Drag this object from Hierarchy into your `Assets/Prefabs/` folder to make it a prefab.
*   **Explosion Effect:**
    *   Create the particle system as described in `VFX_GUIDE.md` (Section 2).
    *   Make sure "Looping" is **OFF** and "Stop Action" is set to **Destroy**.
    *   Save as a prefab.
*   **Fire Zone Prefab:**
    *   Create the particle system as described in `VFX_GUIDE.md` (Section 3).
    *   Attach `FireZone.cs`.
    *   Save as a prefab.

### C. How to "Link" them in the Inspector
Once you have created the Particle System GameObjects and Prefabs:

1.  **Weapon Scripts (Hierarchy):**
    *   Select `LeftHand` (Champagne). Link `Foam Particles`, `Recharge Particles`, and `Empty Puff`.
    *   Select `RightHand` (Moonshine). Link `Recharge Particles` and `Empty Puff`.
2.  **ScriptableObjects (Project Folder):**
    *   Select `MoonshineData`. Drag the **Molotov Projectile Prefab** into the `Projectile Prefab` slot.
3.  **Projectile Prefab (Project Folder):**
    *   Select the **Molotov Projectile Prefab**.
    *   In the `MolotovProjectile` script slot, drag the **Fire Zone Prefab** and **Explosion Effect Prefab** into their respective slots.

## 7. Shop Interaction
*   **The Trigger:** Create a Cube at the end of the level, set its Collider to **Is Trigger**.
*   **The UI:** Follow the "Shop UI" steps in Section 3 using `ShopUI.uxml` and `ShopManager.cs`.
*   **The Bridge:** Create a small script (e.g., `ShopTrigger.cs`) that has a reference to the `ShopManager` and calls `shopManager.OpenShop()` in `OnTriggerEnter`.
