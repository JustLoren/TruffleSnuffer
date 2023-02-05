using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public AudioClip hoverClip;
    public AudioSource audioSrc;

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioSrc.PlayOneShot(hoverClip);
    }

    public void OnSelect(BaseEventData eventData)
    {        
        audioSrc.PlayOneShot(hoverClip);
    }
}
