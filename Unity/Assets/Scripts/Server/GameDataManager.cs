using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Shop
{
    public int shop_id;
    public string gun_Name;
    public int transaction_price;
}

[System.Serializable]
public class Player
{
    public int player_id;
    public string player_email;
    public string player_name;
    public int current_money;
}


public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }


    [Header("Prefab Mapping (npc_type_id index)")]
    public GameObject[] npcPrefabs;
    public Player currentPlayer;
    public static string CurrentUserEmail;

    public string serverurl = "http://localhost:3000";

    public WeaponDataListSO weaponSO;
    public NPCDataListSO npcSO;
    public List<Shop> shops = new List<Shop>();


    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (weaponSO == null || npcSO == null)
        {
            Debug.LogError("WeaponDataListSO 또는 NPCDataListSO가 할당되지 않았습니다. 데이터를 로드할 수 없습니다.");
            return;
        }

        StartCoroutine(GetWeapon());
        StartCoroutine(GetNPC_Character());
        StartCoroutine(GetShop());
    }

    private IEnumerator GetWeapon()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverurl}/weapon_types"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                List<Weapon> tempWeaponList = JsonConvert.DeserializeObject<List<Weapon>>(www.downloadHandler.text);
                weaponSO.Weapons = tempWeaponList;
                Debug.Log($"[Weapon Data] {weaponSO.Weapons.Count}개의 무기 데이터를 SO에 성공적으로 저장.");
            }
            else
            {
                Debug.LogError("무기 조회 실패: " + www.error);
            }
        }
    }

    private IEnumerator GetNPC_Character()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverurl}/npc_character"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                List<NPCCaharacter> tempNPCList =
                    JsonConvert.DeserializeObject<List<NPCCaharacter>>(www.downloadHandler.text);

               
                foreach (var npc in tempNPCList)
                {
                    int id = npc.npc_type_id;
                    if (npcPrefabs != null && id >= 0 && id < npcPrefabs.Length)
                        npc.npcPrefab = npcPrefabs[id];
                }

                npcSO.NPCs = tempNPCList;
                Debug.Log($"[NPC Data] {npcSO.NPCs.Count}개 저장 + 프리팹 매핑 완료");
            }
            else
            {
                Debug.LogError("NPC 캐릭터 조회 실패: " + www.error);
            }
        }
    }

    private IEnumerator GetShop()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverurl}/shop"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                shops = JsonConvert.DeserializeObject<List<Shop>>(www.downloadHandler.text);
                Debug.Log("--- 상점 데이터 수신 완료 ---");
                foreach (var shop in shops)
                {
                    Debug.Log($" 상점 id : {shop.shop_id}, 무기 이름 : {shop.gun_Name}, 무기 가격 : {shop.transaction_price}");
                }
            }
            else
            {
                Debug.LogError("상점 조회 실패: " + www.error);
            }
        }
    }

    public void SetPlayerData(UserData data)
    {
        Debug.Log("SetPlayerData 실행");
        CurrentUserEmail = data.playerEmail;
        currentPlayer.player_id = data.playerId;
        currentPlayer.player_name = data.playerName;
        currentPlayer.current_money = data.currentMoney;

        Debug.Log($"플레이어 이메일 : {data.playerEmail}");
    }
}