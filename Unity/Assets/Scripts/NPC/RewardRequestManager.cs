using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class RewardRequestManager : MonoBehaviour
{
    public static RewardRequestManager Instance;

    private const string RewardApiUrl = "http://localhost:3003/reward";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RequestKillReward(string useremail, int npcId)
    {
        StartCoroutine(RequestRoutine(useremail, npcId));
    }

    private IEnumerator RequestRoutine(string useremail, int npcId)
    {
        if (string.IsNullOrEmpty(useremail))
        {
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

                if (response != null && response.success)
                {
                    if (UIManager.Instance != null)
                        UIManager.Instance.SetMoney(response.newMoney);
                }
            }
            else
            {
                // 서버 꺼져있으면 여기로 옴 (이미 클라에서 AddMoney로 올려둔 상태)
                Debug.LogWarning($"보상 서버 요청 실패: {www.error} (Code: {www.responseCode})");
            }
        }
    }
}
