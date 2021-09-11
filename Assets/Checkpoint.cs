using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector] public Transform playerSpawnPos;
    private bool alreadyActivated;
    [SerializeField] private List<GameObject> dependentGameobjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        playerSpawnPos = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || alreadyActivated) return;
        
        CheckpointManager.instance.SetActiveCheckpoint(this);
        alreadyActivated = true;
    }
}
