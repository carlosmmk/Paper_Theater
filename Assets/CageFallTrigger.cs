using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageFallTrigger : MonoBehaviour
{
    [SerializeField] private Rigidbody cageRb;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        cageRb.isKinematic = false;
    }
}
