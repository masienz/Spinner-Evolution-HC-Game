using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MoreMountains.NiceVibrations;
using Facebook.Unity;
using GameAnalyticsSDK;

public class MainCharacter : MonoBehaviour
{
    /*public enum PartSection
    {
        Top,
        Mid,
        Bottom,
        Additional
    }*/

    [System.Serializable]
    public struct Skin
    {
        public GameObject skinHead;
        public float goldCount;
    }

    [System.Serializable]
    public struct Bonus
    {
        public float bonusCount;

        [Range(0, 100)] public float percentCount;
    }

    [System.Serializable]
    public struct PartGroup
    {
        public GameObject gameObject;
        public float actionCount;
    }

    public EndGame endGame;
    public MapManager manager;
    public Settings settings;
    public GameObject beybladeObj;
    public GameObject beybladeParentObj;
    public GameObject targetRotObj;
    public GameObject camContNormal;
    public GameObject camContEnd;
    public bool isStart = false;
    public bool canMove;
    public bool isHitWall;
    public bool isDead = false;
    public bool isVibrate = false;
    public bool isSoundEffect = false;

    public float score = 0;

    [Header("Beyblade Feature")]
    [SerializeField] private float power = 10f;
    [SerializeField] private float currentScale = 1f;
    [SerializeField] private float startScale = 1f;

    [Header("Beyblade Changing Counts")]
    [SerializeField] private float powerChangeCount = 5f;
    //[SerializeField] private float speedIncrease = 2f;
    [SerializeField] private float scaleIncrease = 0.1f;
    [SerializeField] private float rotateSpeedIncrease = 100f;
    [SerializeField] private float scaleAnimDuration = 1.5f;
    [SerializeField] private float scaleAnimIncrease = 0.5f;
    [SerializeField] private Ease scaleAnim;
    [SerializeField] private int minPowerIncreaseOnHitEnemy = 2;
    [SerializeField] private int maxPowerIncreaseOnHitEnemy = 6;
    [SerializeField] private int minPowerDecreaseOnHitObstacle = 2;
    [SerializeField] private int maxPowerDecreaseOnHitObstacle = 6;

    [Header("Gold")]
    [SerializeField] private float goldCount = 0;
    [SerializeField] private int mingoldIncreaseCount = 2;
    [SerializeField] private int maxgoldIncreaseCount = 20;

    [Header("Part Parents")]
    [SerializeField] private List<GameObject> topMainParts;
    [SerializeField] private List<PartGroup> levelParts;
    public List<Skin> skins;
    public Skin currentSkin;

