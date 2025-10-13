using UnityEngine;
using RudeWarriors.Framework.Core;
using UnityEngine.InputSystem;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[AutoRegister(persistent: true)]
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour, IService
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpHeight = 1.4f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float standingHeight = 2.0f;

    [Header("Look")]
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float lookSmoothness = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;
    private float targetHeight;
    private bool isGrounded;

#if ENABLE_INPUT_SYSTEM
    private InputSystem_Actions inputActions;
   // auto-generated input map
#endif

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

#if ENABLE_INPUT_SYSTEM
        input = new PlayerInputActions();
        input.Enable();
        RWDebug.System("[FPC] Using new Input System.");
#else
        RWDebug.System("[FPC] Using legacy Input Manager.");
#endif

        if (cameraPivot == null)
        {
            var cam = Camera.main;
            if (cam != null) cameraPivot = cam.transform;
        }

        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = Vector3.down * (controller.height / 2f - 0.1f);
            groundCheck = gc.transform;
        }

        targetHeight = controller.height;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        GroundCheck();
        HandleLook();
        HandleMovement();
        HandleCrouch();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    private void HandleMovement()
    {
        float x = 0f, z = 0f;
        bool jump = false;
        bool sprint = false;
        bool crouch = false;

#if ENABLE_INPUT_SYSTEM
        Vector2 move = input.Player.Move.ReadValue<Vector2>();
        x = move.x;
        z = move.y;
        jump = input.Player.Jump.triggered;
        sprint = input.Player.Sprint.IsPressed();
        crouch = input.Player.Crouch.IsPressed();
#else
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        jump = Input.GetButtonDown("Jump");
        sprint = Input.GetKey(KeyCode.LeftShift);
        crouch = Input.GetKey(KeyCode.LeftControl);
#endif

        float speed = crouch ? crouchSpeed : (sprint ? sprintSpeed : walkSpeed);
        Vector3 moveVec = transform.right * x + transform.forward * z;
        controller.Move(moveVec * speed * Time.deltaTime);

        if (jump && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
#if ENABLE_INPUT_SYSTEM
        bool isCrouching = input.Player.Crouch.IsPressed();
#else
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
#endif
        targetHeight = isCrouching ? crouchHeight : standingHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * 10f);
    }

    private void HandleLook()
    {
        float mouseX = 0f;
        float mouseY = 0f;

#if ENABLE_INPUT_SYSTEM
        Vector2 look = input.Player.Look.ReadValue<Vector2>();
        mouseX = look.x * sensitivity;
        mouseY = look.y * sensitivity;
#else
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;
#endif

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Quaternion camRot = Quaternion.Euler(xRotation, 0f, 0f);
        cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, camRot, Time.deltaTime * lookSmoothness);
        transform.Rotate(Vector3.up * mouseX);
    }
}
