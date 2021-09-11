using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    public MainMenu mainMenu;
    public float animDuration = 2;
    public Ease easeType;
    public float period;
    [HideInInspector]public bool animating;
    

    
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
        animating = true;
        transform.DOLocalMoveY(0, animDuration).SetEase(easeType, 0, period).OnComplete(()=> animating = false);
    }

    private void OnDisable()
    {
        animating = false;
    }

    public void Unpause()
    {
        animating = true;
        transform.DOLocalMoveY(mainMenu.endYValue, animDuration).SetEase(easeType, 0, period).OnComplete(()=> gameObject.SetActive(false));
    }

    public void Pause()
    {
        gameObject.SetActive(true);
    }
}
