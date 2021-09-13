using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public Rigidbody rb;
    private PlayerController player;
    public FixedJoint fixedJoint;
    
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        fixedJoint = GetComponentInChildren<FixedJoint>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            //StartCoroutine(DisablePhysics());
        }
    }

    public IEnumerator DisablePhysics()
    {
        while (rb.velocity.magnitude != 0 || rb.angularVelocity.magnitude != 0)
        {
            yield return null;
        }

        rb.isKinematic = true;
    }
}
