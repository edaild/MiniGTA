using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f;

    private bool isGrounded = false;

    private Rigidbody rb;
    public Transform playerCamera;
    private float xRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCamera = GetComponentInChildren<Camera>()?.transform;

        if (playerCamera == null)
        {
            Debug.LogError("플레이어 오브젝트의 자식으로 카메라가 존재하지 않습니다!");
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLockToggle();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleLook();
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleMovement();
        }
    }

    void HandleMouseLockToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        Vector3 targetVelocity = move.normalized * moveSpeed;

        Vector3 velocityChange = targetVelocity - rb.velocity;
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }

    void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}