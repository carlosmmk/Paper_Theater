using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("REFERENCES")] 
    
    [SerializeField] private Image fadePanel;
    private Rigidbody rb;
    
    [Space(10)]
    
    public float speed = 5;
    public float jumpForce = 10;
    public float regularGravity = -20;
    public float fallingGravity = -20;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float maxSpeed;
    [Range(0,1)]
    public float airControl;
    public float deceleration;

    private Vector3 direction;
    private bool jumped;
    private bool dead;
    private float gravity = -20;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (dead) return;
        
        float hInput = Input.GetAxisRaw("Horizontal");

        direction.x = hInput * speed;
        direction.y = gravity;

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

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, -0.15f, groundLayer); 
    }

    private void Jump()
    {
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
            direction = Vector3.zero;
            transform.position = CheckpointManager.instance.activeCheckpoint.transform.position;
            //resetar o player
            yield return new WaitForSeconds(0.5f);
            dead = false;
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Threat")) return;
        
        Die();
    }
}
