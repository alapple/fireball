# Unity VFX Configuration Guide (Detailed)

This guide provides step-by-step instructions for creating the weapon effects. If you have never used the **Particle System** in Unity, follow these steps exactly.

---

## 0. Common Setup (How to create a VFX)
1.  **Create Object:** Right-click in Hierarchy -> **Effects** -> **Particle System**.
2.  **Naming:** Name it according to the effect (e.g., `ChampagneFoam`).
3.  **Inspector:** All settings below are found in the **Particle System** component in the Inspector.
4.  **Modules:** Click the checkbox next to a module name (e.g., "Color over Lifetime") to enable it.

---

## 1. Champagne Foam Stream
**Purpose:** High-pressure foam spray from the bottle.
- **Main Module:**
    - **Duration:** 1.00 (Looping: ON)
    - **Start Lifetime:** Random Between Two Constants: `0.5` and `0.8`
    - **Start Speed:** Random Between Two Constants: `12` and `18`
    - **Start Size:** `0.3`
    - **Gravity Modifier:** `0.6` (Ensures the foam arcs toward the ground)
    - **Simulation Space:** `World` (Foam stays in the air when you move)
- **Emission Module:**
    - **Rate over Time:** `120`
- **Shape Module:**
    - **Shape:** `Cone`
    - **Angle:** `3` (Very tight stream)
    - **Radius:** `0.05`
- **Color over Lifetime:**
    - Enable it. Click the gradient. Set the **Alpha** (top markers) at the end to `0` (Fade out). Color should be pure white.
- **Size over Lifetime:**
    - Enable it. Click the curve. Use a curve that starts small (`0.2`) and grows to full size (`1.0`) to simulate expanding foam.
- **Renderer Module:**
    - **Render Mode:** `Billboard`
    - **Material:** Use `Default-Particle` or a white blob texture.

---

## 2. Molotov Explosion (Burst)
**Purpose:** The instantaneous impact effect of the Moonshine bottle.
- **Main Module:**
    - **Duration:** 1.00 (Looping: OFF, Play on Awake: ON)
    - **Start Lifetime:** Random Between Two Constants: `0.4` and `0.7`
    - **Start Speed:** Random Between Two Constants: `5` and `12`
    - **Start Size:** `0.8`
    - **Gravity Modifier:** `-0.2` (Smoke/Heat drifts up)
- **Emission Module:**
    - **Rate over Time:** `0`
    - **Bursts:** Add one. `Time: 0`, `Count: 40`.
- **Shape Module:**
    - **Shape:** `Sphere`
    - **Radius:** `0.3`
- **Color over Lifetime:**
    - Gradient: `Bright Orange (#FF8000)` at 0% -> `Dark Red (#800000)` at 50% -> `Dark Grey (#303030)` at 100%. Fade alpha to 0 at the very end.
- **Size over Lifetime:**
    - Curve: Starts at `1.0` and shrinks to `0.0`.
- **Renderer Module:**
    - **Material:** `Default-Particle`.

---

## 3. Ground Fire Zone (Lingering)
**Purpose:** The fire that stays on the floor after the explosion.
- **Main Module:**
    - **Duration:** 5.00 (Looping: ON)
    - **Start Lifetime:** `1.2`
    - **Start Speed:** `2.0`
    - **Start Size:** `1.0`
    - **Gravity Modifier:** `-0.5` (Heat rises fast)
- **Emission Module:**
    - **Rate over Time:** `25`
- **Shape Module:**
    - **Shape:** `Circle`
    - **Radius:** `2.0` (Matches the gameplay fire zone size)
    - **Radius Thickness:** `0` (Particles spawn only on the edge) or `1` (Everywhere in circle). Set to `1`.
- **Color over Lifetime:**
    - Gradient: `Yellow` -> `Orange` -> `Transparent`.
- **Noise Module:**
    - **Strength:** `0.8`
    - **Frequency:** `1.5`
    - **Scroll Speed:** `1.0` (Makes the flames flicker and "dance").
- **Renderer Module:**
    - **Render Mode:** `Stretched Billboard` (Length Scale: `2`) to make flames look taller.

---

## 4. Fermentation Bubbles (Passive)
**Purpose:** Visual indicator that the bottle is "recharging".
- **Main Module:**
    - **Start Lifetime:** `1.5`
    - **Start Speed:** `0.8`
    - **Start Size:** `0.1`
    - **Simulation Space:** `Local` (Bubbles move with the bottle)
- **Emission Module:**
    - **Rate over Time:** `8`
- **Shape Module:**
    - **Shape:** `Box`
    - **Scale:** `0.2, 0.2, 0.2`
- **Color over Lifetime:**
    - Gradient: `Pale Gold (#E6BE8A)` with `50% Alpha`.
- **Noise Module:**
    - **Strength:** `1.0` (Makes them wiggle as they rise).

---

## 5. Empty Bottle Smoke (Puff)
**Purpose:** Signal that you just ran out of ammo.
- **Main Module:**
    - **Duration:** 0.5 (Looping: OFF)
    - **Start Lifetime:** `1.0`
    - **Start Speed:** `3.0`
    - **Start Size:** `0.4`
    - **Gravity Modifier:** `-0.8` (Rises very fast)
- **Emission Module:**
    - **Bursts:** `Time: 0`, `Count: 15`.
- **Shape Module:**
    - **Shape:** `Cone`, `Angle: 15`.
- **Color over Lifetime:**
    - `Light Grey` -> `Transparent`.
- **Size over Lifetime:**
    - Curve: Starts at `0.1` and grows to `1.0`.
