using UnityEngine;
using UnityEngine.UI;   // ✅ 반드시 필요
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject deathPanel;
    public GameObject shopPanel;
    public GameObject inventoryPanel;

    [Header("Money UI")]
   // public TMP_Text moneyText;
    private int currentMoney;

    [Header("Money Animation")]
    public float moneyAnimDuration = 0.3f;
    Coroutine moneyCoroutine;

    [Header("Police Alert UI")]
    public Text policeAlertText;          // ✅ 레거시 Text 유지
    public float policeAlertDuration = 2f;
    Coroutine policeAlertCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (deathPanel != null) deathPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        // 🔴 시작 시 무조건 끔
        if (policeAlertText != null)
            policeAlertText.gameObject.SetActive(false);
    }

    // 🚨 경찰 출현 UI
    public void ShowPoliceAlert(string msg = "경찰 출현!")
    {
        if (policeAlertText == null)
        {
            Debug.LogError("❌ policeAlertText 연결 안됨");
            return;
        }

        policeAlertText.text = msg;
        policeAlertText.gameObject.SetActive(true);

        if (policeAlertCoroutine != null)
            StopCoroutine(policeAlertCoroutine);

        policeAlertCoroutine = StartCoroutine(HidePoliceAlert());
    }

    IEnumerator HidePoliceAlert()
    {
        yield return new WaitForSecondsRealtime(policeAlertDuration);

        if (policeAlertText != null)
            policeAlertText.gameObject.SetActive(false);

        policeAlertCoroutine = null;
    }

    // 💰 돈 UI는 그대로
    public void SetMoney(int amount)
    {
        if (moneyCoroutine != null)
            StopCoroutine(moneyCoroutine);

        moneyCoroutine = StartCoroutine(AnimateMoney(currentMoney, amount));
        currentMoney = amount;
    }

    public void AddMoney(int amount)
    {
        SetMoney(currentMoney + amount);
    }

    IEnumerator AnimateMoney(int from, int to)
    {
        float t = 0f;

        while (t < moneyAnimDuration)
        {
            t += Time.unscaledDeltaTime;
            float ratio = Mathf.Clamp01(t / moneyAnimDuration);

            int value = Mathf.RoundToInt(Mathf.Lerp(from, to, ratio));

            //if (moneyText != null)
            //    moneyText.text = $"달러 : {value}$";

            yield return null;
        }

        //if (moneyText != null)
        //    moneyText.text = $"달러 : {to}$";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;           
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowDeathUI()
    {
        Time.timeScale = 1f; // ✅ 버튼 클릭 보장(사망 처리에서 0 걸어도 여기서 풀어줌)

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


}
