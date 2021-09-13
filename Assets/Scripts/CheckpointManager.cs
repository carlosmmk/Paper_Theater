using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;
    public Checkpoint activeCheckpoint;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveCheckpoint(Checkpoint checkpoint)
    {
        activeCheckpoint = checkpoint;
    }
}
