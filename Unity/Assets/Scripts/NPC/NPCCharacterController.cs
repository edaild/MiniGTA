using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class NPCCharacterController : MonoBehaviour
{
    public NPCCaharacter NpcCharacterData;
    public int health;

    private const string RewardApiUrl = "http://localhost:3003/reward";

    private void Start()
    {
        if (NpcCharacterData == null) return;

        string name = NpcCharacterData.npc_name;
        health = NpcCharacterData.base_health;
        int money = NpcCharacterData.base_money;



        Debug.Log($"NPC '{NpcCharacterData.npc_name}' (ID: {NpcCharacterData.npc_type_id})가 생성되었습니다.");
        Debug.Log($"초기 체력: {NpcCharacterData.base_health}");
    }

    private void Update()
    {
        Die();

        if (Input.GetKeyDown(KeyCode.N))
        {
            health -= 100;
            Debug.Log("NPC 체력 감소");
        }
    }

    void Die()
    {
        if (health <= 0)
        {
            if (NpcCharacterData != null)
            {
                string userEmail = GameDataManager.CurrentUserEmail;
                Debug.Log(userEmail);

                //if (string.IsNullOrEmpty(userEmail))
                //{
                //    Debug.LogError("로그인된 사용자 이메일이 GameDataManager에 설정되지 않았습니다. 보상 지급 실패.");
                //    return;
                //}

                StartCoroutine(SendKillRewardRequest(userEmail, NpcCharacterData.npc_type_id));
                Destroy(transform.gameObject);
            }
        }
    }

    private IEnumerator SendKillRewardRequest(string useremail, int npcId)
    {
        if (string.IsNullOrEmpty(useremail))
        {
            Debug.LogError("보상 지급을 위해서는 플레이어 이메일이 필요합니다.");
            yield break;
        }

        NpcKillRequest requestData = new NpcKillRequest { useremail = useremail, npcTypeId = npcId };
        string jsonRequestBody = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(RewardApiUrl, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                NpcKillResponse response = JsonConvert.DeserializeObject<NpcKillResponse>(www.downloadHandler.text);

                if (response.success)
                {
                    Debug.Log($"NPC 처치 보상 지급 성공! 금액: {response.rewardAmount}, 현재 돈: {response.newMoney}");
                    Destroy(transform.gameObject);
                }
                else
                {
                    Debug.LogError($" 보상 지급 실패: {response.message}");
                    Destroy(transform.gameObject);
                }
            }
            else
            {
                Debug.LogError($" 보상 서버 요청 실패: {www.error} (Code: {www.responseCode})");
     
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= 100;
            Destroy(collision.gameObject);
        }
    }
}