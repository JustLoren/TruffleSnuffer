using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoundWrapup : MonoBehaviour
{
    public Animator anim;
    public TMPro.TextMeshProUGUI winnerDisplay;
    public TMPro.TextMeshProUGUI descriptionBlock;
    public void Show()
    {
        var pigs = FindObjectsOfType<PigController>().OrderBy(p => p.trufflesGathered).ToList();
        if (pigs[0].isLocalPlayer)
            descriptionBlock.text = "It's you. You won.";
        else
            descriptionBlock.text = "It wasn't you. Sorry!";
        
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
