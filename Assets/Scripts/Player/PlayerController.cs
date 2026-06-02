using UnityEngine;
using UnityEngine.InputSystem;

namespace Fireball.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 9f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -19.62f;

        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 0.1f;
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;
        private float xRotation = 0f;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool sprintHeld;
        private bool isForcedSprint;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
        public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
        public void OnSprint(InputValue value) => sprintHeld = value.isPressed;

        public void SetForcedSprint(bool forced) => isForcedSprint = forced;

        private void Update()
        {
            HandleRotation();
            HandleMovement();
        }

        private void HandleRotation()
        {
            float mouseX = lookInput.x * mouseSensitivity;
            float mouseY = lookInput.y * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleMovement()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float currentSpeed = (sprintHeld || isForcedSprint) ? sprintSpeed : walkSpeed;
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            controller.Move(move * currentSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        public void OnJump()
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }
}
