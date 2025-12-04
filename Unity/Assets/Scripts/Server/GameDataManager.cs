//using Microsoft.Unity.VisualStudio.Editor;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore.Text;


[System.Serializable]
public class Weapon
{
    public int weapon_id;
    public string weapon_name;
    public float weapon_damage;
    public string weapon_type;
}

[System.Serializable]
public class Shop
{
    public int shop_id;
    public string gun_name;
    public int gun_price;
}

public class GameDataManager : MonoBehaviour
{
    public string serverurl = "http://localhost:3000";

    public List<Weapon> Weapondata = new List<Weapon>();
    public List<Shop> shops = new List<Shop>();


    private void Start()
    {
        StartCoroutine(GetWeapon());
        StartCoroutine(GetShop());
    }
    private IEnumerator GetWeapon()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{serverurl}/weapon"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Weapondata = JsonConvert.DeserializeObject<List<Weapon>>(www.downloadHandler.text);
                Debug.Log("들어온 데이터");
                Debug.Log("---------------------------");
                foreach (var weapon  in Weapondata)
                {
                    Debug.Log($"무기 이름 : {weapon.weapon_name}, 데미지 : {weapon.weapon_damage}");
                }
                Debug.Log("---------------------------");
            }
            else
            {
                Debug.LogError("무기 조회 실패 " + www.error);
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
                    Debug.Log($" 상점 id : {shop.shop_id}, 무기 이름 : {shop.gun_name}, 무기 가격 : {shop.gun_price}");
                }
            }
            else
            {
                Debug.LogError("상점 조회 실패 " + www.error);
            }
        }
    }
}
