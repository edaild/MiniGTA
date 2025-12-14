using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public int weapon_type_id;
    public string weapon_name;
    public float base_damage;
    public string ammo_type;

  
    public GameObject WeaponPrefab;
}

[CreateAssetMenu(fileName = "WeaponList", menuName = "Game Data/Weapon List")]
public class WeaponDataListSO : ScriptableObject
{
    public List<Weapon> Weapons = new List<Weapon>();
}