    [Header("Beyblade Move")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float speedWithFever = 8f;
    [SerializeField] private float startTurningSpeed = 800;
    [SerializeField] private float lerpSpeed = 4f;
    [SerializeField] private float sideLerpSpeed = 800f;
    [SerializeField] private float sideSpeed = 1000f;
    [SerializeField] private float swerveRotateLerpSpeed = 10f;
    [SerializeField] private float hitDuration = 1f;
    [SerializeField] private float maxIntervalPos = 2.1f;
    [SerializeField] private Ease hitAnim;

    [Header("Beyblade Hit Walls")]
    [SerializeField] private float wallForce = 1f;
    [SerializeField] private float wallDistance = 0.5f;
    [SerializeField] private float loseControlTime = 0.5f;

    [Header("Beyblade Fever")]
    [SerializeField] private int feverCount;
    [SerializeField] private int feverTime;
    [SerializeField] private float feverBarAnimTime = 1;
    [SerializeField] private Ease feverBarAnim;
    [SerializeField] private int feverIncreaseByGateCount = 30;
    [SerializeField] private int feverIncreaseByEnemy = 5;

    [Header("Beyblade Effects")]
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private GameObject speedTrailDefaultEffect;
    [SerializeField] private List<GameObject> speedTrailEffects;

    [Header("Beyblade End Values")]
    [SerializeField] private List<Bonus> endBonusses;
    public float endStartValue = 50;
    [SerializeField] private float bonusDecreaseSpeed;
    [SerializeField] private float bonusIncreaseOnTouch = 15;
    [SerializeField] private float maxArrowRotate;
    [SerializeField] private float minArrowRotate;
    [SerializeField] private float lerpSpeedEnd = 2f;
    [SerializeField] private float endGrowScale = 2f;
    [SerializeField] private float endGrowDuration = 0.5f;
    [SerializeField] private Ease endGrowEase;
    [SerializeField] private ParticleSystem endParticleObj;

    [Header("Beyblade Dead")]
    [SerializeField] private float DestroyTime = 3;
    [SerializeField] private float randomForceMin = 3;
    [SerializeField] private float randomForceMax = 5;
    [SerializeField] private float randomTorqueMax = 2;

    [Header("Beyblade Upgrade")]
    [SerializeField] private int upgradePowerMoneyIncrease = 50;
    [SerializeField] private int upgradePowerIncrease = 5;
    [SerializeField] private float upgradeMoneyFactorIncrease = 0.1f;
    [SerializeField] private int upgradeMoneyIncrease = 50;

    [Header("Beyblade UI")]
    [SerializeField] private SpriteRenderer powerBGImage;
    [SerializeField] private GameObject endPowerFactorPanel;
    [SerializeField] private Slider endPowerFactorSlider;
    [SerializeField] private RectTransform arrow;
    [SerializeField] private TextMeshPro powerText;
    [SerializeField] private Slider feverBar;
    [SerializeField] private GameObject endLevelPanel;
    [SerializeField] private GameObject startLevelPanel;
    [SerializeField] private GameObject endLosePanel;
    [SerializeField] private GameObject goldParent;
    [SerializeField] private Text goldText;
    [SerializeField] private Text goldTextEndPanel;
    [SerializeField] private Text goldTextEndLosePanel;
    [SerializeField] private Text powerUpgradeLevelText;
    [SerializeField] private Text powerUpgradeMoneyText;
    [SerializeField] private Text moneyUpgradeLevelText;
    [SerializeField] private Text moneyUpgradeMoneyText;
    [SerializeField] private Slider levelSlider;
    [SerializeField] private Text currentLevelText;
    [SerializeField] private Text afterLevelText;
    [SerializeField] private GameObject endTapFast;

    [Header("Beyblade Sound")]
    [SerializeField] private AudioSource bonusGate;
    [SerializeField] private AudioSource reduceGate;
    [SerializeField] private AudioSource loseSound;
    [SerializeField] private List<AudioSource> winSounds;

    private float distanceTravelled;
    private float lastSidePoint;
    private float newSidePoint;
    private float beybladeTurningSpeed = 800;
    private float startSpeed;
    private float turningCount = 0;
    private float rotZ = 0;
    private float startPower = 10;
    private float moneyFactor = 0.1f;
    private float takingGoldThisLevel = 0;

	[Header("Levels")]
    [SerializeField] private int currentLevel = 0;
    private int currentLevelPart = 0;
    private int currentFeverCount = 0;
    [SerializeField] private int powerUpgradeLevel = 0;
    [SerializeField] private int moneyUpgradeLevel = 0;

    private bool isHitting = false;
    private bool isFevering = false;
    private bool isEnding = false;
    private bool endHit = false;

    private GameObject currentObj = null;

    private GameObject endGrowObj = null;
    private GameObject endChangeMatObj = null;

    private int CalculateGold()
    {
        int gold = 0;

        gold = (int)(Random.Range(mingoldIncreaseCount, maxgoldIncreaseCount) * moneyFactor);

        return gold;
    }

    private void Awake()
    {
        camContNormal.SetActive(true);
        camContEnd.SetActive(false);

        takingGoldThisLevel = 0;
        startPower = 10;
        currentLevel = 0;
        powerBGImage.color = new Color(0.2877358f, 0.8512698f, 1, 1);

        Load();

        CheckMoneyUpgrade();
        CheckPowerUpgrade();

        isStart = false;
        startLevelPanel.SetActive(true);
        endLevelPanel.SetActive(false);
        endLosePanel.SetActive(false);
        endTapFast.SetActive(false);
        feverBar.value = 0;
        currentLevelPart = 0;

        startSpeed = speed;
        beybladeTurningSpeed = startTurningSpeed;
        canMove = true;
        currentScale = startScale;
        beybladeObj.transform.localScale = Vector3.one * currentScale;

        //CheckListsQueue();
        StartCoroutine(MainGame());
        ChangeParts();
        endPowerFactorPanel.SetActive(false);
        isHitting = false;

        for (int i = 0; i < speedTrailEffects.Count; i++)
        {
            speedTrailEffects[i].SetActive(false);
        }

        speedTrailDefaultEffect.SetActive(true);

        distanceTravelled = 10;

        transform.position = manager.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
        GameAnalytics.Initialize();

        if (FB.IsInitialized) // Fb sdk initilaze
            FB.ActivateApp();
        else
            FB.Init(() => { FB.ActivateApp(); });
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            if (FB.IsInitialized)
                FB.ActivateApp();
            else
                FB.Init(() => { FB.ActivateApp(); });
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        //Beyblade Progress
        if (!isEnding) levelSlider.value = distanceTravelled / manager.path.length;
        else levelSlider.value = 1;
        currentLevelText.text = currentLevel.ToString();
        afterLevelText.text = (currentLevel + 1).ToString();

        powerText.text = ((int)power).ToString();

        if (!isStart) return;

        //Beyblade Turning
        beybladeObj.transform.Rotate(beybladeParentObj.transform.up, beybladeTurningSpeed * Time.deltaTime);
        beybladeObj.transform.localEulerAngles = new Vector3(0, beybladeObj.transform.localEulerAngles.y, 0);

        if (distanceTravelled > manager.path.length - endStartValue - 10 && !isEnding)
		{
            StopAllCoroutines();
            StartCoroutine(EndGameFactor());
        }
    }

    private IEnumerator MainGame()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (!isStart) continue;

            if (isDead) break;

            Moving();
        }
    }

    private IEnumerator EndGameFactor()
    {
        isFevering = false;
        isEnding = true;

        beybladeParentObj.transform.rotation = Quaternion.Lerp(beybladeParentObj.transform.rotation, Quaternion.Euler(7, 0, 0), swerveRotateLerpSpeed * Time.deltaTime);

        for (int i = 0; i < speedTrailEffects.Count; i++)
        {
            speedTrailEffects[i].SetActive(false);
        }

        camContNormal.SetActive(false);
        camContEnd.SetActive(true);

        endPowerFactorPanel.SetActive(true);

        feverBar.gameObject.SetActive(false);

        endTapFast.SetActive(true);

        float curBonus = 0;

        lastSidePoint = 0;

        //yield return new WaitForSeconds(0.5f);

        // Choose power factor
        while (true)
        {
            yield return new WaitForFixedUpdate();

            curBonus -= Time.deltaTime * bonusDecreaseSpeed;

            if (Input.touchCount != 0 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
                curBonus += bonusIncreaseOnTouch;

                endTapFast.SetActive(false);
            }

            curBonus = Mathf.Clamp(curBonus, 0, 100);

            endPowerFactorSlider.value = curBonus / 100;

            if (distanceTravelled > manager.path.length - 10)
			{
                for (int i = 0; i < endBonusses.Count; i++)
                {
                    if (curBonus <= endBonusses[i].percentCount)
                    {
                        endPowerFactorPanel.SetActive(false);

                        power += endBonusses[i].bonusCount;
                        ChangePart(Gate.GateAction.good);
                        StartCoroutine(ChangeScaleAnim(Vector3.one * currentScale));

                        break;
                    }
                }

                break;
            }

            distanceTravelled += speed * Time.deltaTime;

            Vector3 currentPathPointPos = manager.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
            Vector3 pos1 = currentPathPointPos + transform.right * lastSidePoint;
            Vector3 pos2 = Vector3.Lerp(transform.position, pos1, lerpSpeed * Time.deltaTime);
            transform.position = new Vector3(pos2.x, 0.1f, pos2.z);
        }

        beybladeObj.transform.localEulerAngles = new Vector3(7, beybladeObj.transform.localEulerAngles.y, 0);

        yield return new WaitForSeconds(scaleAnimDuration);

        int queueCount = 0;
        float endFactor = 1f;

        int currentPathPoint = 0;

        // End Game Animation

        while (true)
        {
            yield return new WaitForFixedUpdate();
            
            if ((currentObj == null || currentObj.GetComponent<EnemyBeyblade>().isDead) && !endHit)
            {
                if (endGame.enemyBeyblades.Count - 1 > queueCount)
                {
                    if (endGame.enemyBeyblades[queueCount].power < power)
                    {
                        currentObj = endGame.enemyBeyblades[queueCount].beybladeObj;
                        if (endGame.enemyBeyblades[queueCount].growObj != null) endGrowObj = endGame.enemyBeyblades[queueCount].growObj;
                        if (endGame.enemyBeyblades[queueCount].changedMatObj != null) endChangeMatObj = endGame.enemyBeyblades[queueCount].changedMatObj;
                        if (endChangeMatObj != null)
                        {
                            ParticleSystem spawnedEffect = Instantiate(endParticleObj);
                            spawnedEffect.transform.position = endChangeMatObj.transform.position;
                            ParticleSystem spawnedEffect1 = Instantiate(endParticleObj);
                            spawnedEffect1.transform.position = endChangeMatObj.transform.GetChild(0).position;
                        }
                        queueCount++;
                    }
                    else
                    {
                        endFactor = endGame.enemyBeyblades[queueCount].factor;
                        break;
                    }
                }
                else if (endGame.enemyBeyblades.Count - 1 == queueCount)
				{
                    currentObj = endGame.enemyBeyblades[queueCount].beybladeObj;
                    if(endGame.enemyBeyblades[queueCount].growObj != null) endGrowObj = endGame.enemyBeyblades[queueCount].growObj;
                    if (endGame.enemyBeyblades[queueCount].changedMatObj != null) endChangeMatObj = endGame.enemyBeyblades[queueCount].changedMatObj;
                    if (endChangeMatObj != null)
                    {
                        ParticleSystem spawnedEffect = Instantiate(endParticleObj);
                        spawnedEffect.transform.position = endChangeMatObj.transform.position;
                        ParticleSystem spawnedEffect1 = Instantiate(endParticleObj);
                        spawnedEffect1.transform.position = endChangeMatObj.transform.GetChild(0).position;
                    }
                    endFactor = endGame.enemyBeyblades[queueCount].factor;
                }
                else
                {
                    break;
                }
            }

            else if (endGame.enemyBeyblades.Count - 1 <= queueCount)
			{
                Vector3 targetPos = endGame.endPathPoints[currentPathPoint].transform.position;

                if (Vector3.Distance(transform.position, targetPos) < 0.3f)
				{
                    currentPathPoint++;
                    if (endGame.endPathPoints.Count <= currentPathPoint)
					{
                        break;
                    }
                }

                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                if (!isHitting && !endHit) beybladeParentObj.transform.rotation = Quaternion.Lerp(beybladeParentObj.transform.rotation, Quaternion.Euler(13, 0, 0), swerveRotateLerpSpeed * Time.deltaTime);
                else if (endHit)
				{
                    beybladeParentObj.transform.rotation = Quaternion.Lerp(beybladeParentObj.transform.rotation, Quaternion.Euler(0, 0, 0), swerveRotateLerpSpeed * Time.deltaTime);
                }
            }

            else transform.position = Vector3.Lerp(transform.position, currentObj.transform.position, lerpSpeedEnd * Time.deltaTime);
        }

        for (int i = 0; i < endGame.endEffects.Count; i++)
        {
            endGame.endEffects[i].Play();
        }

        yield return new WaitForSeconds(1.5f);

        goldTextEndPanel.text = "+" + ((int)takingGoldThisLevel).ToString();

        currentLevel++;

        endLevelPanel.SetActive(true);

        PlaySound(winSounds[0]);

        PlaySound(winSounds[1]);

        Vibrate();
    }

    private void CheckFever()
    {
        if (isEnding) return;

        if (feverBar.value >= 1.0f)
        {
            StartCoroutine(Fever());
        }

        //Fever bar change
    }

    private IEnumerator Fever()
    {
        isFevering = true;
        currentFeverCount = 0;

        for (int i = 0; i < speedTrailEffects.Count; i++)
        {
            speedTrailEffects[i].SetActive(true);
        }

        speedTrailDefaultEffect.SetActive(false);

        yield return new WaitForSeconds(feverTime);

        isFevering = false;
        feverBar.DOValue(0, feverBarAnimTime).SetEase(feverBarAnim);

        for (int i = 0; i < speedTrailEffects.Count; i++)
        {
            speedTrailEffects[i].SetActive(false);
        }

        speedTrailDefaultEffect.SetActive(true);
    }

    private void FeverBar(float count)
    {
        float lastValue = feverBar.value;

        float endValue = lastValue + (count / 100);

        if (count > 0) feverBar.transform.DOShakePosition(1, 50, 10, 90, false, true);
        else feverBar.transform.DOShakePosition(1, 20, 10, 90, false, true);

        feverBar.DOValue(endValue, feverBarAnimTime).SetEase(feverBarAnim);
    }

    private void Moving()
    {
        //Side control
        if (Input.touchCount != 0 && canMove)
        {
            var touch = Input.GetTouch(0);

            float x = touch.deltaPosition.x / Screen.width;

            if (touch.phase == TouchPhase.Moved)
            {
                newSidePoint += x * sideSpeed * Time.deltaTime;
                lastSidePoint = Mathf.Lerp(lastSidePoint, newSidePoint, sideLerpSpeed * Time.deltaTime);

                if (x > 0) rotZ = -10;
                else rotZ = 10;

                if (isHitWall)
                {
                    if (lastSidePoint >= maxIntervalPos - wallDistance && lastSidePoint > 0)
                    {
                        lastSidePoint -= wallForce;
                        rotZ = 10;

                        Debug.Log($"Last Side Point : {lastSidePoint} / Formul Result {maxIntervalPos - wallDistance} / Right Wall ");

                        var effect = Instantiate(hitEffect);

                        effect.transform.position = transform.position + Vector3.right + Vector3.up;

                        effect.Play();

                        Destroy(effect.gameObject, 1f);

                        StartCoroutine(LoseControl());
                    }
                    else if (lastSidePoint <= -maxIntervalPos + wallDistance && lastSidePoint < 0)
                    {
                        lastSidePoint += wallForce;
                        rotZ = -10;

                        Debug.Log($"Last Side Point : {lastSidePoint} / Formul Result {-maxIntervalPos + wallDistance} / Left Wall");

                        var effect = Instantiate(hitEffect);

                        effect.transform.position = transform.position + Vector3.left + Vector3.up;

                        effect.Play();

                        Destroy(effect.gameObject, 1f);

                        StartCoroutine(LoseControl());
                    }
                }

                lastSidePoint = Mathf.Clamp(lastSidePoint, -maxIntervalPos, maxIntervalPos);
                newSidePoint = Mathf.Clamp(lastSidePoint, -maxIntervalPos, maxIntervalPos);
            }
        }
        else if (canMove)
        {
            rotZ = 0;
        }

        //Position
        if(!isFevering) distanceTravelled += speed * Time.deltaTime;
        else distanceTravelled += speedWithFever * Time.deltaTime;

        Vector3 currentPathPoint = manager.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction.Stop);
        Vector3 pos1 = currentPathPoint + transform.right * lastSidePoint;
        Vector3 pos2 = Vector3.Lerp(transform.position, pos1, lerpSpeed * Time.deltaTime);
        transform.position = new Vector3(pos2.x, 0.1f, pos2.z);

        //Beyblade Swerve
        if (!isHitting)
        {
            beybladeParentObj.transform.rotation = Quaternion.Lerp(beybladeParentObj.transform.rotation, Quaternion.Euler(7, 0, rotZ), swerveRotateLerpSpeed * Time.deltaTime);
        }
    }

    private IEnumerator LoseControl()
    {
        canMove = false;

        yield return new WaitForSeconds(loseControlTime);

        rotZ = 0;

        canMove = true;
    }

    private void ChangePart(Gate.GateAction gateAction)
    {
        if (gateAction == Gate.GateAction.good)
        {
            ChangeParts();

            //Changing Effects

            //Changing Scale
            currentScale += scaleIncrease;
            beybladeTurningSpeed += rotateSpeedIncrease;
        }
        else if (gateAction == Gate.GateAction.bad)
        {
            ChangeParts();

            //Changing Effects

            //Changing Scale
            currentScale -= scaleIncrease;
            beybladeTurningSpeed -= rotateSpeedIncrease;
        }
        StartCoroutine(ChangeScaleAnim(Vector3.one * currentScale));
    }

    private IEnumerator ChangeScaleAnim(Vector3 targetScale)
    {
        Tween tween = beybladeParentObj.transform.DOScale(targetScale + scaleAnimIncrease * Vector3.one, scaleAnimDuration / 2).SetEase(scaleAnim);

        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (!tween.IsPlaying())
            {
                beybladeParentObj.transform.DOScale(targetScale, scaleAnimDuration / 2).SetEase(scaleAnim);
                break;
            }
        }
    }

    private void ChangeParts()
    {
        for (int i = 0; i < levelParts.Count; i++)
        {
            levelParts[i].gameObject.SetActive(false);
        }

        for (int i = levelParts.Count - 1; i > -1; i--)
        {
            if (levelParts[i].actionCount < power)
            {
                levelParts[i].gameObject.SetActive(true);
                currentLevelPart = i;
                break;
            }
        }
    }

    /*private float CalculateScore()
    {
        float calculatedScore = 0;
        float startScore = 5;

        calculatedScore += Random.Range(power, power * 2) * startScore;

        return calculatedScore;
    }*/

    private IEnumerator HitBeyblade(GameObject col)
    {
        float rotZ = 0;
        float rotX = 0;
        isHitting = true;

        if (Mathf.Abs(col.transform.position.x - transform.position.x) <= 0.5f)
        {
            rotZ = 0;
            rotX = 15;
        }
        else if (col.transform.position.x > transform.position.x)
        {
            rotZ = -15;
            rotX = 15;
        }
        else
        {
            rotZ = 15;
            rotX = 15;
        }

        Tween tween = beybladeParentObj.transform.DORotate(new Vector3(rotX, 0, rotZ), hitDuration).SetEase(hitAnim);
        Tween tween2;

        while (true)
        {
            yield return new WaitForFixedUpdate();

            isHitting = true;

            if (!tween.IsPlaying())
            {
                tween2 = beybladeParentObj.transform.DORotate(new Vector3(rotX - rotX * 3 / 2, 0, rotZ - rotZ * 3 / 2), hitDuration / 2).SetEase(hitAnim);

                break;
            }
        }

        while (true)
        {
            yield return new WaitForFixedUpdate();

            isHitting = true;

            if (!tween2.IsPlaying())
            {
                beybladeParentObj.transform.DORotate(new Vector3(0, 0, 0), hitDuration).SetEase(hitAnim);

                break;
            }
        }


        isHitting = false;
    }

    private IEnumerator HitObstacle(GameObject col)
    {
        float rotZ = 0;
        float rotX = 0;
        isHitting = true;

        if (Mathf.Abs(col.transform.position.x - transform.position.x) <= 0.5f)
        {
            rotZ = 0;
            rotX = 15;
            distanceTravelled -= 10;
        }
        else if (col.transform.position.x > transform.position.x)
        {
            rotZ = -15;
            rotX = 15;
            distanceTravelled -= 10;
            newSidePoint -= 3;
            lastSidePoint -= 3;
        }
        else
        {
            rotZ = 15;
            rotX = 15;
            distanceTravelled -= 10;
            newSidePoint += 3;
            lastSidePoint += 3;
        }

        Tween tween2 = beybladeParentObj.transform.DORotate(new Vector3(-rotX / 2, 0, -rotZ / 2), hitDuration / 2).SetEase(hitAnim);

        

        while (true)
        {
            yield return new WaitForFixedUpdate();

            isHitting = true;

            if (!tween2.IsPlaying())
            {
                beybladeParentObj.transform.DORotate(new Vector3(0, 0, 0), hitDuration).SetEase(hitAnim);

                break;
            }
        }


        isHitting = false;
    }

    public void LoadNextLevel()
    {
        Save();
        SceneManager.LoadSceneAsync("MainGame");
    }

    public void StartGame()
    {
        isStart = true;
        startLevelPanel.SetActive(false);
    }

    private void Crash()
    {
        isDead = true;

        endLosePanel.SetActive(true);

        PlaySound(loseSound);

        Vibrate();

        List<GameObject> parts = new List<GameObject>();

        for (int i = 0; i < levelParts[currentLevelPart].gameObject.transform.childCount; i++)
        {
            parts.Add(levelParts[currentLevelPart].gameObject.transform.GetChild(i).gameObject);
        }

        foreach (var part in parts)
        {
            Rigidbody partRigid = part.GetComponent<Rigidbody>();
            partRigid.constraints = RigidbodyConstraints.None;

            int isaret = Random.Range(-1, 1);

            if (isaret == 0) partRigid.AddForce(Random.Range(randomForceMin, randomForceMax) * transform.forward + transform.up * Random.Range(randomForceMin, randomForceMax) + transform.right * Random.Range(randomForceMin, randomForceMax));
            else partRigid.AddForce(Random.Range(randomForceMin, randomForceMax) * transform.forward + transform.up * Random.Range(randomForceMin, randomForceMax) + transform.right * Random.Range(randomForceMin, randomForceMax) * -1);

            partRigid.AddTorque(Vector3.one * Random.Range(0, randomTorqueMax));

            Destroy(part, DestroyTime);
        }

        if(!isEnding) StopAllCoroutines();

        goldTextEndLosePanel.text = "+" + ((int)takingGoldThisLevel).ToString();
    }

    private void TakeGold(out int calculated)
    {
        int thisGold = CalculateGold();

        goldCount += thisGold;

        takingGoldThisLevel += thisGold;

        goldText.text = ((int)goldCount).ToString();

        goldParent.transform.DOShakePosition(1, 30, 10, 90, false, true);

        /*goldText.faceColor = Color.green;

        yield return new WaitForSeconds(0.2f);

        goldText.faceColor = Color.white;*/

        calculated = thisGold;
    }

    private void CheckPowerUpgrade()
    {
        int needMoney = 100 + (powerUpgradeLevel + 1) * upgradePowerMoneyIncrease;

        powerUpgradeLevelText.text = "Level " + (powerUpgradeLevel + 1).ToString();

        powerUpgradeMoneyText.text = needMoney.ToString();

        if (goldCount <= needMoney)
        {
            powerUpgradeLevelText.transform.parent.GetComponent<Button>().interactable = false;
        }
        else
        {
            powerUpgradeLevelText.transform.parent.GetComponent<Button>().interactable = true;
        }

        startPower = 10 + (powerUpgradeLevel) * upgradePowerIncrease;
        power = startPower;
    }

    public void UpgradePower()
    {
        int needMoney = 100 + (powerUpgradeLevel + 1) * upgradePowerMoneyIncrease;

        if (goldCount >= needMoney)
        {
            goldCount -= needMoney;
            startPower = 10 + (powerUpgradeLevel) * upgradePowerIncrease;
            power = startPower;
            StartCoroutine(PowerUp());
            powerUpgradeLevel++;

            goldText.text = ((int)goldCount).ToString();

            goldParent.transform.DOShakePosition(1, 30, 10, 90, false, true);

            CheckPowerUpgrade();
        }
    }

    private void CheckMoneyUpgrade()
    {
        int needMoney = 100 + (moneyUpgradeLevel + 1) * upgradeMoneyIncrease;

        moneyUpgradeLevelText.text = "Level " + (moneyUpgradeLevel + 1).ToString();

        moneyUpgradeMoneyText.text = needMoney.ToString();

        if (goldCount <= needMoney)
        {
            moneyUpgradeLevelText.transform.parent.GetComponent<Button>().interactable = false;
        }
        else
        {
            moneyUpgradeLevelText.transform.parent.GetComponent<Button>().interactable = true;
        }

        moneyFactor = 1f + (moneyUpgradeLevel) * upgradeMoneyFactorIncrease;
    }

    public void UpgradeMoney()
    {
        int needMoney = 100 + (moneyUpgradeLevel + 1) * upgradeMoneyIncrease;

        if (goldCount >= needMoney)
        {
            goldCount -= needMoney;
            moneyFactor = 1f + (moneyUpgradeLevel) * upgradeMoneyFactorIncrease;
            moneyUpgradeLevel++;

            goldText.text = ((int)goldCount).ToString();

            goldParent.transform.DOShakePosition(1, 30, 10, 90, false, true);

            CheckMoneyUpgrade();
        }
    }

    private IEnumerator PowerUp()
    {
        powerBGImage.color = new Color(0, 1, 0, 1);

        yield return new WaitForSeconds(0.2f);

        powerBGImage.color = new Color(0.2877358f, 0.8512698f, 1, 1);
    }

    private IEnumerator PowerDown()
    {
        powerBGImage.color = new Color(1, 0, 0, 1);

        yield return new WaitForSeconds(0.2f);

        powerBGImage.color = new Color(0.2877358f, 0.8512698f, 1, 1);
    }

    private void PlaySound(AudioSource audioSource)
	{
        if (!isSoundEffect) return;

        audioSource.Stop();
        audioSource.Play();
	}

    private void Vibrate()
	{
        if (!isVibrate) return;

        MMVibrationManager.TransientHaptic(0.85f, 0.05f, true, this);
	}

    private void OnTriggerEnter(Collider col)
    {
        if (isDead) return;

        if (col.CompareTag("TransformGate") && col.GetComponent<Gate>())
        {
            Gate gate = col.GetComponent<Gate>();

            if (gate.gateAction == Gate.GateAction.good)
            {
                //Power Up
                if (gate.gateGoodFormul == Gate.GateGoodFormul.plus) power += gate.actionCount;
                else if (gate.gateGoodFormul == Gate.GateGoodFormul.multiply) power *= gate.actionCount;

                StartCoroutine(PowerUp());

                if (!isFevering)
                {
                    currentFeverCount++;
                    CheckFever();
                    FeverBar(feverIncreaseByGateCount);
                }

                PlaySound(bonusGate);
                Vibrate();
            }
            else if (gate.gateAction == Gate.GateAction.bad)
            {
                //Power Down
                if (gate.gateBadFormul == Gate.GateBadFormul.minus) power -= gate.actionCount;
                else if (gate.gateBadFormul == Gate.GateBadFormul.divided) power /= gate.actionCount;

                StartCoroutine(PowerDown());

                if (power < 1)
                {
                    Crash();
                }

                if (!isFevering && !isDead)
                {
                    currentFeverCount = 0;
                    CheckFever();
                    FeverBar(-feverIncreaseByGateCount);
                }

                PlaySound(reduceGate);
                Vibrate();
            }

            //Change part
            if(!isDead) ChangePart(gate.gateAction);
        }

        if (col.CompareTag("EnemyBeyblade") && !col.GetComponent<EnemyBeyblade>().isDead)
        {
            if (!isFevering) StartCoroutine(HitBeyblade(col.gameObject));

            if (!isFevering && !isDead)
            {
                currentFeverCount = 0;
                CheckFever();
                FeverBar(feverIncreaseByEnemy);
            }

            int calculated = 0;

            TakeGold(out calculated);

            col.GetComponent<EnemyBeyblade>().Crash();

            //col.GetComponent<EnemyBeyblade>().Score(calculated);

            //Debug.Log((int)calulated);

            score += calculated;

            if (col.GetComponent<EnemyBeyblade>().type == EnemyBeyblade.BeybladeType.normal || isFevering)
            {
                power += Random.Range(minPowerIncreaseOnHitEnemy, maxPowerIncreaseOnHitEnemy);
                if(!isEnding) StartCoroutine(PowerUp());
            }
            else
            {
                power -= Random.Range(minPowerIncreaseOnHitEnemy, maxPowerIncreaseOnHitEnemy);
                if (!isEnding) StartCoroutine(PowerDown());
                StartCoroutine(HitObstacle(col.gameObject));
            }

            var effect = Instantiate(hitEffect);

            effect.transform.position = ((col.transform.position - transform.position) / 2) + transform.position + Vector3.up;

            effect.Play();

            Destroy(effect.gameObject, 1f);

            if (power < 1)
            {
                Crash();
            }

            if (col.gameObject == endGame.enemyBeyblades[endGame.enemyBeyblades.Count - 1].beybladeObj) endHit = true;

            if (endGrowObj != null) endGrowObj.transform.DOScale(endGrowObj.transform.localScale * endGrowScale, endGrowDuration).SetEase(endGrowEase);
            if (endChangeMatObj != null)
			{
                endChangeMatObj.GetComponent<ChangeColorSmooth>().ChangeColorSmoothFunc();
                endChangeMatObj.transform.GetChild(0).GetComponent<ChangeColorSmooth>().ChangeColorSmoothFunc();
            }
            if (endParticleObj != null) endParticleObj.Play();
        }

        if (col.CompareTag("Obstacle"))
        {
            if (!isFevering)
            {
                StartCoroutine(HitObstacle(col.gameObject));
                col.GetComponent<BoxCollider>().enabled = false;
                power -= Random.Range(minPowerDecreaseOnHitObstacle, maxPowerDecreaseOnHitObstacle);
                StartCoroutine(PowerDown());
                Destroy(col.gameObject, 0.5f);

                if (power < 1)
                {
                    Crash();
                }
            }
        }
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("Gold", goldCount);
        PlayerPrefs.SetInt("PowerUpgradeLevel", powerUpgradeLevel);
        PlayerPrefs.SetInt("MoneyUpgradeLevel", moneyUpgradeLevel);
        PlayerPrefs.SetInt("Level", currentLevel);


        if (isSoundEffect) PlayerPrefs.SetInt("Sound", 1);
        else PlayerPrefs.SetInt("Sound", 0);

        if (isVibrate) PlayerPrefs.SetInt("Vibrate", 1);
        else PlayerPrefs.SetInt("Vibrate", 0);
    }

    private void Load()
    {
        if(PlayerPrefs.HasKey("Gold")) goldCount = PlayerPrefs.GetFloat("Gold");
        if (PlayerPrefs.HasKey("PowerUpgradeLevel")) powerUpgradeLevel = PlayerPrefs.GetInt("PowerUpgradeLevel");
        if (PlayerPrefs.HasKey("MoneyUpgradeLevel")) moneyUpgradeLevel = PlayerPrefs.GetInt("MoneyUpgradeLevel");
        if (PlayerPrefs.HasKey("Level")) currentLevel = PlayerPrefs.GetInt("Level");

        goldText.text = ((int)goldCount).ToString();

        if (PlayerPrefs.HasKey("Vibrate"))
		{
            if (PlayerPrefs.GetInt("Vibrate") == 1)
            {
                isVibrate = true;
                settings.vibrate.GetComponent<Image>().sprite = settings.vibrateOn;
                settings.isOpenVibrate = true;
            }
            else
            {
                isVibrate = false;
                settings.vibrate.GetComponent<Image>().sprite = settings.vibrateOff;
                settings.isOpenVibrate = false;
            }
        }
        else
		{
            isVibrate = true;
            settings.vibrate.GetComponent<Image>().sprite = settings.vibrateOn;
            settings.isOpenVibrate = true;
        }
            
    }

    [ContextMenu("DeleteSave")]
    public void DeleteSave()
    {
        PlayerPrefs.DeleteAll();
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}
