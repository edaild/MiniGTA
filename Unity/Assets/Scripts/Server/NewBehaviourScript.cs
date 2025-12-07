using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    private string CurrentUserEmail => GameDataManager.CurrentUserEmail;

    private const string ApiBaseUrl = "http://localhost:3003";

    public UserData PlayerData { get; private set; }

    public void Start()
    {
        StartCoroutine(FetchPlayerDataRoutine(5f));
    }

    private IEnumerator FetchPlayerDataRoutine(float interval)
    {
        while (true)
        {
            if (!string.IsNullOrEmpty(CurrentUserEmail))
            {
                yield return StartCoroutine(FetchPlayerData(CurrentUserEmail));
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public IEnumerator FetchPlayerData(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            Debug.LogError("플레이어 이메일이 설정되지 않았습니다.");
            yield break;
        }

        string url = $"{ApiBaseUrl}/api/player/{userEmail}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                PlayerInfoResponse response = JsonConvert.DeserializeObject<PlayerInfoResponse>(www.downloadHandler.text);

                if (response.success)
                {
                    PlayerData = response.user;
                    Debug.Log($"인게임 데이터 업데이트 성공: 돈: {PlayerData.currentMoney}, 레벨: {PlayerData.playerLevel}");
                }
                else
                {
                    Debug.LogError($"인게임 데이터 조회 실패: {response.message}");
                }
            }
            else
            {
                Debug.LogError($"서버 요청 실패 ({url}): {www.error}");
            }
        }
    }
}