using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject mainMenu;
    public TMPro.TextMeshProUGUI truffleCount;
    public static InGameUI Instance { get; set; }
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new System.Exception("How on earth do we have two InGameUIs?!?!");

        Hide();
    }

    public void Show()
    {
        mainMenu.SetActive(false);
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        mainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetTruffleCount(0);
    }

    public void SetTruffleCount(int count)
    {        
        truffleCount.text = count.ToString();
    }

    public GameObject truffleGainFx;    
    public void SpawnTruffleGainFX()
    {
        Instantiate(truffleGainFx, this.transform);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
