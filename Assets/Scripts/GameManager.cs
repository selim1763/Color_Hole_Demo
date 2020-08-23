using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Inspector
    [Header("Level Objects")]
    public SOLevel[] soLevels;
    public Transform bridgeObjectsParent;
    public Transform gateObject;

    [Header("Materials")]
    public Material matHoleDepth;
    public Material matInteractableObject;
    public Material matObstacle;
    public Material matGround;
    public Material matEdge;
    public Material matGate;

    [Header("Start Points")]
    public Transform stage1HoleStartPoint;
    public Transform stage2HoleStartPoint;
    public Transform stage1CameraStartPoint;
    public Transform stage2CameraStartPoint;

    [Header("Particles")]
    public ParticleSystem prtConfettiExplosion;
    // Inspector

    private UIManager uiManager;
    private HoleController holeController;

    private Transform currentLevelObject;
    private Transform currentBridgeObject;

    private Vector3 gateStartPosition;
    private int currentStage;
    private int totalLevelCount;
    private int objectCount;

    private int _CurrentLevel = 1;
    public int CurrentLevel
    {
        get { return _CurrentLevel; }
        private set { _CurrentLevel = value; }
    }

    public bool IsReady { get; private set; }


    void Start()
    {
        InitializeVariables();
        StartCoroutine(LoadCurrentLevel());
    }

    private void InitializeVariables()
    {
        uiManager = GetComponent<UIManager>();
        holeController = GameObject.FindWithTag("Hole").GetComponent<HoleController>();
        gateStartPosition = gateObject.position;
        totalLevelCount = soLevels.Length;

        IsReady = false;
    }


    private IEnumerator LoadCurrentLevel()
    {
        Debug.Assert(!IsReady);

        CleanUp();
        SetEntitiesToDefaultState();

        int levelIndex = (CurrentLevel - 1) % totalLevelCount;
        SOLevel levelObject = soLevels[levelIndex];

        SpawnLevelObjects(levelObject);
        AssignMaterialColors(levelObject);

        objectCount = currentLevelObject.Find("Stage1").childCount;
        currentStage = 1;

        yield return null;

        OnLevelLoad();
        IsReady = true;
    }

    private void OnLevelLoad()
    {
        holeController.OnLevelLoad();
        uiManager.ShowSwipeIcon();
    }

    private void CleanUp()
    {
        if (currentLevelObject != null && currentBridgeObject != null)
        {
            Destroy(currentLevelObject.gameObject);
            Destroy(currentBridgeObject.gameObject);
        }
    }

    private void SetEntitiesToDefaultState()
    {
        holeController.transform.position = stage1HoleStartPoint.position;
        Camera.main.transform.position = stage1CameraStartPoint.position;
        gateObject.position = gateStartPosition;
    }

    private void SpawnLevelObjects(SOLevel levelObject)
    {
        currentLevelObject = Instantiate(levelObject.levelPrefab);
        currentBridgeObject = Instantiate(bridgeObjectsParent);
        currentBridgeObject.gameObject.SetActive(true);
    }

    private void AssignMaterialColors(SOLevel levelObject)
    {
        matGround.color = levelObject.groundColor;
        matHoleDepth.color = levelObject.groundColor;
        matEdge.color = levelObject.edgeColor;
        matGate.color = levelObject.obstacleColor;
        matInteractableObject.color = levelObject.interactableObjectColor;
        matObstacle.color = levelObject.obstacleColor;
    }

    public void OnObjectFall(bool isObstacle)
    {
        if (!IsReady)
        {
            return;
        }

        if (isObstacle)
        {
            StartCoroutine(OnLevelFailed());
        }
        else
        {
            objectCount -= 1;

            if (objectCount <= 0)
            {
                StartCoroutine(OnStageComplete());
            }
        }
    }


    private IEnumerator OnStageComplete()
    {
        IsReady = false;

        if (currentStage == 1)
        {
            var cor = StartCoroutine(OnStageCompleted());
            yield return cor;

            IsReady = true;
        }
        else if (currentStage == 2)
        {
            StartCoroutine(OnLevelCompleted());
        }

    }

    private IEnumerator OnStageCompleted()
    {
        var cor = StartCoroutine(DoStageCompleteAnimations());
        yield return cor;

        objectCount = currentLevelObject.Find("Stage2").childCount;
        currentStage = 2;
    }

    private IEnumerator DoStageCompleteAnimations()
    {
        holeController.transform.DOMoveZ(stage2HoleStartPoint.position.z, 1.0f).SetEase(Ease.Flash);
        var tw = gateObject.transform.DOMoveY(0, 1.0f).SetEase(Ease.Flash);

        yield return tw.WaitForCompletion();

        tw = holeController.transform.DOMoveX(stage2HoleStartPoint.position.x, 3.0f).SetEase(Ease.Flash);

        yield return new WaitForSecondsRealtime(0.5f);

        Camera.main.transform.DOMoveX(stage2CameraStartPoint.position.x, 3.0f).SetEase(Ease.Flash);

        yield return tw.WaitForCompletion();
    }

    private IEnumerator OnLevelCompleted()
    {
        prtConfettiExplosion.Play();
        yield return new WaitForSecondsRealtime(2.0f);

        GoNextLevel();
    }

    private IEnumerator OnLevelFailed()
    {
        IsReady = false;
        yield return new WaitForSeconds(0.8f);

        var tw = Camera.main.transform.DOShakePosition(1.75f, 0.75f, 10, 0);
        yield return tw.WaitForCompletion();

        StartCoroutine(LoadCurrentLevel());
    }

    private void GoNextLevel()
    {
        CurrentLevel += 1;
        StartCoroutine(LoadCurrentLevel());
    }

    public void OnFirstInput()
    {
        uiManager.HideSwipeIcon();
    }
}
