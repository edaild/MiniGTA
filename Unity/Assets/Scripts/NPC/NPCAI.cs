using UnityEngine;
using UnityEngine.AI;

public class NPCAI : MonoBehaviour
{
    public Transform player;

    public float walkSpeed = 1.5f;
    public float runSpeed = 3.5f;
    public float wanderRadius = 10f;
    public float alertRadius = 15f;
    public float alertTime = 4f;
    public float fleeDistance = 10f;

    NavMeshAgent agent;
    Animator animator;

    bool isAlert = false;
    float alertTimer = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        // 🔥 리스폰/재활성화 시 완전 초기화
        isAlert = false;
        alertTimer = 0f;

        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = walkSpeed;
            agent.ResetPath();
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        SetRandomDestination();
    }

    void Update()
    {
        if (agent == null || !agent.enabled) return;

        if (isAlert)
        {
            alertTimer -= Time.deltaTime;

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                FleeFromPlayer();
            }

            if (alertTimer <= 0f)
            {
                isAlert = false;
                agent.speed = walkSpeed;
                SetRandomDestination();
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                SetRandomDestination();
            }
        }

        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
            animator.SetBool("Grounded", true);
            animator.SetFloat("MotionSpeed", 1f);
        }
    }

    void SetRandomDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FleeFromPlayer()
    {
        if (player == null) return;

        float currentDist = Vector3.Distance(transform.position, player.position);

        for (int i = 0; i < 10; i++)
        {
            Vector3 baseDir = (transform.position - player.position).normalized;
            if (baseDir.sqrMagnitude < 0.01f) baseDir = transform.forward;

            float angle = Random.Range(-80f, 80f);
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * baseDir;
            Vector3 candidate = transform.position + dir * fleeDistance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                float newDist = Vector3.Distance(hit.position, player.position);
                if (newDist > currentDist + 1f)
                {
                    agent.speed = runSpeed;
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }

        SetRandomDestination();
    }

    public void OnGunShot()
    {
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > alertRadius) return;
        }

        isAlert = true;
        alertTimer = alertTime;
        agent.speed = runSpeed;

        FleeFromPlayer();
    }
}
