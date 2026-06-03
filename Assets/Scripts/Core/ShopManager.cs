using UnityEngine;
using UnityEngine.UIElements;

namespace Fireball.Core
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;

        private VisualElement _shopPanel;
        private Label _goldLabel;
        private Button _fermentationUpgradeButton;
        private Button _armorUpgradeButton;
        private Button _closeButton;

        [Header("Shop Stats")]
        [SerializeField] private int fermentationUpgradeCost = 50;
        [SerializeField] private int armorUpgradeCost = 75;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _shopPanel = root.Q<VisualElement>("ShopPanel");
            _goldLabel = root.Q<Label>("GoldLabel");
            _fermentationUpgradeButton = root.Q<Button>("FermentationButton");
            _armorUpgradeButton = root.Q<Button>("ArmorButton");
            _closeButton = root.Q<Button>("CloseButton");

            if (_shopPanel != null) _shopPanel.style.display = DisplayStyle.None;
            UpdateUI();

            if (_fermentationUpgradeButton != null) _fermentationUpgradeButton.clicked += UpgradeFermentation;
            if (_armorUpgradeButton != null) _armorUpgradeButton.clicked += UpgradeArmor;
            if (_closeButton != null) _closeButton.clicked += CloseShop;
        }

        public void OpenShop()
        {
            if (_shopPanel != null) _shopPanel.style.display = DisplayStyle.Flex;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            UpdateUI();
        }

        public void CloseShop()
        {
            if (_shopPanel != null) _shopPanel.style.display = DisplayStyle.None;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

        private void UpgradeFermentation()
        {
            if (playerStats != null && playerStats.TrySpendGold(fermentationUpgradeCost))
            {
                playerStats.fermentationLevel++;
                playerStats.fermentationMultiplier += 0.2f; // +20% per level
                Debug.Log($"Upgraded Fermentation to level {playerStats.fermentationLevel}!");
                UpdateUI();
            }
        }

        private void UpgradeArmor()
        {
            if (playerStats != null && playerStats.TrySpendGold(armorUpgradeCost))
            {
                playerStats.armorUpgradeLevel++;
                playerStats.armorLevel += 1f; // Used as reduction factor in PlayerHealth
                Debug.Log($"Upgraded Armor to level {playerStats.armorUpgradeLevel}!");
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (_goldLabel != null && playerStats != null) 
                _goldLabel.text = $"Gold: {playerStats.gold}";
        }

        public void AddGold(int amount)
        {
            if (playerStats != null)
            {
                playerStats.AddGold(amount);
                UpdateUI();
            }
        }
    }
}
