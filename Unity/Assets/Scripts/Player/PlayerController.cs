using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 5f;

    public float sprintMultiplier = 1.6f;
    public float maxStamina = 100f;
    public float staminaUseRate = 30f;
    public float staminaRegenRate = 20f;
    public float jumpStaminaCost = 10f;

    public float maxHealth = 100f;

    public Slider healthSlider;
    public Slider staminaSlider;
    public Text staminaWarningText;

    private float currentStamina;
    private float currentHealth;
    private bool isGrounded = false;
    private bool isSprinting = false;
    private bool staminaExhausted = false;

    private Rigidbody rb;
    public Transform playerCamera;
    private float xRotation = 0f;

    public float CurrentStamina => currentStamina;
    public float CurrentHealth => currentHealth;

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

        currentStamina = maxStamina;
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }

        if (staminaWarningText != null)
        {
            staminaWarningText.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLockToggle();

        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        bool hasMoveInput = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f;
        bool sprintInput = Input.GetKey(KeyCode.LeftShift);

        float recoverThreshold = maxStamina * 0.3f;

        bool canSprint = sprintInput && hasMoveInput && !staminaExhausted && isGrounded && currentStamina > 0f;

        isSprinting = canSprint;

        if (isSprinting)
        {
            currentStamina -= staminaUseRate * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                staminaExhausted = true;
                isSprinting = false;
            }
        }
        else
        {
            if (isGrounded && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (staminaExhausted && currentStamina >= recoverThreshold)
        {
            staminaExhausted = false;
        }

        if (staminaWarningText != null)
        {
            bool show = staminaExhausted || currentStamina <= 0f;
            staminaWarningText.gameObject.SetActive(show);
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            HandleLook();
        }

        bool jumpInput = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space);
        bool canJump = jumpInput && isGrounded && !staminaExhausted && currentStamina >= jumpStaminaCost && currentStamina >= recoverThreshold;

        if (canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            currentStamina -= jumpStaminaCost;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                staminaExhausted = true;
            }
        }

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

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

        float currentSpeed = moveSpeed;
        if (isSprinting)
        {
            currentSpeed *= sprintMultiplier;
        }

        Vector3 targetVelocity = move.normalized * currentSpeed;

        Vector3 velocityChange = targetVelocity - rb.velocity;
        velocityChange.y = 0;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}
