using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject deathPanel;
    public GameObject shopPanel;
    public GameObject inventoryPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (deathPanel != null) deathPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ToggleShopPanel();

        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventoryPanel();
    }

    public void ShowDeathUI()
    {
        if (deathPanel != null) deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ToggleShopPanel()
    {
        if (shopPanel == null) return;

        bool newState = !shopPanel.activeSelf;
        shopPanel.SetActive(newState);

        if (newState && inventoryPanel != null)
            inventoryPanel.SetActive(false);

        UpdateCursorState();
    }

    public void ToggleInventoryPanel()
    {
        if (inventoryPanel == null) return;

        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);

        if (newState && shopPanel != null)
            shopPanel.SetActive(false);

        UpdateCursorState();
    }

    void UpdateCursorState()
    {
        bool anyOpen =
            (shopPanel != null && shopPanel.activeSelf) ||
            (inventoryPanel != null && inventoryPanel.activeSelf);

        if (anyOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
