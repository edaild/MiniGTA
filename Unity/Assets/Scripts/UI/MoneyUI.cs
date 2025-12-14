using System.Collections;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    [Header("Count Up")]
    [SerializeField] private float countDuration = 0.35f; // 숫자 올라가는 시간
    [SerializeField] private bool showDeltaText = true;

    [Header("Punch")]
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchTime = 0.12f;

    private int _currentMoney;
    private Coroutine _co;

    void Reset()
    {
        moneyText = GetComponent<TMP_Text>();
    }

    public void SetMoneyInstant(int money)
    {
        _currentMoney = money;
        UpdateText(_currentMoney, 0);
    }

    public void AnimateTo(int newMoney)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(CoAnimateMoney(newMoney));
    }

    private IEnumerator CoAnimateMoney(int newMoney)
    {
        int from = _currentMoney;
        int to = newMoney;
        int delta = to - from;

        // 펀치(살짝 커졌다가 원복)
        Vector3 baseScale = transform.localScale;
        transform.localScale = baseScale * punchScale;
        float tPunch = 0f;
        while (tPunch < punchTime)
        {
            tPunch += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(tPunch / punchTime);
            transform.localScale = Vector3.Lerp(baseScale * punchScale, baseScale, k);
            yield return null;
        }
        transform.localScale = baseScale;

        // 카운트 업
        float t = 0f;
        float dur = Mathf.Max(0.01f, countDuration);

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dur);
            int val = Mathf.RoundToInt(Mathf.Lerp(from, to, k));
            UpdateText(val, delta);
            yield return null;
        }

        _currentMoney = to;
        UpdateText(_currentMoney, delta);
        _co = null;
    }

    private void UpdateText(int money, int delta)
    {
        if (showDeltaText && delta != 0)
        {
            string sign = delta > 0 ? "+" : "";
            moneyText.text = $"달러 : {money}$  <size=70%><color=#FFD24A>({sign}{delta})</color></size>";
        }
        else
        {
            moneyText.text = $"달러 : {money}$";
        }
    }
}
