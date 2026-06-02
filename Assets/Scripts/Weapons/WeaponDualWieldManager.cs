using UnityEngine;
using Fireball.Player;

namespace Fireball.Weapons
{
    public class WeaponDualWieldManager : MonoBehaviour
    {
        [Header("Weapons")]
        [SerializeField] private BottleWeapon leftHandWeapon;
        [SerializeField] private BottleWeapon rightHandWeapon;

        [Header("Dependencies")]
        [SerializeField] private PlayerController playerController;

        private void Update()
        {
            CheckAmmoStatus();
        }

        private void CheckAmmoStatus()
        {
            bool bothEmpty = leftHandWeapon.IsEmpty && rightHandWeapon.IsEmpty;
            
            if (playerController != null)
            {
                playerController.SetForcedSprint(bothEmpty);
            }

            // Optional: Visually show that weapons are empty/disabled
        }

        // Input Handlers (called by PlayerInput component)
        public void OnAttack(UnityEngine.InputSystem.InputValue value)
        {
            if (value.isPressed) leftHandWeapon.StartFire();
            else leftHandWeapon.StopFire();
        }

        public void OnAttackSecondary(UnityEngine.InputSystem.InputValue value)
        {
            if (value.isPressed) rightHandWeapon.StartFire();
            else rightHandWeapon.StopFire();
        }
    }
}
