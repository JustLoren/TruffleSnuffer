using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoundWrapup : MonoBehaviour
{
    public Animator anim;
    public TMPro.TextMeshProUGUI winnerDisplay;
    public TMPro.TextMeshProUGUI descriptionBlock;
    public string winnerText = "It's you. You won.";
    public string loserText = "It wasn't you. Sorry!";
    public void Show()
    {
        var pigs = FindObjectsOfType<PigController>().OrderByDescending(p => p.trufflesGathered).ThenBy(p => p.netId).ToList();
        
        descriptionBlock.text = pigs[0].isLocalPlayer ? winnerText : loserText;
        
        winnerDisplay.color = pigs[0].color;

        this.gameObject.SetActive(true);
        anim.ResetTrigger("Hide");
    }

    public void Hide()
    {
        StartCoroutine(DoHide());
    }

    private IEnumerator DoHide()
    {
        anim.SetTrigger("Hide");
        yield return new WaitForSecondsRealtime(1f);
        this.gameObject.SetActive(false);
    }
}
