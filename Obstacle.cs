using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Moving Actions")]
    public bool isRotate;
    public bool isMoving;

    [Header("Moving Counts")]
    [SerializeField] private float targetCloseDistance = 1f;
    [SerializeField] private float turningSpeed = 300;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private List<GameObject> movePoints;
    [SerializeField] private Vector3 rotateAxis;

    private GameObject currentTarget;
    private int currentTargetLevel = 0;

    private void Awake()
    {
        if (isMoving)
        {
            int random = Random.Range(0, movePoints.Count);
            currentTarget = movePoints[random];
            currentTargetLevel = random;
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) < targetCloseDistance)
            {
                currentTargetLevel++;

                if (movePoints.Count - 1 >= currentTargetLevel)
                {
                    currentTarget = movePoints[currentTargetLevel];
                }
                else
                {
                    currentTarget = movePoints[0];
                    currentTargetLevel = 0;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);
        }

        if (isRotate) transform.Rotate(rotateAxis, turningSpeed * Time.deltaTime);
    }
}
