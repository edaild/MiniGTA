using UnityEngine;
using UnityEngine.AI;
using bullet.fx.pack;

public class PoliceAI : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    Gun gun;
    Transform player;

    public float shootRange = 20f;
    public float fireCooldown = 0.5f;
    float fireTimer = 0f;

    CapsuleCollider playerCol;

    [Header("Gun Visible Toggle")]
    public GameObject gunObject; // 총 모델 or 총 루트 오브젝트 (Inspector에 넣어도 됨)

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>(true);
        gun = GetComponentInChildren<Gun>(true); // ✅ 비활성 포함해서 찾기

        // ✅ gunObject를 인스펙터에 안 넣었으면 Gun이 붙은 오브젝트를 총로 잡음
        if (gunObject == null && gun != null)
            gunObject = gun.gameObject;

        // ✅ 시작은 "숨김"으로 (원하면 주석처리 가능)
        if (gunObject != null)
            gunObject.SetActive(false);
    }

    void Start()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerCol = p.GetComponent<CapsuleCollider>();
        }
    }

    void Update()
    {
        // ✅ 플레이어가 Start때 없을 수도 있으니 계속 재탐색
        if (player == null) FindPlayer();

        if (player == null || agent == null) return;
        if (!agent.isOnNavMesh) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool inShootRange = dist <= shootRange;

        // ✅ 여기 추가: 사거리 밖(추격) = 총 숨김, 사거리 안(조준/사격) = 총 보임
        if (gunObject != null)
            gunObject.SetActive(inShootRange);

        if (inShootRange)
        {
            agent.SetDestination(transform.position);

            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    Time.deltaTime * 10f
                );
            }
        }
        else
        {
            agent.SetDestination(player.position);
        }

        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetFloat("MotionSpeed", 1f);
            animator.SetBool("Grounded", true);
            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);
            animator.SetBool("IsAiming", inShootRange);
        }

        fireTimer += Time.deltaTime;

        // ✅ 총이 숨겨져 있으면(사거리 밖) 어차피 안 쏨
        if (inShootRange && fireTimer >= fireCooldown && gun != null)
        {
            fireTimer = 0f;

            Vector3 target;
            if (playerCol != null)
                target = playerCol.transform.TransformPoint(playerCol.center); // 몸통(센터)
            else
                target = player.position + Vector3.up * 0.9f;

            gun.Shoot(target, Gun.ShooterTeam.Police);
        }
    }
}
