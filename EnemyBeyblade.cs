using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyBeyblade : MonoBehaviour
{
    public enum BeybladeType { normal, elite};

    
    [SerializeField] private float power = 10;
    [SerializeField] private float DestroyTime = 3;
    [SerializeField] private float randomForceMin = 3;
    [SerializeField] private float randomForceMax = 5;
    [SerializeField] private float randomTorqueMax = 2;

    [Header("Beyblade Type")]
    public BeybladeType type;
    public GameObject normalBeybladeObj;
    public GameObject eliteBeybladeObj;

    [Header("Score Text")]
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private float textMoveSpeed = 10;

    [HideInInspector] public bool isDead = false;

    [Header("Move Counts")]
    [SerializeField] private bool isMoveToTargets = false;
    [SerializeField] private float targetCloseDistance = 1f;
    [SerializeField] private float turningSpeed = 300;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private List<GameObject> movePoints;

    /*[SerializeField] private float explosionForce = 3;
     [SerializeField] private float explosionRadius = 3;*/
    [SerializeField] private List<GameObject> normalParts;
    [SerializeField] private List<GameObject> eliteParts;

    private GameObject currentTarget;
    private int currentTargetLevel = 0;

    private void Awake()
    {
        scoreText.gameObject.SetActive(false);

        if (isMoveToTargets)
        {
            int random = Random.Range(0, movePoints.Count);
            currentTarget = movePoints[random];
            currentTargetLevel = random;
        }
    }

    private void FixedUpdate()
    {
        if (isMoveToTargets)
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

        if(!isDead) transform.Rotate(transform.up, turningSpeed * Time.deltaTime);
    }

    public void Crash()
    {
        isDead = true;

        if (type == BeybladeType.normal)
        {
            foreach (var part in normalParts)
            {
                Rigidbody partRigid = part.GetComponent<Rigidbody>();
                partRigid.constraints = RigidbodyConstraints.None;

                int isaret = Random.Range(-1, 1);

                partRigid.mass = 20;

                if (isaret == 0) partRigid.AddForce(Random.Range(randomForceMin, randomForceMax) * transform.forward + transform.up * Random.Range(randomForceMin, randomForceMax) + transform.right * Random.Range(randomForceMin, randomForceMax));
                else partRigid.AddForce(Random.Range(randomForceMin, randomForceMax) * transform.forward + transform.up * Random.Range(randomForceMin, randomForceMax) + transform.right * Random.Range(randomForceMin, randomForceMax) * -1);

                partRigid.AddTorque(Vector3.one * Random.Range(0, randomTorqueMax));

                Destroy(part, DestroyTime);
            }
        }
        else
        {
            foreach (var part in eliteParts)
            {
                Rigidbody partRigid = part.GetComponent<Rigidbody>();
                partRigid.constraints = RigidbodyConstraints.None;

                int isaret = Random.Range(-1, 1);

                partRigid.mass = 20;

                if (isaret == 0) partRigid.AddForce(Random.Range(randomForceMin, randomForceMax) * transform.forward + transform.up * Random.Range(randomForceMin, randomForceMax) + transform.right * Random.Range(randomForceMin, randomForceMax));
                else partRigid.AddForce(Random.Range(randomForceMin, randomForceMax) * transform.forward + transform.up * Random.Range(randomForceMin, randomForceMax) + transform.right * Random.Range(randomForceMin, randomForceMax) * -1);

                partRigid.AddTorque(Vector3.one * Random.Range(0, randomTorqueMax));

                Destroy(part, DestroyTime);
            }
        }

        Destroy(gameObject, DestroyTime);
    }

    public void Score(float score)
    {
        GameObject spawned = Instantiate(scoreText.gameObject, scoreText.transform.position, Quaternion.identity);

        spawned.GetComponent<TextMeshPro>().text = ((int)score).ToString();

        spawned.SetActive(true);

        StartCoroutine(ScoreMove(spawned));

        Destroy(spawned.gameObject, 0.5f);
    }

    private IEnumerator ScoreMove(GameObject textObj)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (textObj != null) textObj.transform.position += Vector3.up * textMoveSpeed * Time.deltaTime;

            else break;
        }
    }

    public void CheckTypeBeyblade()
    {
        if (type == BeybladeType.normal)
        {
            normalBeybladeObj.SetActive(true);
            eliteBeybladeObj.SetActive(false);
        }
        else
        {
            normalBeybladeObj.SetActive(false);
            eliteBeybladeObj.SetActive(true);
        }
    }
}
