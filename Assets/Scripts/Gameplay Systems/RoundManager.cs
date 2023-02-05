using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoundManager : NetworkBehaviour
{
    public static RoundManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            throw new System.Exception("Why are there two RoundManagers? Cheese and crackers, Loren.");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public int roundLength = 99;
    public int timeBetweenRounds = 10;

    [SyncVar(hook = nameof(UpdateTimeRemaining))] 
    public int timeRemaining;
    private void UpdateTimeRemaining(int _old, int _new)
    {
        InGameUI.Instance?.SetRoundTime(_new);
    }
    [SyncVar(hook = nameof(UpdateIntermission))]
    public bool isIntermission;
    private void UpdateIntermission(bool _old, bool _new)
    {
        InGameUI.Instance?.SetRoundMode(_new);
    }

    private float realTimeRemaining = 0f;
    private float delayTimeRemaining = 0f;

    public bool IsRoundOngoing => realTimeRemaining > 0f;

    public override void OnStartServer()
    {
        base.OnStartServer();

        realTimeRemaining = 0f;
        delayTimeRemaining = 0f;
    }

    private void Update()
    {
        if (!isServer)
        {
            this.enabled = false;
            return;
        }

        if (realTimeRemaining == 0f)
        {
            delayTimeRemaining = Mathf.Max(delayTimeRemaining - Time.deltaTime, 0f);
            timeRemaining = Mathf.RoundToInt(delayTimeRemaining);

            if (delayTimeRemaining == 0f)
            {
                isIntermission = false;
                realTimeRemaining = roundLength;
                TruffleSpawner.Instance?.BeginRound();
            }
        } 
        else
        {
            realTimeRemaining = Mathf.Max(realTimeRemaining - Time.deltaTime, 0f);
            timeRemaining = Mathf.RoundToInt(realTimeRemaining);

            if (realTimeRemaining == 0f)
            {
                isIntermission = true;
                delayTimeRemaining = timeBetweenRounds;
                TruffleSpawner.Instance?.RoundCleanup();
            }
        }
    }
}
