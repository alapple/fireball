using UnityEngine;
using UnityEngine.InputSystem;
using Fireball.Player;
using System.Collections.Generic;

namespace Fireball.Weapons
{
    public class WeaponDualWieldManager : MonoBehaviour
    {
        [Header("Weapon Slots")]
        [SerializeField] private BottleWeapon leftHandWeapon;
        [SerializeField] private BottleWeapon rightHandWeapon;

        [Header("Inventory")]
        [SerializeField] private List<BottleWeapon> allWeapons = new List<BottleWeapon>();
        
        [Header("Dependencies")]
        [SerializeField] private PlayerController playerController;

        private int currentLeftIndex = 0;
        private int currentRightIndex = 1;

        private void Start()
        {
            InitializeWeapons();
        }

        private void InitializeWeapons()
        {
            // Deactivate all first
            foreach (var weapon in allWeapons)
            {
                weapon.gameObject.SetActive(false);
            }

            // Activate initial set
            if (allWeapons.Count > 0)
            {
                leftHandWeapon = allWeapons[0];
                leftHandWeapon.gameObject.SetActive(true);
            }
            if (allWeapons.Count > 1)
            {
                rightHandWeapon = allWeapons[1];
                rightHandWeapon.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            CheckAmmoStatus();
        }

        private void CheckAmmoStatus()
        {
            bool leftEmpty = leftHandWeapon != null && leftHandWeapon.IsEmpty;
            bool rightEmpty = rightHandWeapon != null && rightHandWeapon.IsEmpty;
            
            if (playerController != null)
            {
                playerController.SetForcedSprint(leftEmpty && rightEmpty);
            }
        }

        // Input Handlers
        public void OnAttack(InputValue value)
        {
            if (leftHandWeapon == null) return;
            if (value.isPressed) leftHandWeapon.StartFire();
            else leftHandWeapon.StopFire();
        }

        public void OnAttackSecondary(InputValue value)
        {
            if (rightHandWeapon == null) return;
            if (value.isPressed) rightHandWeapon.StartFire();
            else rightHandWeapon.StopFire();
        }

        public void OnNext(InputValue value)
        {
            if (value.isPressed) CycleWeapons(1);
        }

        public void OnPrevious(InputValue value)
        {
            if (value.isPressed) CycleWeapons(-1);
        }

        public void OnScrollWheel(InputValue value)
        {
            float scroll = value.Get<Vector2>().y;
            if (scroll > 0) CycleWeapons(1);
            else if (scroll < 0) CycleWeapons(-1);
        }

        private void CycleWeapons(int direction)
        {
            if (allWeapons.Count <= 2) return;

            // In a dual wield system, we might want to cycle the "active" pair or just one hand.
            // For simplicity, let's swap the right hand weapon with the next available in inventory.
            
            rightHandWeapon.StopFire();
            rightHandWeapon.gameObject.SetActive(false);

            currentRightIndex = (currentRightIndex + direction + allWeapons.Count) % allWeapons.Count;
            
            // Ensure we don't pick the same weapon as left hand
            if (currentRightIndex == currentLeftIndex)
            {
                currentRightIndex = (currentRightIndex + direction + allWeapons.Count) % allWeapons.Count;
            }

            rightHandWeapon = allWeapons[currentRightIndex];
            rightHandWeapon.gameObject.SetActive(true);
        }
    }
}
