using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector2 initialPos;
    public Vector2 moveRange;
    public float moveSpeed;

    public Vector3 lastPosition;
    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;

        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mov = new Vector3 (moveRange.x + Mathf.Sin(moveSpeed * Time.time) * moveRange.x, 
            moveRange.y + Mathf.Sin(moveSpeed * Time.time) * moveRange.y, transform.position.z);
        transform.position = ((Vector3)initialPos + mov);
    }

    private void LateUpdate()
    {
        // Vector3 velocity = transform.position - lastPosition;
        // Debug.Log(velocity);
        //
        // GameObject.FindGameObjectWithTag("Player").transform.Translate(velocity, transform);
        //
        // lastPosition = transform.position;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() == null) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.parent == transform) return;

            var playerScript = other.gameObject.GetComponent<PlayerController>();

            if (playerScript.IsGrounded())
            {
                other.transform.SetParent(transform);
                // playerScript.rb.velocity = rb.velocity;
                // playerScript.rb.angularVelocity = rb.angularVelocity;
            }
                 
                
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() == null) return;
        
        other.transform.SetParent(null);
        
    }
}