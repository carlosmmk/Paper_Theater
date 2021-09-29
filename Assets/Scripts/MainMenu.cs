using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Transform curtainLeft;
    public Transform curtainRight;
    public float animDuration;
    public float endYValue;
    public Ease easeType;
    public float period;
    public Image fadePanel;
    public float fadeDuration;
    
    // Start is called before the first frame update
    void Start()
    {
        fadePanel.color = Color.black;
        fadePanel.DOFade(0, fadeDuration).SetUpdate(true);
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        transform.DOLocalMoveY(endYValue, animDuration).SetEase(easeType, 0, period).OnComplete(()=> gameObject.SetActive(false));
        OpenCurtains();
        Time.timeScale = 1;
        
    }

    public void Quit()
    {
        fadePanel.DOFade(1, fadeDuration).OnComplete(()=>Application.Quit()).SetUpdate(true);
    }

    public void OpenCurtains()
    {
        curtainLeft.DOLocalMoveX(-6.5f, animDuration).SetUpdate(true);;
        curtainRight.DOLocalMoveX(6.5f, animDuration).SetUpdate(true);;
    }
    
    public void CloseCurtains()
    {
        curtainLeft.DOLocalMoveX(-2.18f, animDuration).SetUpdate(true);;
        curtainRight.DOLocalMoveX(2.28f, animDuration).SetUpdate(true);;
    }
}
