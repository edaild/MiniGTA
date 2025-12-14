using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어만 피해
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
