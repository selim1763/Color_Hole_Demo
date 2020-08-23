using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectHandler : MonoBehaviour
{
    public enum State
    {
        Idle,
        Falling
    }

    private State state = State.Idle;

    private GameManager gameManager;
    private Transform holeCenterPoint;
    private Rigidbody rb;

    private bool isObstacle;
    private bool isAlive;

    private int layerFallingObject;
    private int layerDontRender;
    

    private void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        holeCenterPoint = GameObject.FindWithTag("HoleCenterPoint").transform;
        rb = GetComponent<Rigidbody>();

        layerFallingObject = LayerMask.NameToLayer("FallingObject");
        layerDontRender = LayerMask.NameToLayer("DoNotRender");

        isObstacle = gameObject.tag == "Obstacle";
        isAlive = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "HoleTrigger")
        {
            OnForceField();
        }
    }

    public void OnForceField()
    {
        if(!gameManager.IsReady && isObstacle)
        {
            return;
        }

        Vector3 dir = (holeCenterPoint.position - rb.position).normalized;
        float diff = Vector3.Distance(holeCenterPoint.position, rb.position);

        rb.AddForce(dir, ForceMode.Impulse);

        if (diff < 1.5f)
        {
            rb.AddForce(dir * 5f, ForceMode.VelocityChange);
            gameObject.layer = layerFallingObject;
            state = State.Falling;
            
            gameManager.OnObjectFall(isObstacle);
        }
    }

    private void Update()
    {
        if(!isAlive)
        {
            return;
        }

        StateUpdate();
    }

    private void StateUpdate()
    {
        if(state == State.Falling)
        {
            bool hasFallen = CheckHasFallen();
            if (hasFallen)
            {
                OnBecomeFallen();
            }
        }
    }

    private void OnBecomeFallen()
    {
        gameObject.layer = layerDontRender;
        rb.isKinematic = true;
        isAlive = false;
    }

    public bool CheckHasFallen()
    {
        return transform.position.y < -4.0f;
    }
}


