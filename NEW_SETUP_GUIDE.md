# Fireball Project: Ultimate Level Design Setup Guide

Follow these instructions to get your player, weapons, and movement fully functional and ready for level design.

## 1. Player Foundation
1. **Create Player Object**: In your scene, create an Empty GameObject named `Player`.
2. **Add Components**:
    - **CharacterController**: Set `Slope Limit` to 45, `Step Offset` to 0.3.
    - **PlayerController**: Found in `Scripts/Player/`.
    - **PlayerInput**: 
        - Set `Actions` to `Assets/InputSystem_Actions.inputactions`.
        - Set `Behavior` to `Send Messages`.
    - **PlayerHealth**: Found in `Scripts/Player/`.
3. **Setup Camera**:
    - Create a child Camera named `MainCamera`.
    - Position it at eye level (e.g., Y = 0.8).
    - **Assign to PlayerController**: Drag the `MainCamera` transform into the `Camera Transform` field of the `PlayerController` component.
4. **Tagging**: Set the Player's tag to `Player`.

## 2. Weapon & Inventory Setup
1. **Weapon Manager**:
    - Create a child Empty GameObject named `WeaponManager`.
    - Attach `WeaponDualWieldManager`.
    - **Dependencies**: Drag the `Player` object into the `Player Controller` field.
2. **Creating Weapons**:
    - Create two child objects under `WeaponManager` named `Champagne` and `Moonshine`.
    - **Champagne**:
        - Attach `ChampagneFlamethrower`.
        - Assign `ChampaineData` ScriptableObject.
        - Create a child `ParticleSystem` for the foam. Drag it into the `Foam Particles` field.
    - **Moonshine**:
        - Attach `MoonshineMolotov`.
        - Assign `MoonshineData` ScriptableObject.
        - Create a child Empty named `FirePoint` at the bottle's mouth. Drag it into the `Fire Point` field.
3. **Register in Inventory**:
    - On the `WeaponDualWieldManager`, add both `Champagne` and `Moonshine` to the `All Weapons` list.
    - The manager will automatically handle activating the first two and switching the rest via **Mouse Wheel** or **1/2 keys**.

## 3. Visuals & Particles
- **Correct Placement**: Ensure your weapon models and particle systems are children of the `MainCamera` so they follow your view.
- **Foam Particles**: In the Particle System component, set `Simulation Space` to `World` so the foam stays in the air as you move.
- **Fermentation Effect**: Add a small "bubbles" particle system to the `Fermentation Effect` slot on each weapon to show when ammo is regenerating.

## 4. Controls Summary
- **Move**: `W/A/S/D`
- **Look**: Mouse (360° Horizontal, 90° Vertical)
- **Shoot Left**: `Left Mouse Button` (Champagne)
- **Shoot Right**: `Right Mouse Button` (Moonshine)
- **Switch Weapon**: `Mouse Wheel` or `1 / 2` keys (Cycles the right-hand weapon)
- **Jump**: `Space`
- **Sprint**: `Left Shift` (Automatic when out of ammo!)

---

## Future Context Prompt
*Copy and paste this into our chat if you have questions later:*

> "I am working on the Fireball project. The player uses a CharacterController with a PlayerController script. Weapons are managed by a WeaponDualWieldManager which handles switching and ammo. Champagne uses a continuous particle-based flamethrower logic, while Moonshine throws physics-based Molotov projectiles. Stats are stored in the PlayerStats ScriptableObject."
