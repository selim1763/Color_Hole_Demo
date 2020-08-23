using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class HoleController : MonoBehaviour
{
    private GameManager gameManager;
    private Rigidbody rb;
    private int layerRayArea;

    private Vector3 lastWorldPosition = Vector3.zero;
    private bool isLastFramePositionViable = false;

    public bool HasAnyInputSinceLevelLoaded { get; private set; }


    void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        layerRayArea = LayerMask.GetMask("InvisibleRayArea");
    }

    private void OnEnable()
    {
        Lean.Touch.LeanTouch.OnFingerDown += HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUpdate += HandleFingerUpdate;
    }

    private void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerDown -= HandleFingerDown;
        Lean.Touch.LeanTouch.OnFingerUpdate -= HandleFingerUpdate;
    }

    private void HandleFingerDown(Lean.Touch.LeanFinger finger)
    {
        isLastFramePositionViable = false;

        if(!HasAnyInputSinceLevelLoaded)
        {
            gameManager.OnFirstInput();
            HasAnyInputSinceLevelLoaded = true;
        }
    }

    public void OnLevelLoad()
    {
        HasAnyInputSinceLevelLoaded = false;
    }

    private void HandleFingerUpdate(Lean.Touch.LeanFinger finger)
    {
        if(!gameManager.IsReady)
        {
            isLastFramePositionViable = false;
            return;
        }

        Vector3? hit = Utility.ScreenPointToWorldPoint(
                Camera.main,
                finger.ScreenPosition,
                layerRayArea
        );

        if (hit.HasValue)
        {
            Vector3 worldPosition = hit.Value;
            worldPosition.y = 0;

            MoveDelta(worldPosition);
        }

    }

    private void MoveDelta(Vector3 worldPosition)
    {
        if (isLastFramePositionViable)
        {
            Vector3 deltaPos = worldPosition - lastWorldPosition;
            Vector3 newPos = rb.position + deltaPos;

            rb.MovePosition(newPos);
        }

        lastWorldPosition = worldPosition;
        isLastFramePositionViable = true;
    }

}
