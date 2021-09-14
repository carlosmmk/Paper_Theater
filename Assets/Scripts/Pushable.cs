using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public Rigidbody rb;
    private PhysicMaterial originalPM;
    private Collider collider;
    public LayerMask groundLayer;
    public bool grounded;
    public float groundedRayDist;
    [HideInInspector] public bool jointBroken;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        originalPM = collider.material;
        collider.material = null;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, -transform.up, groundedRayDist, groundLayer);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            StartCoroutine(DisablePhysics());
        }
    }

    public IEnumerator DisablePhysics()
    {
        while (rb.velocity.magnitude != 0 || rb.angularVelocity.magnitude != 0)
        {
            yield return null;
        }

        rb.isKinematic = true;
        collider.material = originalPM;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -transform.up * groundedRayDist);
    }
}
