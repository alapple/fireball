using UnityEngine;
using UnityEngine.UIElements;
using Fireball.ScriptableObjects;

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
            var uiDoc = GetComponent<UIDocument>();
            if (uiDoc == null || uiDoc.rootVisualElement == null)
            {
                Debug.LogWarning($"ShopManager on {gameObject.name} is missing a UIDocument or has no Root Element. UI will not initialize.");
                return;
            }

            var root = uiDoc.rootVisualElement;

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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OpenShop();
            }
        }

        public void OpenShop()
        {
            var uiDoc = GetComponent<UIDocument>();
            if (uiDoc == null || uiDoc.rootVisualElement == null)
            {
                Debug.LogError($"ShopManager: Cannot open shop! UIDocument or Root is missing on {gameObject.name}");
                Time.timeScale = 0f; // Still pause to show something is happening
                return;
            }

            // Re-fetch if null (sometimes UI Toolkit needs a second chance)
            if (_shopPanel == null)
            {
                Debug.Log("ShopManager: ShopPanel was null, attempting to re-fetch...");
                _shopPanel = uiDoc.rootVisualElement.Q<VisualElement>("ShopPanel");
                _goldLabel = uiDoc.rootVisualElement.Q<Label>("GoldLabel");
                _fermentationUpgradeButton = uiDoc.rootVisualElement.Q<Button>("FermentationButton");
                _armorUpgradeButton = uiDoc.rootVisualElement.Q<Button>("ArmorButton");
                _closeButton = uiDoc.rootVisualElement.Q<Button>("CloseButton");
                
                // Re-bind if we found them
                if (_fermentationUpgradeButton != null) _fermentationUpgradeButton.clicked += UpgradeFermentation;
                if (_armorUpgradeButton != null) _armorUpgradeButton.clicked += UpgradeArmor;
                if (_closeButton != null) _closeButton.clicked += CloseShop;
            }

            if (_shopPanel != null) 
            {
                _shopPanel.style.display = DisplayStyle.Flex;
                uiDoc.rootVisualElement.style.display = DisplayStyle.Flex;
                Debug.Log("ShopManager: ShopPanel set to Flex.");
            }
            else
            {
                Debug.LogError("ShopManager: FAILED to find 'ShopPanel' in the UXML! Check the name in UI Builder.");
            }
            
            Time.timeScale = 0f; // PAUSE THE GAME
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            UpdateUI();
        }

        public void CloseShop()
        {
            if (_shopPanel != null) 
            {
                _shopPanel.style.display = DisplayStyle.None;
            }
            
            Time.timeScale = 1f; // RESUME THE GAME
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
