//using Microsoft.Unity.VisualStudio.Editor;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;


[System.Serializable]
public class Weapon
{
    public int weapon_type_id;
    public string weapon_name;
    public float base_damage;
    public string ammo_type;
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
    public int base_damage;
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
    public string serverurl = "http://localhost:3000";
    public WeaponDataListSO weaponSO;
    public NPCDataListSO npcSO;
    public List<Shop> shops = new List<Shop>();


    private void Start()
    {
        if(weaponSO == null || npcSO == null)
        {
            Debug.LogError("ScriptableObject 에셋을 인스펙터에 연결해주세요!");
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
                UnityEditor.EditorUtility.SetDirty(weaponSO);
            }
            else
            {
                Debug.LogError("무기 조회 실패 " + www.error);
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

                Debug.Log($"[NPC Data] {npcSO.NPCs.Count}개의 NPC 데이터를 SO에 성공적으로 저장했습니다.");

                UnityEditor.EditorUtility.SetDirty(npcSO);
            }
            else
            {
                Debug.LogError("NPC 캐릭터 조회 실패 " + www.error);
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
                Debug.Log("들어온 데이터");
                Debug.Log("---------------------------");
                foreach (var shop in shops)
                {
                    Debug.Log($" 상점 id : {shop.shop_id}, 무기 이름 : {shop.gun_Name}, 무기 가격 : {shop.transaction_price}");
                }
            }
            else
            {
                Debug.LogError("상점 조회 실패 " + www.error);
            }
        }
    }
}
