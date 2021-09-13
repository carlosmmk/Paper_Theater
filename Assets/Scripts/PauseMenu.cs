using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public MainMenu mainMenu;
    private Button[] buttons;
    public float animDuration = 2;
    public Ease easeType;
    public float period;
    [HideInInspector]public bool animating;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        SetButtons(false);
        transform.DOLocalMoveY(0, animDuration).SetEase(easeType, 0, period).OnComplete(()=> SetButtons(true)).SetUpdate(true);
    }

    private void OnDisable()
    {
        SetButtons(true);
    }

    private void SetButtons(bool active)
    {
        animating = !active;
        foreach (var b in buttons)
        {
            b.enabled = active;
        }
    }
    
    
    public void Pause()
    {
        if(mainMenu.gameObject.activeSelf) return;
        
        gameObject.SetActive(true);
        mainMenu.CloseCurtains();
        Time.timeScale = 0;
    }
    
    public void Unpause()
    {
        animating = true;
        transform.DOLocalMoveY(mainMenu.endYValue, animDuration).SetEase(easeType, 0, period).OnComplete(()=> gameObject.SetActive(false));
        mainMenu.OpenCurtains();
        Time.timeScale = 1;
    }

    
}
