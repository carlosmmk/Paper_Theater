using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Image buttonImage;
    public TextMeshProUGUI buttonText;
    private Sprite originalButtonImage;
    private Color originalTextColor;
    public Sprite highlightButtonImage;
    public Color highlightTextColor;
    
    
    // Start is called before the first frame update
    void Start()
    {
        originalButtonImage = buttonImage.sprite;
        originalTextColor = buttonText.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject) return;
        
        DeselectButton();
    }

    public void OnSelect(BaseEventData eventData)
    {
        HighlightButton();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DeselectButton();
    }

    void HighlightButton()
    {
        buttonImage.sprite = highlightButtonImage;
        buttonText.color = highlightTextColor;
        
    }
    
    void DeselectButton()
    {
        buttonImage.sprite = originalButtonImage;
        buttonText.color = originalTextColor;
    }
    
}
