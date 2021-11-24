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

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }
    
    
    void FixedUpdate()
    {
        Vector3 mov = new Vector3 (moveRange.x + Mathf.Sin(moveSpeed * Time.time) * moveRange.x, 
            moveRange.y + Mathf.Sin(moveSpeed * Time.time) * moveRange.y, 0);
        transform.position = (Vector3)initialPos + mov;
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
            }
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() == null) return;
        
        other.transform.SetParent(null);
    }
}