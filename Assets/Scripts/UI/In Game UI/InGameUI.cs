using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject mainMenu;
    public UnityEngine.UI.Image truffleIndicator;
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

    #region Multiplayer Scorekeeping
    public GameObject scoreCounterPrefab;
    private Dictionary<PigController, GameObject> opponents = new();

    public void ColorizePlayer(Color color)
    {
        truffleIndicator.color = color;
    }

    public void AddCompetitor(PigController opponent)
    {
        if (!opponents.ContainsKey(opponent))
        {
            var newUI = Instantiate(scoreCounterPrefab, this.transform);
            var rectTransform = newUI.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(225 + (opponents.Keys.Count * 200), 0f);
            opponents.Add(opponent, newUI);
            var truffleBg = newUI.GetComponentInChildren<UnityEngine.UI.Image>();
            truffleBg.color = opponent.color;

            SetOpponentScore(opponent, opponent.trufflesGathered);
        }
        else
            Debug.LogError("We've set this competitor's color twice now!");
    }

    public void RemoveCompetitor(PigController opponent)
    {
        if (opponents.ContainsKey(opponent))
        {
            var ui = opponents[opponent];
            opponents.Remove(opponent);
            Destroy(ui);
        }
    }

    public void SetOpponentScore(PigController opponent, int score)
    {
        opponents[opponent].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = score.ToString();
    }
    #endregion
}
