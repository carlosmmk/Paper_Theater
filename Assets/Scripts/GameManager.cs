using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public PauseMenu pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.animating) return;
            (pauseMenu.gameObject.activeSelf ? (Action)pauseMenu.Unpause : pauseMenu.Pause)();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerController.Die(false);
        }
    }
}
