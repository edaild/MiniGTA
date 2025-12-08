using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Weapon
{
    public int weapon_type_id;
    public string weapon_name;
    public float base_damage;
    public string ammo_type;
    public GameObject WeaponPrefab;
}

[System.Serializable]
public class Shop
{
    public int shop_id;
    public string gun_Name;
    public int transaction_price;
}

[System.Serializable]
public class NPCCaharacter
{
    public int npc_type_id;
    public string npc_name;
    public bool is_hostile;
    public int base_health;
    public int base_damage;
    public int base_money;
    public GameObject npcPrefab;
}

[CreateAssetMenu(fileName = "WeaponList", menuName = "Game Data/Weapon List")]
public class WeaponDataListSO : ScriptableObject
{
    public List<Weapon> Weapons = new List<Weapon>();
}

[CreateAssetMenu(fileName = "NPCList", menuName = "Game Data/NPC List")]
public class NPCDataListSO : ScriptableObject
{
    public List<NPCCaharacter> NPCs = new List<NPCCaharacter>();
}

public class GameDataManager : MonoBehaviour
{
    
    public static string CurrentUserEmail = "111";

    public string serverurl = "http://localhost:3000";

    public WeaponDataListSO weaponSO;
    public NPCDataListSO npcSO;
    public List<Shop> shops = new List<Shop>();


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
                List<NPCCaharacter> tempNPCList = JsonConvert.DeserializeObject<List<NPCCaharacter>>(www.downloadHandler.text);
                npcSO.NPCs = tempNPCList;

                Debug.Log($"[NPC Data] {npcSO.NPCs.Count}개의 NPC 데이터를 SO에 성공적으로 저장");
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
}