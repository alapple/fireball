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

        public BottleWeapon LeftHandWeapon => leftHandWeapon;
        public BottleWeapon RightHandWeapon => rightHandWeapon;

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
            HandleContinuousInput();
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

        private void HandleContinuousInput()
        {
            // Direct polling of mouse buttons to prevent stuttering from the event system
            if (leftHandWeapon != null)
            {
                if (Mouse.current.leftButton.isPressed) leftHandWeapon.StartFire();
                else leftHandWeapon.StopFire();
            }

            if (rightHandWeapon != null)
            {
                if (Mouse.current.rightButton.isPressed) rightHandWeapon.StartFire();
                else rightHandWeapon.StopFire();
            }
        }

        // Input Handlers (Keep these for other inputs like cycling)
        public void OnAttack(InputValue value) { /* Handled in Update now */ }
        public void OnAttackSecondary(InputValue value) { /* Handled in Update now */ }


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
