using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 5f;
    public Transform targetPosition;
    public float initialY;
    public List<Transform> boardPositions;
    public Vector3 fnisyPlayerPosition;
    public bool isMoving;
    public Collider player_collider;
    private int currentPositionIndex;

    private bool isreturn;

    public void Awake()
    {
        if(player_collider == null)
            player_collider = GetComponent<Collider>();
    }

    private void Start()
    {
        initialY = transform.position.y;
        Debug.Log(boardPositions.Count);
        Debug.Log("initialY");
    }

    private void Update()
    {
        if (isMoving == true && targetPosition != null)
            HandleMove();
        

        if(transform.position == fnisyPlayerPosition)
            isMoving = false;

        if (Input.GetKeyDown(KeyCode.N) && !isMoving)
        {
            int diceResult = Random.Range(1, 7);
            Debug.Log($"주사위 수는?: {diceResult}");

            int nextIndex = (currentPositionIndex + diceResult) % boardPositions.Count;
            targetPosition = boardPositions[nextIndex];
            currentPositionIndex += nextIndex;
            isMoving = true;
        }

        ResetCurrentPositionIndex();


    }

    private void ResetCurrentPositionIndex()
    {
        if(currentPositionIndex >= 28)
            currentPositionIndex = 0;
    }

    private void HandleMove()
    {
        player_collider.enabled = false;

        float fixedY = initialY;

        float step = playerSpeed * Time.deltaTime;

        Vector3 targetPositionFixedY = new Vector3(targetPosition.position.x, fixedY, targetPosition.position.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPositionFixedY, step);

        if (transform.position == targetPositionFixedY)
        {
            isMoving = false;
            Debug.Log("목적지 도착.");
            player_collider.enabled = true;
        }
            
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Debug.Log("블럭과 충돌");
            BlockData blockData = collision.gameObject.GetComponent<BlockData>();

            if (blockData == null) return;

            if (blockData.blockSO == null) return;
            string name = blockData.blockSO.Block_name;
            int price = blockData.blockSO.Block_price;

            Debug.Log($"현재 블럭 이름은 : {name}, 가격은:{price}");
        }
    }
}
