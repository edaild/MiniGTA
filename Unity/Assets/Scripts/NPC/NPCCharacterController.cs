using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NPCCharacterController : MonoBehaviour
{
    [Header("NPC Data")]
    public NPCCaharacter NpcCharacterData;
    public Slider hpBar;

    [Header("Respawn (Citizen Only)")]
    public GameObject respawnPrefab;   
    public float respawnDelay = 20f;

    [Header("Server")]
    private const string RewardApiUrl = "http://localhost:3003/reward";

    [Header("Local Fallback (Server OFF Test)")]
    public bool useLocalRewardFallback = true;
    public int localFallbackMoney = 100;

    int health;
    int maxHealth;
    bool isDead = false;

    void Start()
    {
        if (NpcCharacterData == null)
        {
            Debug.LogError("[NPC] NpcCharacterData 없음");
            return;
        }

        maxHealth = NpcCharacterData.base_health;
        health = maxHealth;

        if (hpBar != null)
            hpBar.value = 1f;
    }

    // ===================== DAMAGE =====================
    void ApplyDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (hpBar != null)
            hpBar.value = Mathf.Clamp01((float)health / maxHealth);

        if (health <= 0)
            Die();
    }

    // ===================== DIE =====================
    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1️⃣ 시민이면 경찰 스폰
        if (!NpcCharacterData.is_hostile && PoliceManager.Instance != null)
            PoliceManager.Instance.SpawnPoliceWave();

        // 2️⃣ 서버에 보상 요청 (이건 그대로)
        string userEmail = GameDataManager.CurrentUserEmail;
        int npcId = NpcCharacterData.npc_type_id;
        StartCoroutine(RequestReward(userEmail, npcId));

        // 3️⃣ 기능만 끄기 (Destroy ❌)
        if (hpBar != null)
            hpBar.gameObject.SetActive(false);

        foreach (var col in GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.enabled = false;

        // 4️⃣ 리스폰 코루틴 시작
        StartCoroutine(RespawnCoroutine());
    }


    // ===================== SERVER REWARD =====================
    IEnumerator RequestReward(string useremail, int npcTypeId)
    {
        // 이메일 없으면 서버 불가 → fallback
        if (string.IsNullOrEmpty(useremail))
        {
            ApplyFallbackMoney();
            yield break;
        }

        var requestData = new NpcKillRequest
        {
            useremail = useremail,
            npcTypeId = npcTypeId
        };

        string json = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(RewardApiUrl, ""))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<NpcKillResponse>(www.downloadHandler.text);
                    if (response != null && response.success)
                    {
                        UIManager.Instance?.SetMoney(response.newMoney);
                        yield break;
                    }
                }
                catch { }
            }

            // 서버 실패 → fallback
            ApplyFallbackMoney();
        }
    }

    void ApplyFallbackMoney()
    {
        if (!useLocalRewardFallback) return;

        int money = (NpcCharacterData != null)
            ? NpcCharacterData.base_money
            : localFallbackMoney;

        UIManager.Instance?.AddMoney(money);
    }

   
    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Bullet"))
        {
            ApplyDamage(100);
            Destroy(collision.gameObject);
        }
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // 체력 초기화
        health = maxHealth;
        isDead = false;

        // 다시 켜기
        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.enabled = true;

        foreach (var col in GetComponentsInChildren<Collider>(true))
            col.enabled = true;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            agent.ResetPath();
        }

        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(true);
            hpBar.value = 1f;
        }
    }
}
