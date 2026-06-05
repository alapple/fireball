# 🍺 Fireball: The Ultimate "How to Build Your Game" Guide

This guide is for you, Jakob! Since you're just starting with Unity, I've explained every button and click. By the end of this, you'll have a player that moves, looks around in 360°, and shoots bottles.

---

## 🟢 Part 1: Opening your Scene
1. At the bottom of Unity, look at the **Project window** (where your folders are).
2. Go to `Assets` -> `Scenes`.
3. Double-click `Level1`. This is your playground.

---

## 🏃 Part 2: Setting up the Player (You!)
We need to create the character you control.

1. **Create the Body**: At the top left in the **Hierarchy**, right-click in the empty space and choose **Create Empty**. Name it `Player`.
2. **Add Physics**: With `Player` selected, look at the **Inspector** (the right side of your screen). Click the **Add Component** button and type `Character Controller`. This stops you from walking through walls.
3. **Add the Brain**: Click **Add Component** again and add `Player Controller`. 
4. **Add Input**: Click **Add Component** and add `Player Input`. 
    - In the `Actions` slot, click the tiny circle and select `InputSystem_Actions`.
    - Change `Behavior` to **Send Messages**.
5. **Add Health**: Click **Add Component** and add `Player Health`.

---

## 👀 Part 3: Fixing the Camera (Mouse Look)
If you can't look around, it's because the camera isn't "hooked up."

1. **Create the Eyes**: Right-click on your `Player` object in the **Hierarchy** and choose **Camera**. 
2. **Position it**: Move the camera up so it's at head-height (Change **Position Y** to `0.8` in the Inspector).
3. **Connect it**: Click on your `Player` object again. Look at the `Player Controller` component. Drag the **Camera** object from the Hierarchy into the empty **Camera Transform** box.
    - *Now, when you move your mouse, the camera will follow!*

---

## 🔫 Part 4: Setting up the Weapons (The Bottles)
This is the "Dual Wield" system.

1. **Create the Hands**: Right-click on your **Camera** (the one you just made) and choose **Create Empty**. Name it `WeaponManager`.
2. **Add the Manager**: Click **Add Component** on `WeaponManager` and add `Weapon Dual Wield Manager`.
    - Drag your `Player` object into the **Player Controller** box on this script.
3. **Create the Champagne**: Right-click on `WeaponManager` and choose **Create Empty**. Name it `Champagne`.
    - Add the component `Champagne Flamethrower`.
    - Go to `Assets/ScriptableObjects` in your folders. Drag `ChampaineData` into the **Weapon Data** box.
    - Drag `PlayerStats` into the **Player Stats** box.
4. **Create the Moonshine**: Right-click on `WeaponManager` and choose **Create Empty**. Name it `Moonshine`.
    - Add the component `Moonshine Molotov`.
    - Drag `MoonshineData` into the **Weapon Data** box.
    - Drag `PlayerStats` into the **Player Stats** box.
5. **Put them in your Pockets**: Click on `WeaponManager`. In the Inspector, look for the **All Weapons** list.
    - Click the `+` icon twice. 
    - Drag the `Champagne` object into the first slot.
    - Drag the `Moonshine` object into the second slot.

---

## ✨ Part 5: Fixing the Particles (Visual Effects)
If the foam doesn't show up at the bottle's mouth, follow this:

1. **For Champagne**: Right-click on your `Champagne` object and choose **Effects** -> **Particle System**. 
    - **Move it**: Use the move tool to place this particle system right where the "mouth" of the bottle would be.
    - **Connect it**: Click the `Champagne` object. Drag this **Particle System** into the **Foam Particles** box.
    - **Crucial Setting**: In the Particle System settings, find **Simulation Space** and change it to **World**. This makes the foam stay in the air instead of sticking to your hand.
2. **For Moonshine**: Right-click on `Moonshine` and choose **Create Empty**. Name it `FirePoint`.
    - Place it at the mouth of the bottle.
    - Click the `Moonshine` object and drag `FirePoint` into the **Fire Point** box.

---

## 👾 Part 6: Adding Enemies
1. Go to your folder: `Assets/Prefabs`.
2. Drag `RivalMelee` or `RivalRanged` directly into your **Scene View** (the big middle screen).
3. Make sure they are standing on the floor!

---

## 🎮 How to Play
- **W, A, S, D**: Move.
- **Mouse**: Look around (360°).
- **Left Click**: Spray Champagne.
- **Right Click**: Throw Moonshine.
- **Mouse Wheel**: Switch which bottle is in your right hand!
- **Space**: Jump.

---

## 🚩 If it doesn't work (Jakob's Troubleshooting)
- **"I can't see the mouse!"**: That's normal! The game locks the mouse so you can aim. Press `Esc` to get it back.
- **"The bottles are behind me!"**: Make sure the `WeaponManager` is a **child** of the `Camera`. If it's not a child, it won't move when you look up and down.
- **"I'm falling through the floor!"**: Make sure your floor has a **Box Collider** and your Player has a **Character Controller**.

---

### Future Context Prompt (Copy this if you need help later!)
> "I am Jakob, working on Fireball. I have set up the Player with a CharacterController and a Camera child. I am using the WeaponDualWieldManager on a child of the Camera. My weapons use WeaponData ScriptableObjects. I need help with [INSERT YOUR PROBLEM HERE]."
