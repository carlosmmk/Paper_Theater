using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndTrigger : MonoBehaviour
{
    public Image fadePanel;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        fadePanel.DOFade(1, 2).OnComplete(()=>SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)).SetUpdate(true);
    }
}
