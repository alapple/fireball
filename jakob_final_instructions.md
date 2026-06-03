# Instructions for Jakob

I have implemented the core logic for the stat system, shop, and HUD. Here is what you need to do in the Unity Editor to get everything working:

## 1. Create the PlayerStats Asset
1. Right-click in `Assets/ScriptableObjects/` -> **Create** -> **Fireball** -> **Player Stats**.
2. Name it `PlayerStats`.
3. This asset will store gold, health, and upgrade levels during gameplay.

## 2. Configure the Player
1. Select your **Player** object.
2. In the `PlayerHealth` component, assign the `PlayerStats` asset you just created to the **Player Stats** field.
3. Select the **Champagne** and **Moonshine** weapon objects (children of the player).
4. Assign the same `PlayerStats` asset to their **Player Stats** fields.

## 3. Setup the HUD
1. In `Level1`, create a new **UIDocument** object.
2. Assign `Assets/Scripts/UI/PlayerHUD.uxml` to the **Visual Tree Asset** field.
3. Attach the `PlayerHUD` script to this object.
4. Assign the `PlayerStats` asset to the **Player Stats** field of the `PlayerHUD` script.

## 4. Setup the Shop (Level1)
1. Ensure there is a **Shop** object with a `UIDocument`.
2. Assign `Assets/Scripts/UI/ShopUI.uxml` to its **Visual Tree Asset**.
3. In the `ShopManager` component, assign the `PlayerStats` asset.
4. Setup a **Trigger Collider** at the end of the level.
5. Create a simple script or use a Unity Event to call `ShopManager.OpenShop()` when the player enters the trigger.

## 5. Main Menu
1. In the `MainMenu` scene, select the `MainMenuUI` object.
2. Assign the `PlayerStats` asset to its **Player Stats** field. This ensures stats are reset when a new game starts.

## 6. Testing
- Enemies now have a `Gold Value`. When they die, they will add gold to the `ShopManager` (which updates the `PlayerStats`).
- Open the Shop to buy upgrades. 
- **Fermentation** increases ammo regeneration speed.
- **Armor** reduces incoming damage.

Happy level designing!
