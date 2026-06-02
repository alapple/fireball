# Project Blueprint & Agent Instructions

## 1. Project Overview
* **Engine:** Unity (Targeting Unity 6.4 architecture)
* **Genre:** First-Person Shmup / Dungeon Crawler (Dark Comedy)
* **Theme:** A desperate, chaotic protagonist fighting through ancient crypts to steal treasure from skeletons and elite rival adventuring parties using volatile alcoholic drinks as weapons.
* **Visual Style:** First-person 3D grid layout using modular primitives (Cubes/Placeholders). Heavy emphasis on fluid particle systems and physical object interactions.

---

## 2. Main Menu & UI Architecture
The agent must implement a UI framework with the following screens:

### Main Menu Screen
* **Play Button:** Triggers the loading sequence into Level 1.
* **Settings Button:** Opens a panel for basic configurations (Audio volume, Mouse Sensitivity).
* **Quiz Button:** Opens a placeholder overlay/panel styled contextually for mini-game interactions or trivia layout.

### Loading Screen
* A clean transition screen between the Main Menu and Level 1.
* Must include a UI Image component acting as a background layer, a loading progress bar, and a randomized gameplay tip text box.

---

## 3. Core Gameplay Systems (Two-Bottle Loop)
Implement a robust first-person controller with the following strict constraints:

* **Inventory Limit:** Strict maximum of **2 bottles** equipped at any time (Left Hand / Right Hand mapping). No weapon wheel.
* **Ammo & Fermentation Mechanic:** 
  * Drinks have finite ammunition capacities.
  * **No manual reload or pickups.** When not firing, bottles automatically regenerate their liquid capacity over time ("self-fermenting").
  * If **both** bottles hit 0% ammo simultaneously, the player cannot attack or melee—they must instantly sprint, weave, and utilize the environment to escape until the liquid recharges.
* **Health Balance (Forgiving but Swarm-Prone):** 
  * Single physical hits from rival weapons or skeleton swords deal low damage.
  * Enemies actively attempt to flank and corner the player. Getting body-blocked or swarmed causes rapid health decay.
* **The Weapon Archetypes:**
  1. *Weapon 1 (Sustained Spray):* High-pressure continuous foam stream ("Champagne Flamethrower") that pushes enemies back.
  2. *Weapon 2 (Lobbed AOE):* An arcing, physical projectile grenade ("Moonshine Molotov") that shatters on impact and leaves a temporary zone of fire.

---

## 4. Handcrafted Progression & Rival AI
Do not use procedural generation. Implement a strict room-by-room linear level architecture:

* **The In-Dungeon Shop:** Place a secure zone/room before the level exit. The player can spend collected gold coins to purchase stat upgrades (faster fermentation rate, thicker armor coat) before manually stepping through the door to the next level.
* **Grounded Rival AI:** Enemy adventurers must use standard, physical medieval weapons (swords, shields, bows, rapid crossbows). No magical spells. They scale in difficulty across floors by forming shield walls, firing tighter arrow spreads, and actively sweeping behind player cover.
