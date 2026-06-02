# Fireball Project Setup Instructions

This project implements a first-person shmup with a two-bottle combat system. Below are instructions for setting up the components in the Unity Editor.

## 1. Scenes
Create the following scenes and add them to the Build Settings:
1. `MainMenu`
2. `LoadingScene`
3. `Level1`

## 2. Main Menu Setup
- Create a Canvas with three buttons: `PlayButton`, `SettingsButton`, `QuizButton`.
- Create two Panels: `SettingsPanel`, `QuizPanel`.
- Attach the `MainMenuUI` script to a manager object.
- Drag the buttons and panels into the `MainMenuUI` fields.

## 3. Loading Scene Setup
- Create a Canvas with a Slider (`ProgressBar`), TextMeshProUGUI (`TipText`), and Image (`Background`).
- Attach the `LoadingScreenManager` script to a manager object.
- Drag the UI elements into the fields. Set `Target Scene Name` to `Level1`.

## 4. Player Setup (Level1)
- Create a Player object with a `CharacterController`.
- Attach `PlayerController`, `PlayerHealth`, and `PlayerInput`.
- Set `PlayerInput` to use `InputSystem_Actions`.
- Create a child Camera and assign it to the `PlayerController`'s `CameraTransform`.
- Create a child object for the weapon manager and attach `WeaponDualWieldManager`.
- Tag the Player as `Player`.

## 5. Weapons Setup
- Create `WeaponData` ScriptableObjects for Champagne and Moonshine in `Assets/ScriptableObjects/`.
- Attach `ChampagneFlamethrower` and `MoonshineMolotov` to child objects of the player (hands).
- Assign the `WeaponData` to each weapon.
- **Champagne:** Requires a `ParticleSystem` for the foam.
- **Moonshine:** Requires a `MolotovProjectile` prefab.
- **Molotov Projectile:** Needs a Rigidbody, Collider, and the `MolotovProjectile` script.
- **Fire Zone:** Needs the `FireZone` script and a trigger collider.

## 6. Enemy Setup
- Create an enemy prefab with a `NavMeshAgent`.
- Attach `RivalMelee` or `RivalRanged`.
- Ensure enemies are on a layer specified in the `PlayerHealth`'s `Enemy Layer` mask for swarm detection.
- **Ranged Enemy:** Needs a projectile prefab with the `EnemyProjectile` script.

## 7. Shop Setup
- Create a trigger zone at the end of the level.
- Create a Shop Canvas with `ShopPanel`, `GoldText`, and buttons for upgrades.
- Attach `ShopManager` and link the UI elements.
- Trigger `OpenShop()` when the player enters the zone.
