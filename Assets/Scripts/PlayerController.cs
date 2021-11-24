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

    [SerializeField] private PhysicMaterial playerPhysicMaterial;
    [SerializeField] private PhysicMaterial slopePhysicMaterial;

    public GameObject depthMask;
    public Transform groundCheck;
    public Transform feetPosition;
    public LayerMask groundLayer;
    [HideInInspector] public Rigidbody rb;
    private RagdollController ragdollController;
    private PlayerAnimation anim;
    private PushAndPull pushAndPull;
    [HideInInspector] public CapsuleCollider playerCollider;
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
    [HideInInspector] public float originalSpeed;
    [Range(0, 1)] public float airControl;
    public float deceleration;
    [SerializeField] private float _timeToDie;
    private float _fallingTimer;

    [HideInInspector] public Vector3 direction;
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
        playerCollider = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<PlayerAnimation>();
        pushAndPull = GetComponent<PushAndPull>();
        ragdollController = GetComponent<RagdollController>();
        originalSpeed = maxSpeed;
        _fallingTimer = _timeToDie;
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

        playerCollider.material = direction.x == 0 && IsGrounded()? slopePhysicMaterial : playerPhysicMaterial;

        directionFacing = transform.localScale.x > 0 ? 1 : -1;

        FlipSprite();
        AdjustCollider();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump")) //if player releases jump button
        {
            if (jumped && rb.velocity.y > 0) //if player is gaining height
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.75f);
            }
        }
        CoyotteTime();
        
        Landing();

        if (rb.velocity.y < 0 && !dead && !IsGrounded())
        {
            _fallingTimer -= Time.deltaTime;
        }

        if (rb.velocity.y > 0)
        {
            _fallingTimer = _timeToDie;
        }
    }

    private void FixedUpdate()
    {
        Movement();
        Gravity();

        Deceleration();
    }

    private void FlipSprite()
    {
        if (direction.x == 0 || pushAndPull.pushing || blockedInput) return;

        transform.localScale = new Vector3(
            direction.x > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y, transform.localScale.z);
    }

    private void CoyotteTime()
    {
        if (!IsGrounded() && !jumped)
        {
            if (coyotteSaved) return;
            
            StartCoroutine(CoyotteTimeCoroutine()); 
        }
    }

    public bool IsGrounded()
    {
        if (groundedCooldown || !playerCollider.enabled) return false;

        return Physics.CheckBox(groundCheck.position, new Vector3(groundedRadius, 0.15f), Quaternion.identity, groundLayer);
    }

    private void Jump()
    {
        if (!canJump || pushAndPull.pushing || jumped) return;
        
        anim.PlayAnimation("TakeOff");
        rb.velocity = new Vector3(rb.velocity.x,jumpForce);
        StartCoroutine(StartGroundedCooldown());
        jumped = true;
    }
    
    private void Landing()
    {
        if (IsGrounded())
        {
            if (landed) return;

            if (_fallingTimer < 0)
            {
                //if player dies
                Die(true);
            }
            
            jumped = false;
            canJump = true;
            coyotteSaved = false;
            landed = true;
            _fallingTimer = _timeToDie;

            if (!anim.IsPlayerLanding()) 
                anim.PlayAnimation("Landing");
            
        }
        else
        {
            if (landed)
            {
                landed = false;
            }
        }
    }

    private void Movement()
    {
        if (dead)
        {
            return;
        }
        
        float movementModifier = IsGrounded() ? 1 : airControl;

        rb.velocity += AdjustVelocityToSlope(new Vector3(direction.x * movementModifier, rb.velocity.y) * Time.deltaTime);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
        
        Vector3 AdjustVelocityToSlope(Vector3 velocity)
        {
            var ray = new Ray(groundCheck.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
            {
                var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                var adjustedVelocity = slopeRotation * velocity;

                if (adjustedVelocity.y < 0)
                {
                    return adjustedVelocity;
                }
            }

            return velocity;
        }
    }

    private void Gravity()
    {
        if (IsGrounded() && !IsPlayerOnSlope()) return;
        
        gravity = rb.velocity.y < 0 ? fallingGravity : regularGravity;

        rb.velocity += Vector3.up * gravity * Time.deltaTime;
    }

    private void Deceleration()
    {
        if (direction.x != 0) return;
        
        if (IsGrounded())
        {
            rb.velocity += -rb.velocity * deceleration * Time.deltaTime; //decelerate general speed if its on the ground
        }
        else
        {
            rb.velocity += new Vector3(-rb.velocity.x * deceleration * airControl * Time.deltaTime, 0); //decelerate only X speed if its on air
        }
            
        if (Mathf.Abs(rb.velocity.x) < 0.1f)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); //null speed if it's too small
        }
    }

    public void Die(bool activateRagdoll)
    {
        if (dead)
        {
            return;
        }

        if (activateRagdoll)
        {
            ragdollController.ActivateRagdoll();
        }

        dead = true;
        rb.velocity = Vector3.zero;
        direction = Vector3.zero;

        Sequence deathSequence = DOTween.Sequence();
        deathSequence.AppendInterval(1f);
        deathSequence.Append(fadePanel.DOFade(1, 2).OnComplete(() => StartCoroutine(Reborn())));
        deathSequence.AppendInterval(0.5f);
        deathSequence.Append(fadePanel.DOFade(0, 2));
        //animação de morte
    }
    
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
            
        dead = false;
        _fallingTimer = _timeToDie;
        //resetar o player
        yield return new WaitForSeconds(0.5f);
            
            
    }

    private void AdjustCollider()
    {
        if (IsPlayerSawn())
        {
            playerCollider.height = 1;
            playerCollider.center = new Vector2(0, -0.54f);
            return;
        }
        
        if (IsGrounded())
        {
            //playerCollider.height = new Vector3(playerCollider.size.x, 2, playerCollider.size.z);
            playerCollider.height = 2;
            playerCollider.center = Vector3.zero;
            return;
        }
        
        var newColliderHeight = 2.2f - feetPosition.localPosition.y / 6;
        playerCollider.height = newColliderHeight;
        //playerCollider.size = new Vector3(playerCollider.size.x, newColliderHeight, playerCollider.size.z);
        playerCollider.center = new Vector2(0, feetPosition.localPosition.y / 15);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Threat"))
        {
            Die(true);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("Ground"))) return;

        if (IsGrounded() || groundedCooldown)
        {
            
        }
    }

    public void BlockInput(bool blocked)
    {
        blockedInput = blocked;
        direction = blocked ? Vector3.zero : direction;
    }
    
    public void BlockInput(float time)
    {
        StartCoroutine(BlockInputTimer(time));
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
    
    public IEnumerator CoyotteTimeCoroutine()
    {
        coyotteSaved = true;
        canJump = true;
        
        yield return new WaitForSeconds(coyotteTime);
        
        if (!IsGrounded())
            canJump = false;
    }

    private Vector2 GetSlopeNormal()
    {
        Vector2 checkPos = transform.position - new Vector3(0, playerCollider.height / 2);
        
        RaycastHit hit; 
        if (Physics.Raycast(new Ray(checkPos, Vector3.down), out hit, 1.5f, groundLayer));
        {
            var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            Debug.DrawRay(hit.point, Vector2.Perpendicular(hit.normal), Color.green);
            Debug.DrawRay(hit.point, hit.normal, Color.red);
        }
        
        return Vector2.Perpendicular(hit.normal).normalized;
    }

    private bool IsPlayerOnSlope()
    {
        var slopeNormal = GetSlopeNormal();
        
        var slopeX = Mathf.Abs(slopeNormal.x);
        var slopeY = Mathf.Abs(slopeNormal.y);

        return slopeX != 0 && slopeX != 1 && slopeY != 0 && slopeY != 1;
    }

    public bool IsPlayerSawn()
    {
        return depthMask.activeSelf;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundedRadius, 0.15f));
    }
}
