using UnityEngine;

public class GunFollower : MonoBehaviour
{
    // <<<< 이게 인스펙터에 보이는 슬롯
    public Transform hand;

    void LateUpdate()
    {
        if (hand == null) return;

        // 손 위치/회전 강제 복사
        transform.position = hand.position;
        transform.rotation = hand.rotation;
    }
}
