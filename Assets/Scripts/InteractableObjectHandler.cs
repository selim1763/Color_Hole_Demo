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
    private Collider col;

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
        col = GetComponent<Collider>();

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
        Vector3 adjPos = new Vector3(transform.position.x, transform.position.y - col.bounds.extents.y, transform.position.z);
        float diff = Vector3.Distance(holeCenterPoint.position, adjPos);

        rb.AddForce(Vector3.down * 8f + dir * 4f, ForceMode.Force);

        if (diff < 1.2f)
         {
            rb.AddForce(Vector3.down * 15f, ForceMode.VelocityChange);
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


