using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Fireball.Core
{
    public class ShopManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private Button fermentationUpgradeButton;
        [SerializeField] private Button armorUpgradeButton;

        [Header("Shop Stats")]
        [SerializeField] private int fermentationUpgradeCost = 50;
        [SerializeField] private int armorUpgradeCost = 75;

        private int playerGold = 100; // Placeholder starting gold

        private void Start()
        {
            if (shopPanel != null) shopPanel.SetActive(false);
            UpdateUI();

            fermentationUpgradeButton.onClick.AddListener(UpgradeFermentation);
            armorUpgradeButton.onClick.AddListener(UpgradeArmor);
        }

        public void OpenShop()
        {
            shopPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpdateUI();
        }

        public void CloseShop()
        {
            shopPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void UpgradeFermentation()
        {
            if (playerGold >= fermentationUpgradeCost)
            {
                playerGold -= fermentationUpgradeCost;
                // Apply upgrade to player stats ScriptableObject (not implemented here yet)
                Debug.Log("Upgraded Fermentation!");
                UpdateUI();
            }
        }

        private void UpgradeArmor()
        {
            if (playerGold >= armorUpgradeCost)
            {
                playerGold -= armorUpgradeCost;
                // Apply upgrade to player health
                Debug.Log("Upgraded Armor!");
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (goldText != null) goldText.text = $"Gold: {playerGold}";
        }

        public void AddGold(int amount)
        {
            playerGold += amount;
            UpdateUI();
        }
    }
}
