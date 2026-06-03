using UnityEngine;
using UnityEngine.UIElements;

namespace Fireball.Core
{
    public class ShopManager : MonoBehaviour
    {
        private VisualElement _shopPanel;
        private Label _goldLabel;
        private Button _fermentationUpgradeButton;
        private Button _armorUpgradeButton;

        [Header("Shop Stats")]
        [SerializeField] private int fermentationUpgradeCost = 50;
        [SerializeField] private int armorUpgradeCost = 75;

        private int _playerGold = 100; // Placeholder starting gold

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _shopPanel = root.Q<VisualElement>("ShopPanel");
            _goldLabel = root.Q<Label>("GoldLabel");
            _fermentationUpgradeButton = root.Q<Button>("FermentationButton");
            _armorUpgradeButton = root.Q<Button>("ArmorButton");

            if (_shopPanel != null) _shopPanel.style.display = DisplayStyle.None;
            UpdateUI();

            if (_fermentationUpgradeButton != null) _fermentationUpgradeButton.clicked += UpgradeFermentation;
            if (_armorUpgradeButton != null) _armorUpgradeButton.clicked += UpgradeArmor;
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
            if (_playerGold >= fermentationUpgradeCost)
            {
                _playerGold -= fermentationUpgradeCost;
                Debug.Log("Upgraded Fermentation!");
                UpdateUI();
            }
        }

        private void UpgradeArmor()
        {
            if (_playerGold >= armorUpgradeCost)
            {
                _playerGold -= armorUpgradeCost;
                Debug.Log("Upgraded Armor!");
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (_goldLabel != null) _goldLabel.text = $"Gold: {_playerGold}";
        }

        public void AddGold(int amount)
        {
            _playerGold += amount;
            UpdateUI();
        }
    }
}
