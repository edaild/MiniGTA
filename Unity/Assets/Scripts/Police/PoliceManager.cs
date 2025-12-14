using UnityEngine;

public class PoliceManager : MonoBehaviour
{
    public static PoliceManager Instance;

    [Header("Police Spawn Settings")]
    public GameObject policePrefab;      // 경찰 NPC 프리팹
    public Transform policeSpawnPoint;   // 경찰차 옆 같은 스폰 위치
    public int policeSpawnCount = 2;     // 한 번에 몇 명 나올지

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnPoliceWave()
    {
        UIManager.Instance?.ShowPoliceAlert("경찰 출현!");

        if (policePrefab == null || policeSpawnPoint == null)
        {
            Debug.LogWarning("PoliceManager: policePrefab 또는 policeSpawnPoint가 비어 있습니다.");
            return;
        }

        for (int i = 0; i < policeSpawnCount; i++)
        {
            Vector3 offset = new Vector3(i * 1.5f, 0f, 0f); 
            Instantiate(
                policePrefab,
                policeSpawnPoint.position + offset,
                policeSpawnPoint.rotation
            );
        }
    }
}
