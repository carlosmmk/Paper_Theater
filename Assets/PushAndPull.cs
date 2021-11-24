using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushAndPull : MonoBehaviour
{
    public float pushRange;
    
    [HideInInspector] public bool pushing;

    private PlayerController playerController;
    private PlayerAnimation anim;
    private Rigidbody rb;
    private Pushable pushableObject;

    private int directionFacing => playerController != null ? playerController.directionFacing : 1;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        anim = GetComponentInChildren<PlayerAnimation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = playerController.rb;
    }

    // Update is called once per frame
    void Update()
    {
        PushPull();
    }
    
    private void PushPull()
    {
        if (pushableObject == null || playerController.IsPlayerSawn())
        {
            return;
        }

        if (!IsPushableAhead() && pushing)
        {
            Debug.Log("NÃO TEM PUSHABLE AHEAD");
            Release();
        }

        if (!pushableObject.grounded && pushableObject.GetComponent<FixedJoint>() != null) //if the object falls
        {
            Release();
            return;
        }

        if (!playerController.IsGrounded() && pushing) // if the player falls
        {
            Release();
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl) && playerController.IsGrounded())
        {
            if (pushing || anim.IsPlayerLanding() || anim.IsPlayerInAir() || !IsPushableAhead())
            {
                return;
            }

            StartCoroutine(GrabObject());
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Release();
        }
    }
    
    void Release()
    {
        pushing = false;
        playerController.BlockInput(0.15f);

        rb.velocity = new Vector2(0, rb.velocity.y);
        playerController.maxSpeed = playerController.originalSpeed;
        Destroy(pushableObject.GetComponent<FixedJoint>());
        pushableObject.isBeingPushed = false;

        if (pushableObject.grounded)
        {
            pushableObject.rb.isKinematic = true;
        }
    }

    bool IsPushableAhead()
    {
        var possiblePushables = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y - 0.5f),
            (Vector3.right * directionFacing), pushRange, playerController.groundLayer);

        return possiblePushables.Any(pushable => pushable.transform.GetComponent<Pushable>() != null);
    }
    
    IEnumerator GrabObject()
    {
        playerController.BlockInput(true);
        pushing = true;
        const float minDist = 0.025f;
        const float grabSpeed = 5f;
        float adjustmentTimer = 0.5f;

        while (true)
        {
            var distToPushable = Vector2.Distance(transform.position, pushableObject.transform.position);

            adjustmentTimer -= Time.deltaTime;

            if (distToPushable > pushableObject.distToGrab) //if player is too far
            {
                rb.AddForce(Vector3.right * directionFacing * grabSpeed);
            }
            else if (distToPushable < pushableObject.distToGrab) //if player is too close
            {
                rb.AddForce(Vector3.right * -directionFacing * grabSpeed);
            }

            if (distToPushable > pushableObject.distToGrab &&
                distToPushable < pushableObject.distToGrab + minDist || adjustmentTimer < 0)
            {
                playerController.BlockInput(false);
                break;
            }

            yield return null;
        }
            
        rb.velocity = new Vector2(0, rb.velocity.y);
        playerController.maxSpeed = playerController.maxSpeed / pushableObject.rb.mass;
        pushableObject.ActivatePhysics();
        pushableObject.isBeingPushed = true;
        var joint = pushableObject.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = rb;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pushable"))
        {
            pushableObject = other.GetComponentInParent<Pushable>();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pushable"))
        {
            pushableObject = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.5f), (Vector3.right * directionFacing) * pushRange); //push ray
    }
}
