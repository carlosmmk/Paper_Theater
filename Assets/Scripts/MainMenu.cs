using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public float animDuration;
    public float endYValue;
    public Ease easeType;
    public float period;
    public Image fadePanel;
    public float fadeDuration;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        transform.DOLocalMoveY(endYValue, animDuration).SetEase(easeType, 0, period).OnComplete(()=> gameObject.SetActive(false));
        
    }

    public void Quit()
    {
        fadePanel.DOFade(1, fadeDuration).OnComplete(()=>Application.Quit());
    }
    
    
}
