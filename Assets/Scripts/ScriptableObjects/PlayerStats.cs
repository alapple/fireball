using UnityEngine;

namespace Fireball.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Fireball/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Currency")]
        public int gold = 0;

        [Header("Health")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float armorLevel = 0f; // 0 to 1, reduction factor

        [Header("Upgrades")]
        public float fermentationMultiplier = 1.0f;
        public int fermentationLevel = 0;
        public int armorUpgradeLevel = 0;

        public void ResetStats()
        {
            gold = 0;
            maxHealth = 100f;
            currentHealth = maxHealth;
            armorLevel = 0f;
            fermentationMultiplier = 1.0f;
            fermentationLevel = 0;
            armorUpgradeLevel = 0;
        }

        public void AddGold(int amount)
        {
            gold += amount;
        }

        public bool TrySpendGold(int amount)
        {
            if (gold >= amount)
            {
                gold -= amount;
                return true;
            }
            return false;
        }
    }
}
