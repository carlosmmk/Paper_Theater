using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine.Rendering.HighDefinition;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    [Header("REFERENCES")] [SerializeField]
    private Image fadePanel;

    public GameObject depthMask;
    public Transform groundCheck;
    public Transform feetPosition;
    public LayerMask groundLayer;
    [HideInInspector] public Rigidbody rb;
    private RagdollController ragdollController;
    private PlayerAnimation anim;
    [HideInInspector] public BoxCollider playerCollider;
    public float groundedRadius;
    private bool groundedCooldown;

    [Space(10)] public float speed = 5;
    public float jumpForce = 10;
    
    public float coyotteTime;
    private bool coyotteSaved;
    
    private bool canJump;
    public float regularGravity = -20;
    public float fallingGravity = -20;
    public float maxSpeed;
    private float originalSpeed;
    [Range(0, 1)] public float airControl;
    public float deceleration;
    public float pushRange;
    public float climbRange;
    public float climbRadius;
    public float heightToDie;

    [HideInInspector] public Vector3 direction;
    [HideInInspector] public bool pushing;
    [HideInInspector] public int directionFacing = 1;
    private bool jumped;
    private bool stopped;
    [HideInInspector] public bool dead;
    private bool blockedInput;
    private float gravity = -20;

    private Pushable pushableObject;

    private bool landed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<BoxCollider>();
        anim = GetComponentInChildren<PlayerAnimation>();
        ragdollController = GetComponent<RagdollController>();
        originalSpeed = maxSpeed;
    }

    private void Start()
    {
    }

    void Update()
    {
        if (dead || blockedInput) return;

        float hInput = Input.GetAxisRaw("Horizontal");

        direction.x = hInput * speed;
        direction.y = gravity;

        directionFacing = (int)transform.localScale.x;

        FlipSprite();
        //AdjustCollider();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        Landing();

        if (Mathf.Abs(rb.velocity.x) < 0.1f && direction.x == 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        PushPull();
        
    }

    private void Landing()
    {
        if (IsGrounded() && !landed)
        {
            anim.PlayAnimation("Landing");
            landed = true;
        }

        if (!IsGrounded()) landed = false;
    }

    private void FixedUpdate()
    {
        Movement();
        Gravity();

        Deceleration();
    }

    private void FlipSprite()
    {
        if (direction.x == 0 || pushing || blockedInput) return;

        transform.localScale = new Vector3(
            direction.x > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y, transform.localScale.z);
    }

    public bool IsGrounded()
    {
        if (groundedCooldown || !playerCollider.enabled) return false;

        var sphereCheck = Physics.CheckSphere(groundCheck.position, groundedRadius, groundLayer);

        if (!sphereCheck && !jumped)
        {
            if (!canJump)
            {
                if (coyotteSaved) return false;
                StartCoroutine(CoyotteTime()); 
            }
            else
            {
                return true;
            }
        }

        return sphereCheck;
    }

    private void Jump()
    {
        if (!IsGrounded() || pushing) return;
        
        anim.PlayAnimation("TakeOff");
        rb.velocity = new Vector3(rb.velocity.x,jumpForce);
        StartCoroutine(StartGroundedCooldown());
        jumped = true;
    }

    private void Movement()
    {
        float movementModifier = IsGrounded() ? 1 : airControl;
        rb.velocity += new Vector3(direction.x * movementModifier, rb.velocity.y) * Time.deltaTime;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
    }

    private void Gravity()
    {
        if (direction.x == 0 && IsGrounded() && !canJump) return;
        
        gravity = rb.velocity.y < 0 ? fallingGravity : regularGravity;

        rb.velocity += Vector3.up * gravity * Time.deltaTime;
    }

    private void Deceleration()
    {
        if (direction.x == 0) //if there's no input
        {
            if (IsGrounded())
            {
                rb.velocity += -rb.velocity * deceleration * Time.deltaTime;
            }
            else
            {
                rb.velocity += new Vector3(-rb.velocity.x * deceleration * airControl * Time.deltaTime, 0);
            }
        }
    }

    private void Die()
    {
        dead = true;
        rb.velocity = Vector3.zero;
        direction = Vector3.zero;
        
        Sequence deathSequence = DOTween.Sequence();
        deathSequence.AppendInterval(1f);
        deathSequence.Append(fadePanel.DOFade(1, 2).OnComplete(() => StartCoroutine(Reborn())));
        deathSequence.AppendInterval(0.5f);
        deathSequence.Append(fadePanel.DOFade(0, 2));
        //animação de morte

        IEnumerator Reborn()
        {
            var lastCheckpoint = CheckpointManager.instance.activeCheckpoint;
            var lastChild = transform.GetChild(transform.childCount - 1);
            
            if (lastChild.name.Contains("Ragdoll"))
            {
                Destroy(lastChild.gameObject); //disable ragdoll
            }
            
            var followerSpotLights = FindObjectsOfType<FollowerSpotLight>();
            
            foreach (var spotlight in followerSpotLights)
                spotlight.SetTarget(transform); //reassign player as target
            
            anim.gameObject.SetActive(true); //enable regular sprite

            direction = Vector3.zero;
            transform.position =
                new Vector3(lastCheckpoint.transform.position.x, lastCheckpoint.transform.position.y, 0);
            //resetar o player
            yield return new WaitForSeconds(0.5f);
            dead = false;
        }
    }

    private void Climbing()
    {
        var possibleCorners = Physics.OverlapSphere(
            new Vector2(transform.position.x + (float)directionFacing / 3, transform.position.y + climbRange),
            climbRadius, groundLayer);

        foreach (var corner in possibleCorners)
        {
            Debug.Log(corner.bounds.max);
        }
    }

    private void PushPull()
    {
        if (pushableObject == null) return;

        if (!IsPushableAhead() && pushing)
        {
            Release();
        }

        if (!pushableObject.grounded && pushableObject.GetComponent<FixedJoint>() != null) //if the object falls
        {
            Release();
            return;
        }

        if (!IsGrounded() && pushing) // if the player falls
        {
            Release();
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl) && IsGrounded())
        {
            if (pushing || anim.IsPlayerLanding() || anim.IsPlayerInAir() || !IsPushableAhead()) return;

            StartCoroutine(GrabObject());
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Release();
        }

        void Release()
        {
            pushing = false;
            StartCoroutine(BlockInputTimer(0.15f));

            rb.velocity = new Vector2(0, rb.velocity.y);
            maxSpeed = originalSpeed;
            Destroy(pushableObject.GetComponent<FixedJoint>());

            if (pushableObject.grounded)
            {
                pushableObject.rb.isKinematic = true;
            }
        }

        bool IsPushableAhead()
        {
            var possiblePushables = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y - 0.5f),
                (Vector3.right * directionFacing), pushRange, groundLayer);

            return possiblePushables.Any(pushable => pushable.transform.GetComponent<Pushable>() != null);
        }

        IEnumerator GrabObject()
        {
            BlockInput(true);
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
                    BlockInput(false);
                    break;
                }

                yield return null;
            }
            
            rb.velocity = new Vector2(0, rb.velocity.y);
            maxSpeed = maxSpeed / pushableObject.rb.mass;
            pushableObject.ActivatePhysics();
            var joint = pushableObject.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rb;
        }
    }

    private void AdjustCollider()
    {
        if (IsGrounded())
        {
            playerCollider.size = new Vector3(playerCollider.size.x, 2, playerCollider.size.z);
            playerCollider.center = Vector3.zero;
            return;
        }

        var newColliderHeight = 2.2f - feetPosition.localPosition.y / 6;
        playerCollider.size = new Vector3(playerCollider.size.x, newColliderHeight, playerCollider.size.z);
        playerCollider.center = new Vector2(0, feetPosition.localPosition.y / 15);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Threat"))
        {
            Die();
        }

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

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("Ground"))) return;

        if (IsGrounded() || groundedCooldown)
        {
            jumped = false;
            coyotteSaved = false;

            //if player dies
            //ragdollController.ActivateRagdoll();
            //Die();
        }
}

    private void BlockInput(bool blocked)
    {
        blockedInput = blocked;
        direction = blocked ? Vector3.zero : direction;
    }

    public IEnumerator BlockInputTimer(float duration)
    {
        blockedInput = true;
        direction = Vector3.zero;
        yield return new WaitForSeconds(duration);
        blockedInput = false;
    }

    public IEnumerator StartGroundedCooldown()
    {
        groundedCooldown = true;
        yield return new WaitForSeconds(0.1f);
        groundedCooldown = false;
    }
    
    public IEnumerator CoyotteTime()
    {
        canJump = true;
        
        yield return new WaitForSeconds(coyotteTime);
        
        canJump = false;
        coyotteSaved = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundedRadius); //grounded sphere
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.5f), (Vector3.right * directionFacing) * pushRange); //push ray
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + (float)directionFacing / 3, transform.position.y + climbRange), climbRadius); //climb sphere
    }
}
