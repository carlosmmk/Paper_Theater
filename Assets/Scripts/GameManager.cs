using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PauseMenu pauseMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.animating) return;
            (pauseMenu.gameObject.activeSelf ? (Action)pauseMenu.Unpause : pauseMenu.Pause)();
        }
    }
}
