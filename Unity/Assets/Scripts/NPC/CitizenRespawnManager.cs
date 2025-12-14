using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CitizenRespawnManager : MonoBehaviour
{
    public static CitizenRespawnManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RespawnAfter(GameObject prefab, Vector3 pos, Quaternion rot, float delay)
    {
        Debug.Log($"[Respawn] Request 들어옴 prefab={(prefab ? prefab.name : "NULL")} delay={delay} pos={pos}");

        if (prefab == null)
        {
            Debug.LogError("[Respawn] prefab null");
            return;
        }

        if (prefab.scene.IsValid())
        {
            Debug.LogError("[Respawn] prefab에 씬 오브젝트가 들어있음(하이라키). Project 프리팹 넣어야함");
            return;
        }

        StartCoroutine(CoRespawn(prefab, pos, rot, delay));
    }

    IEnumerator CoRespawn(GameObject prefab, Vector3 pos, Quaternion rot, float delay)
    {
        Debug.Log("[Respawn] 코루틴 시작");
        yield return new WaitForSecondsRealtime(delay); // ✅ timeScale 0이어도 동작
        Debug.Log("[Respawn] 대기 끝, Instantiate 시도");

        Vector3 spawnPos = pos;
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            spawnPos = hit.position;
        else
            Debug.LogWarning("[Respawn] NavMesh 샘플 실패 -> 원래 pos에 스폰");

        Instantiate(prefab, spawnPos, rot);
        Debug.Log("[Respawn] Instantiate 완료");
    }
}
