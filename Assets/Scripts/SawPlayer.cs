using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawPlayer : MonoBehaviour
{
    [SerializeField] private bool _setInvisibleMask;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var invisibleMask = other.GetComponent<PlayerController>().depthMask;
        
        invisibleMask.SetActive(_setInvisibleMask);
    }
}
