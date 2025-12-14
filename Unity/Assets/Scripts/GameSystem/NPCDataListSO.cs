using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCCaharacter
{
    public int npc_type_id;
    public string npc_name;
    public bool is_hostile;
    public int base_health;
    public int base_damage;
    public int base_money;

    // 서버에서 못 받아오는 값(유니티 전용)
    public GameObject npcPrefab;
}

[CreateAssetMenu(fileName = "NPCList", menuName = "Game Data/NPC List")]
public class NPCDataListSO : ScriptableObject
{
    public List<NPCCaharacter> NPCs = new List<NPCCaharacter>();
}
