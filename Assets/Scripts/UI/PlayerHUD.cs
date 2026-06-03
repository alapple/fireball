using UnityEngine;
using UnityEngine.UIElements;
using Fireball.ScriptableObjects;

namespace Fireball.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;

        private ProgressBar _healthBar;
        private Label _goldLabel;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _healthBar = root.Q<ProgressBar>("HealthBar");
            _goldLabel = root.Q<Label>("GoldDisplay");
            
            UpdateUI();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (playerStats == null) return;

            if (_healthBar != null)
            {
                _healthBar.value = playerStats.currentHealth;
                _healthBar.highValue = playerStats.maxHealth;
                _healthBar.title = $"HP: {Mathf.CeilToInt(playerStats.currentHealth)} / {playerStats.maxHealth}";
            }

            if (_goldLabel != null)
            {
                _goldLabel.text = $"$ {playerStats.gold}";
            }
        }
    }
}
