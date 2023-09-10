using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class MapManager : MonoBehaviour
{
    [SerializeField] private MainCharacter character;
    
    [Header("Choose Spawn Objects")]
    //public bool isCandySpawn = true;
    public bool isEnemySpawn = true;
    public bool isTransformerSpawn = true;

    [Header("Path Spawn Values")]
    [SerializeField] int pathMinLength = 40;
    [SerializeField] int pathMaxLength = 60;
    private int pathLength = 15;
    [SerializeField] float pathPointBetwDistance = 6f;
    [SerializeField] float maxSideTurnDistance = 5f;
    [SerializeField] float pathTilingFactor = 1f;

    [Header("Group Values")]
    [Range(0, 100)] [SerializeField] private float obstacleTypePercent = 10;
    [Range(0, 100)] [SerializeField] private float enemyTypePercent = 10;
    [SerializeField] float groupBetwDistance = 5f;
    [SerializeField] private float intervalPosTransformGate = 1.75f;
    [SerializeField] private float intervalPosObstacle = 1.75f;
    [SerializeField] private int maxSpawnMultiplyCount = 3;
    [SerializeField] private float arrowDistance = 10;
    [SerializeField] private int arrowCount = 3;
    [SerializeField] private float arrowLightedTime = 1;

    /*[Header("Candy")]
    [SerializeField] private List<GameObject> candyStockPrefabs;
    [SerializeField] private List<GameObject> linedCandyPrefabs;
    [SerializeField] private GameObject chocolateBomb;
    [SerializeField] float candyBetwDistance = 2f;
    [SerializeField] int minGroupSpawnCount = 4;
    [SerializeField] int maxGroupSpawnCount = 7;*/

    [Header("Enemies and Obstacles")]
    [SerializeField] private List<GameObject> enemiesPrefabs;
    [SerializeField] private List<GameObject> obstaclePrefabs;

    [Header("TransformerGate")]
    [SerializeField] private GameObject transformerPrefab;
    [SerializeField] private float minPowerPlusMinusCount = 2;
    [SerializeField] private float maxPowerPlusMinusCount = 6;
    [SerializeField] private float minMultiplyDividedPowerCount = 2;
    [SerializeField] private float maxMultiplyDividedPowerCount = 6;

    [Header("End Game")]
    [SerializeField] private GameObject finalObj;
    [SerializeField] private Vector3 finalObjSpawnPos;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Material arrowNormalMat;
    [SerializeField] private Material arrowLightedMat;

    /*[Header("High Striker")]
    [SerializeField] private GameObject highStrikerPrefab;*/

    [HideInInspector] public VertexPath path;

    [HideInInspector] public VertexPath finishPath;

    [HideInInspector] public GameObject endPointer;

    private List<GameObject> spawnedpathPoints;
    private List<GameObject> spawnedTransformers;
    //private List<GameObject> spawnedCandies;
    private List<GameObject> spawnedEnemies;
    private List<GameObject> spawnedStorages;
    private List<GameObject> spawnedArrows;

    //[HideInInspector] public GameObject spawnedHighStriker;

    private GameObject pathsParent;
    private PathCreator pathCreator;
    private RoadMeshCreator roadMeshCreator;

    private int spawnedMultiplyCount = 0;
    private int lightedArrowCount = 0;

    private int GateActionCount(Gate.GateAction gateAction, Gate.GateGoodFormul gateGoodFormul, Gate.GateBadFormul gateBadFormul)
    {
        int count = 0;

        if(gateAction == Gate.GateAction.good && gateGoodFormul == Gate.GateGoodFormul.plus ||
            gateAction == Gate.GateAction.bad && gateBadFormul == Gate.GateBadFormul.minus) count = (int)Random.Range(minPowerPlusMinusCount, maxPowerPlusMinusCount);

        else if (gateAction == Gate.GateAction.good && gateGoodFormul == Gate.GateGoodFormul.multiply ||
            gateAction == Gate.GateAction.bad && gateBadFormul == Gate.GateBadFormul.divided) count = (int)Random.Range(minMultiplyDividedPowerCount, maxMultiplyDividedPowerCount);

        return count;
    }

    private void Awake()
    {
        Application.targetFrameRate = 0;
        QualitySettings.vSyncCount = 0;
        
        if (character == null) Debug.LogError("Karakteri map managera ata!");

        spawnedMultiplyCount = 0;

        pathLength = Random.Range(pathMinLength, pathMaxLength);

        #region Path Points Parent Create or Assign
        if (pathsParent == null)
		{
            if (transform.Find("PathPoints")) pathsParent = transform.Find("PathPoints").gameObject;
            else
			{
                pathsParent = new GameObject();
                pathsParent.transform.position = new Vector3(0, 0, 0);
                pathsParent.transform.rotation = Quaternion.identity;
                pathsParent.transform.parent = this.transform;
                pathsParent.gameObject.name = "PathPoints";
            }
        }
        #endregion

        PathSpawn();

        EndSpawn();

        PathDraw();

        GroupsSpawn();

        InvokeRepeating("ArrowLight", 0, arrowLightedTime);
    }

	private void ArrowLight()
	{
		for (int i = 0; i < spawnedArrows.Count; i++)
		{
            spawnedArrows[i].GetComponent<MeshRenderer>().material = arrowNormalMat;
		}

        spawnedArrows[lightedArrowCount].GetComponent<MeshRenderer>().material = arrowLightedMat;

        lightedArrowCount++;

        if (lightedArrowCount > spawnedArrows.Count - 1) lightedArrowCount = 0;
    }

	private void PathSpawn()
	{
        spawnedpathPoints = new List<GameObject>();

        GameObject firstPoint = new GameObject();
        firstPoint.transform.position = new Vector3(0, 0, 0);
        firstPoint.transform.rotation = Quaternion.identity;
        firstPoint.transform.parent = pathsParent.transform;
        firstPoint.gameObject.name = "PathPoint " + 0;

        spawnedpathPoints.Add(firstPoint);

        for (int i = 0; i < pathLength - 1; i++)
        {
            float nextPointRight = Random.Range(-maxSideTurnDistance, maxSideTurnDistance);

            GameObject nextPoint = new GameObject();
            nextPoint.transform.position = spawnedpathPoints[i].transform.position + spawnedpathPoints[i].transform.forward * pathPointBetwDistance + spawnedpathPoints[i].transform.right * nextPointRight;
            spawnedpathPoints[i].transform.LookAt(nextPoint.transform.position);
            nextPoint.transform.parent = pathsParent.transform;
            nextPoint.gameObject.name = "PathPoint " + (i + 1);

            spawnedpathPoints.Add(nextPoint);
        }
    }

    private void PathDraw()
	{
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < spawnedpathPoints.Count; i++)
		{
            points.Add(spawnedpathPoints[i].transform.position);
		}

        BezierPath bezierPath = new BezierPath(points, false, PathSpace.xyz);
        bezierPath.GlobalNormalsAngle = 90;

        pathCreator = GetComponent<PathCreator>();
        pathCreator.bezierPath = bezierPath;
        pathCreator.TriggerPathUpdate();
        
        path = new VertexPath(pathCreator.bezierPath, transform);

        roadMeshCreator = GetComponent<RoadMeshCreator>();
        roadMeshCreator.textureTiling = path.length * pathTilingFactor;
        roadMeshCreator.flattenSurface = true;
        roadMeshCreator.TriggerUpdate();

        roadMeshCreator.meshFilter.gameObject.AddComponent<MeshCollider>();

        /* Yolun Mesh Colliderý
         * GameObject meshHolder = GameObject.Find("Road Mesh Holder");
        if (!meshHolder.GetComponent<MeshCollider>()) meshHolder.AddComponent<MeshCollider>();
        meshHolder.GetComponent<MeshCollider>().convex = true;*/
    }

    private void GroupsSpawn()
	{
        /*spawnedCandies = new List<GameObject>();
        spawnedEnemies = new List<GameObject>();*/
        spawnedTransformers = new List<GameObject>();

        // Path progress count
        float distanceSpawned = 30;

        // Number of groups by path
        int i = 0;

        // Obstacle repeat count
        int obsRepCount = 0;

        while (distanceSpawned <= path.length - character.endStartValue - 20)
		{
            //float groupType = Random.Range(0f, 100f);

            #region Enemy Obstacle
            // Candy or Obstacle Spawn
            if (i % 5 != 0)
            {
                // Enemy Spawn
                if (isEnemySpawn)
                {
                    int percent = Random.Range(0, 100);

                    GameObject prefab;

                    if (percent > obstacleTypePercent)
                    {
                        int enemyType = Random.Range(0, enemiesPrefabs.Count);

                        prefab = enemiesPrefabs[enemyType];

                        EnemyBeyblade beyblade = null;

                        if (prefab.GetComponent<EnemyBeyblade>()) beyblade = prefab.GetComponent<EnemyBeyblade>();
                        else if (prefab.transform.GetChild(0).GetComponent<EnemyBeyblade>()) beyblade = prefab.transform.GetChild(0).GetComponent<EnemyBeyblade>();


                        if (beyblade != null)
                        {
                            int enemyPercent = Random.Range(0, 100);

                            if (enemyPercent > enemyTypePercent) beyblade.type = EnemyBeyblade.BeybladeType.normal;
                            else beyblade.type = EnemyBeyblade.BeybladeType.elite;

                            beyblade.CheckTypeBeyblade();
                        }
                    }

                    else
                    {
                        int obsType = Random.Range(0, obstaclePrefabs.Count);

                        prefab = obstaclePrefabs[obsType];
                    }

                    Vector3 spawnPoint = path.GetPointAtDistance(distanceSpawned, EndOfPathInstruction.Stop);

                    GameObject spawnedObs = Instantiate(prefab, spawnPoint, path.GetRotationAtDistance(distanceSpawned));

                    float firstDir = Random.Range(0, 100);

                    if (firstDir >= 50f)
                    {
                        spawnedObs.transform.position += spawnedObs.transform.right * intervalPosObstacle * 1;
                        spawnedObs.transform.rotation = Quaternion.Euler(new Vector3(0, spawnedObs.transform.rotation.y, 0));
                    }
                    else
                    {
                        spawnedObs.transform.position += spawnedObs.transform.right * intervalPosObstacle * -1;
                        spawnedObs.transform.rotation = Quaternion.Euler(new Vector3(0, spawnedObs.transform.rotation.y, 0));
                    }

                    obsRepCount++;
                }
            }
            #endregion

            // Transformer Spawn
            else if (isTransformerSpawn)
            {
                Vector3 spawnPoint = path.GetPointAtDistance(distanceSpawned, EndOfPathInstruction.Stop);

                float spawnPercent = Random.Range(0, 100);

                if (spawnPercent > 33f)
                {
                    GameObject spawnedTransformer = Instantiate(transformerPrefab, spawnPoint, path.GetRotationAtDistance(distanceSpawned));
                    GameObject spawnedTransformer1 = Instantiate(transformerPrefab, spawnPoint, path.GetRotationAtDistance(distanceSpawned));

                    Gate gate = spawnedTransformer.transform.GetChild(0).GetComponent<Gate>();
                    gate.isMoveToTargets = false;

                    gate.gateAction = Gate.GateAction.good;
                    if (spawnedMultiplyCount < maxSpawnMultiplyCount)
                    {
                        gate.gateGoodFormul = (Gate.GateGoodFormul)(Random.Range(1, 3));
                        spawnedMultiplyCount++;
                    }
                    else gate.gateGoodFormul = (Gate.GateGoodFormul)1;
                    gate.gateBadFormul = Gate.GateBadFormul.empty;
                    gate.actionCount = GateActionCount(Gate.GateAction.good, gate.gateGoodFormul, gate.gateBadFormul);

                    Gate gate1 = spawnedTransformer1.transform.GetChild(0).GetComponent<Gate>();
                    gate1.isMoveToTargets = false;

                    gate1.gateAction = Gate.GateAction.bad;
                    gate1.gateBadFormul = (Gate.GateBadFormul)(Random.Range(1, 3));
                    gate1.gateGoodFormul = Gate.GateGoodFormul.empty;
                    gate1.actionCount = GateActionCount(gate1.gateAction, gate1.gateGoodFormul, gate1.gateBadFormul);

                    int selectDir = Random.Range(0, 100);

                    if (selectDir > 50)
                    {
                        spawnedTransformer.transform.position += spawnedTransformer.transform.right * intervalPosTransformGate;
                        spawnedTransformer1.transform.position += spawnedTransformer1.transform.right * intervalPosTransformGate * -1;

                        spawnedTransformers.Add(spawnedTransformer);
                        spawnedTransformers.Add(spawnedTransformer1);
                    }
                    else
                    {
                        spawnedTransformer.transform.position += spawnedTransformer.transform.right * intervalPosTransformGate * -1;
                        spawnedTransformer1.transform.position += spawnedTransformer1.transform.right * intervalPosTransformGate;

                        spawnedTransformers.Add(spawnedTransformer);
                        spawnedTransformers.Add(spawnedTransformer1);
                    }
                }

                else
                {
                    GameObject spawnedTransformer = Instantiate(transformerPrefab, spawnPoint, path.GetRotationAtDistance(distanceSpawned));

                    Gate gate = spawnedTransformer.transform.GetChild(0).GetComponent<Gate>();
                    gate.isMoveToTargets = true;

                    int selectDir = Random.Range(0, 100);

                    if (selectDir > 50)
                    {
                        gate.gateAction = Gate.GateAction.good;
                        gate.gateGoodFormul = (Gate.GateGoodFormul)(Random.Range(1, 3));
                        gate.gateBadFormul = Gate.GateBadFormul.empty;
                        gate.actionCount = GateActionCount(Gate.GateAction.good, gate.gateGoodFormul, gate.gateBadFormul);
                    }

                    else
                    {
                        gate.gateAction = Gate.GateAction.bad;
                        gate.gateBadFormul = (Gate.GateBadFormul)(Random.Range(1, 3));
                        gate.gateGoodFormul = Gate.GateGoodFormul.empty;
                        gate.actionCount = GateActionCount(gate.gateAction, gate.gateGoodFormul, gate.gateBadFormul);
                    }

                    spawnedTransformers.Add(spawnedTransformer);
                }
                
            }

            distanceSpawned += groupBetwDistance;
            i++;
        }

        distanceSpawned = path.length - (arrowCount * arrowDistance);

        List<GameObject> spawneds = new List<GameObject>();

		for (int j = 0; j < arrowCount; j++)
		{
            Vector3 spawnPoint = path.GetPointAtDistance(distanceSpawned, EndOfPathInstruction.Stop);

            GameObject spawnedArrow = Instantiate(arrowPrefab, spawnPoint, path.GetRotationAtDistance(distanceSpawned));

            spawnedArrow.transform.Rotate(Vector3.up, 90);

            spawneds.Add(spawnedArrow);

            distanceSpawned += arrowDistance;
        }

        spawnedArrows = spawneds;
    }

    private void EndSpawn()
	{
        GameObject endGame = Instantiate(finalObj);
        endGame.transform.position = spawnedpathPoints[spawnedpathPoints.Count - 1].transform.position;
        endGame.transform.position += finalObjSpawnPos;

        EndGame endGameScr = endGame.transform.GetChild(0).GetComponent<EndGame>();
        character.endGame = endGameScr;
    }
}
