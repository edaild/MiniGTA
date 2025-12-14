using UnityEngine;

public class GunFollowHand : MonoBehaviour
{
    [Header("따라갈 손 뼈")]
    public Transform handBone;         
    [Header("손 기준 위치 보정 (로컬 축)")]
    public Vector3 positionOffset;     

    [Header("손 기준 회전 보정 (도 단위)")]
    public Vector3 rotationOffset;     

    void LateUpdate()
    {
        if (handBone == null) return;

        
        transform.position = handBone.position;
        transform.rotation = handBone.rotation;

        transform.position += transform.right * positionOffset.x
                            + transform.up * positionOffset.y
                            + transform.forward * positionOffset.z;

        
        transform.rotation *= Quaternion.Euler(rotationOffset);
    }
}
