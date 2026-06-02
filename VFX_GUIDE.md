# Unity VFX Configuration Guide

This guide provides the exact Particle System settings needed to create the weapon effects for the "Two-Bottle Loop". 

---

## 1. Champagne Foam Stream
**Target Object:** Child of `LeftHandBottle` (Front).
- **Duration:** 1.00 (Looping)
- **Start Lifetime:** 0.5 - 0.8 (Random between two constants)
- **Start Speed:** 10 - 15
- **Start Size:** 0.2 - 0.5
- **Gravity Modifier:** 0.5 (To make it arc downward)
- **Simulation Space:** World
- **Emission:** Rate over Time = 100
- **Shape:** Cone (Angle: 5, Radius: 0.1)
- **Color over Lifetime:** Gradient from White to Transparent.
- **Size over Lifetime:** Increasing curve.

## 2. Molotov Explosion (Burst)
**Target Object:** `MolotovExplosionPrefab`.
- **Duration:** 1.00 (Not Looping, Play on Awake)
- **Start Lifetime:** 0.5 - 1.0
- **Start Speed:** 5 - 10
- **Start Size:** 0.5 - 1.2
- **Gravity Modifier:** -0.1 (Slight upward drift)
- **Simulation Space:** World
- **Emission:** Bursts (Time: 0, Count: 30)
- **Shape:** Sphere (Radius: 0.2)
- **Color over Lifetime:** Gradient from Bright Orange -> Red -> Dark Grey (Smoke).
- **Size over Lifetime:** Decreasing curve.

## 3. Ground Fire Zone (Lingering)
**Target Object:** `FireZonePrefab`.
- **Duration:** 5.00 (Looping, play for the duration of the zone)
- **Start Lifetime:** 1.0 - 1.5
- **Start Speed:** 1 - 3
- **Start Size:** 0.5 - 1.0
- **Gravity Modifier:** -0.2 (Heat rising)
- **Emission:** Rate over Time = 20
- **Shape:** Circle (Radius: 2.0, Mode: Random)
- **Color over Lifetime:** Yellow -> Orange -> Transparent.
- **Noise Module:** Enabled (Strength: 0.5, Frequency: 1.0) for flickering effect.

## 4. Fermentation Bubbles (Recharge)
**Target Object:** Child of `Bottle` (Top/Mouth).
- **Start Lifetime:** 1.0 - 2.0
- **Start Speed:** 0.5 - 1.0
- **Start Size:** 0.05 - 0.15
- **Simulation Space:** Local
- **Emission:** Rate over Time = 5
- **Shape:** Box (Scale: 0.1, 0.1, 0.1)
- **Color over Lifetime:** Light Green (Toxic/Alcohol) or Pale Gold.
- **Noise Module:** Enabled (Strength: 1.0) to make bubbles wiggle upward.

## 5. Empty Bottle Smoke (Puff)
**Target Object:** Child of `Bottle` (Top/Mouth).
- **Duration:** 0.5
- **Looping:** False
- **Start Lifetime:** 0.8
- **Start Speed:** 2.0
- **Start Size:** 0.3
- **Gravity Modifier:** -0.5 (Rising fast)
- **Simulation Space:** World
- **Emission:** Bursts (Time: 0, Count: 10)
- **Shape:** Cone (Angle: 10)
- **Color over Lifetime:** Light Grey -> Transparent.
- **Size over Lifetime:** Increasing curve.
