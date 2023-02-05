using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseOverlay;
    public UnityEngine.InputSystem.InputActionReference pauseButton;

    public UnityEngine.UI.Image truffleIndicator;
    public TMPro.TextMeshProUGUI truffleCount;

    public TMPro.TextMeshProUGUI roundCounter;
    public Color standardRoundFontColor, intermissionFontColor;
    public void SetRoundTime(int timeRemaining)
    {
        roundCounter.text = timeRemaining.ToString();
    }
    public void SetRoundMode(bool isIntermission)
    {
        roundCounter.color = isIntermission ? intermissionFontColor : standardRoundFontColor;
    }

    public static InGameUI Instance { get; set; }
    public static bool IsPaused => Instance != null && Instance.pauseOverlay.activeSelf;
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new System.Exception("How on earth do we have two InGameUIs?!?!");

        Hide();
    }

    private void PauseButton_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Pause(!IsPaused);
    }

    public void Pause(bool toggle)
    {
        pauseOverlay.SetActive(toggle);

        if (IsPaused)
        {
            var cineMachine = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
            cineMachine.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            var cineMachine = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
            cineMachine.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Show()
    {
        mainMenu.SetActive(false);
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        mainMenu?.SetActive(true);
        pauseOverlay.SetActive(false);
        this.gameObject.SetActive(false);        
    }

    private void OnEnable()
    {
        SetTruffleCount(0);
        pauseButton.action.performed += PauseButton_performed;
    }

    private void OnDisable()
    {
        pauseButton.action.performed -= PauseButton_performed;
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
