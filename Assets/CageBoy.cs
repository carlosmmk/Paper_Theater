using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageBoy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            return;
        }
        
        AudioManager.instance.Play("Cage");
        Destroy(gameObject);
    }
}
