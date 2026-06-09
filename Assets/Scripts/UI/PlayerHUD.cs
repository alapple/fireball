using UnityEngine;
using UnityEngine.UIElements;
using Fireball.ScriptableObjects;
using Fireball.Weapons;

namespace Fireball.UI
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private WeaponDualWieldManager weaponManager;

        private ProgressBar _healthBar;
        private Label _goldLabel;
        private Label _leftAmmoLabel;
        private Label _rightAmmoLabel;
        private VisualElement _leftAmmoBar;
        private VisualElement _rightAmmoBar;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _healthBar = root.Q<ProgressBar>("HealthBar");
            _goldLabel = root.Q<Label>("GoldDisplay");
            _leftAmmoLabel = root.Q<Label>("LeftAmmoLabel");
            _rightAmmoLabel = root.Q<Label>("RightAmmoLabel");
            _leftAmmoBar = root.Q<VisualElement>("LeftAmmoBarInner");
            _rightAmmoBar = root.Q<VisualElement>("RightAmmoBarInner");
            
            if (weaponManager == null)
            {
                weaponManager = FindObjectOfType<WeaponDualWieldManager>();
            }

            UpdateUI();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (playerStats == null) return;

            // Health
            if (_healthBar != null)
            {
                _healthBar.value = playerStats.currentHealth;
                _healthBar.highValue = playerStats.maxHealth;
                _healthBar.title = $"HP: {Mathf.CeilToInt(playerStats.currentHealth)} / {playerStats.maxHealth}";
            }

            // Gold
            if (_goldLabel != null)
            {
                _goldLabel.text = $"$ {playerStats.gold}";
            }

            // Ammo
            if (weaponManager != null)
            {
                UpdateWeaponUI(weaponManager.LeftHandWeapon, _leftAmmoLabel, _leftAmmoBar);
                UpdateWeaponUI(weaponManager.RightHandWeapon, _rightAmmoLabel, _rightAmmoBar);
            }
        }

        private void UpdateWeaponUI(BottleWeapon weapon, Label label, VisualElement barInner)
        {
            if (weapon == null)
            {
                if (label != null) label.text = "-";
                if (barInner != null) barInner.style.width = Length.Percent(0);
                return;
            }

            if (label != null)
            {
                label.text = $"{Mathf.CeilToInt(weapon.CurrentAmmo)}";
            }

            if (barInner != null)
            {
                float percent = (weapon.CurrentAmmo / weapon.MaxAmmo) * 100f;
                barInner.style.width = Length.Percent(percent);
                
                // Change color if empty
                barInner.style.backgroundColor = weapon.IsEmpty ? Color.red : new Color(0.2f, 0.8f, 1f, 0.8f);
            }
        }
    }
}
