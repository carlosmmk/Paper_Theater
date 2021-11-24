using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorMovingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        other.transform.parent = transform.parent;
    }
    
    private void OnTriggerExit(Collider other)
    {
        var rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        other.transform.parent = null;
    }
}
