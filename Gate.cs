using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gate : MonoBehaviour
{
    public enum GateAction
    {
        good,
        bad,
    }

    public enum GateGoodFormul
    {
        empty,
        plus,
        multiply,
    }

    public enum GateBadFormul
    {
        empty,
        minus,
        divided
    }

    public GateAction gateAction;
    public GateGoodFormul gateGoodFormul;
    public GateBadFormul gateBadFormul;

    public TextMeshPro text;
    public TextMeshPro powerText;

    [Header("Materials")]
    public GameObject goodObj;
    public GameObject badObj;

    public float actionCount;

    [Header("Move Counts")]
    public bool isMoveToTargets = false;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float targetCloseDistance = 1f;
    [SerializeField] private List<GameObject> movePoints;

    private GameObject currentTarget;
    private int currentTargetLevel = 0;


    private void Start()
    {
        ChangeGate();

        if (isMoveToTargets)
        {
            int random = Random.Range(0, movePoints.Count);
            currentTarget = movePoints[random];
            currentTargetLevel = random;

            StartCoroutine(Move());
        }
    }

    public void ChangeGate()
    {
        if (gateAction == GateAction.good)
        {
            if (gateGoodFormul == GateGoodFormul.plus)
            {
                text.text = "+ " + actionCount;
                powerText.text = "POWER BOOST";
            }
            else if (gateGoodFormul == GateGoodFormul.multiply)
            {
                text.text = "x " + actionCount;
                powerText.text = "POWER BOOST";
            }
            
            goodObj.SetActive(true);
            badObj.SetActive(false);
        }
        else if (gateAction == GateAction.bad)
        {
            if (gateBadFormul == GateBadFormul.minus)
            {
                text.text = "- " + actionCount;
                powerText.text = "POWER REDUCE";
            }
            else if (gateBadFormul == GateBadFormul.divided)
            {
                text.text = "÷ " + actionCount;
                powerText.text = "POWER REDUCE";
            }

            goodObj.SetActive(false);
            badObj.SetActive(true);
        }
    }

    private IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

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
    }
}
