using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;

public class PlayerController : MonoBehaviour
{
    [Header("REFERENCES")] 
    
    [SerializeField] private Image fadePanel;
    public Transform groundCheck;
    public Transform feetPosition;
    public LayerMask groundLayer;
    [HideInInspector]public Rigidbody rb;
    private PlayerAnimation anim;
    private CapsuleCollider playerCollider;
    public float groundedRadius;
    
    [Space(10)]
    
    public float speed = 5;
    public float jumpForce = 10;
    public float regularGravity = -20;
    public float fallingGravity = -20;
    public float maxSpeed;
    private float originalSpeed;
    [Range(0,1)]
    public float airControl;
    public float deceleration;

    private Vector3 direction;
    [HideInInspector] public bool pushing;
    private bool jumped;
    private bool dead;
    private bool blockedInput;
    private float gravity = -20;

    private Pushable pushableObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<PlayerAnimation>();
        originalSpeed = maxSpeed;
    }

    private void Start()
    {
    }

    void Update()
    {
        AdjustCollider();
        
        if (dead || blockedInput) return;
        
        float hInput = Input.GetAxisRaw("Horizontal");

        direction.x = hInput * speed;
        direction.y = gravity;
        
        FlipSprite();

        gravity = rb.velocity.y < 0 ? fallingGravity : regularGravity;
        
        if (Input.GetButtonDown("Jump"))
        {
            CallJump();
        }

        if (Mathf.Abs(rb.velocity.x) < 0.1f && direction.x == 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),rb.velocity.y);

        PushPull();

    }

    private void FixedUpdate()
    {
        Movement();
        Gravity();

        Deceleration();
        
        if (jumped)
        {
            Jump();
        }
    }

    private void FlipSprite()
    {
        if (direction.x == 0 || pushing) return;
        
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundedRadius, groundLayer); 
    }

    private void Jump()
    {
        anim.SetTrigger("Jump");
        rb.AddForce(Vector3.up*jumpForce, ForceMode.Impulse);
        jumped = false;
    }

    private void CallJump()
    {
        if(!IsGrounded()) return;
        jumped = true;
    }

    private void Movement()
    {
        float movementModifier = IsGrounded() ? 1 : airControl;
        rb.AddForce(Vector3.right*direction.x*movementModifier);
    }

    private void Gravity()
    {
        rb.AddForce(Vector3.up*gravity, ForceMode.Acceleration);
    }

    private void Deceleration()
    {
        if (direction.x == 0)
        {
            rb.AddForce(-Vector3.right*rb.velocity.x*deceleration);
        }
    }

    public void Die()
    {
        dead = true;
        Sequence deathSequence = DOTween.Sequence();
        
        deathSequence.Append(fadePanel.DOFade(1, 2).OnComplete(() => StartCoroutine(Reborn())));
        deathSequence.AppendInterval(0.5f);
        deathSequence.Append(fadePanel.DOFade(0, 2));
        //animação de morte

        IEnumerator Reborn()
        {
            var lastCheckpoint = CheckpointManager.instance.activeCheckpoint;
            
            direction = Vector3.zero;
            transform.position = new Vector3(lastCheckpoint.transform.position.x, lastCheckpoint.transform.position.y, 0);
            //resetar o player
            yield return new WaitForSeconds(0.5f);
            dead = false;
        }
    }
    
    private void PushPull()
    {
        if (pushableObject == null) return;

        if (!pushableObject.grounded && pushableObject.GetComponent<FixedJoint>() != null)
        {
            Release();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            pushing = true;
            rb.velocity = new Vector2(0, rb.velocity.y);
            maxSpeed = maxSpeed / pushableObject.rb.mass;
            pushableObject.rb.isKinematic = false;
            var joint = pushableObject.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = rb;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Release();
        }

        void Release()
        {
            pushing = false;
            StartCoroutine(BlockInput(0.15f));
            
            rb.velocity = new Vector2(0, rb.velocity.y);
            maxSpeed = originalSpeed;
            Destroy(pushableObject.GetComponent<FixedJoint>());

            if (pushableObject.grounded)
            {
                pushableObject.rb.isKinematic = true;
            }
        }
    }

    private void AdjustCollider()
    {
        playerCollider.height = 2 - feetPosition.localPosition.y / 6;
        playerCollider.center = new Vector2(0,feetPosition.localPosition.y / 15);
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

        if (other.relativeVelocity.y != 0 && IsGrounded())
        {
            anim.SetTrigger("Land");
        }
    }

    public IEnumerator BlockInput(float duration)
    {
        blockedInput = true;
        direction = Vector3.zero;
        yield return new WaitForSeconds(duration);
        blockedInput = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, groundedRadius);
    }
}
