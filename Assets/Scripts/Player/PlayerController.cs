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

        public void OnMove(InputValue value) 
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnLook(InputValue value)
        {
            lookInput = value.Get<Vector2>();
        }

        public void OnSprint(InputValue value) 
        {
            sprintHeld = value.isPressed;
        }

        public void SetForcedSprint(bool forced) => isForcedSprint = forced;

        private void Update()
        {
            HandleRotation();
            HandleMovement();

            // Explicitly clear lookInput delta after processing to prevent spinning
            // but keep moveInput as it is a continuous state
            lookInput = Vector2.zero; 
        }

        private void HandleRotation()
        {
            // Direct polling of mouse delta for stability
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            
            float mouseX = mouseDelta.x * mouseSensitivity;
            float mouseY = mouseDelta.y * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleMovement()
        {
            // Direct polling of move input instead of waiting for events
            Vector2 input = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
            
            moveInput = input.normalized;
            sprintHeld = Keyboard.current.leftShiftKey.isPressed;

            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float currentSpeed = (sprintHeld || isForcedSprint) ? sprintSpeed : walkSpeed;
            
            // Calculate move direction
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            
            // Combine horizontal movement and vertical velocity into one vector
            Vector3 finalVelocity = move * currentSpeed;
            finalVelocity.y = velocity.y;

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;

            // SINGLE Move call
            controller.Move(finalVelocity * Time.deltaTime);
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
