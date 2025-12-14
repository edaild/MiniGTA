using System.Collections;
using UnityEngine;

public class CitizenSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject citizenPrefab;   // ✅ 프로젝트 창 프리팹만
    public float respawnDelay = 20f;

    [HideInInspector] public GameObject currentInstance;

    void Start()
    {
        SpawnNow();
    }

    public void SpawnNow()
    {
        if (citizenPrefab == null)
        {
            Debug.LogError("[CitizenSpawner] citizenPrefab 비었음");
            return;
        }

        currentInstance = Instantiate(citizenPrefab, transform.position, transform.rotation);

       
        var ctrl = currentInstance.GetComponent<NPCCharacterController>();
        if (ctrl != null)
        {
            
            ctrl.respawnDelay = respawnDelay;
        }
    }

    public void RequestRespawn()
    {
        StartCoroutine(CoRespawn());
    }

    IEnumerator CoRespawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnNow();
    }
}
