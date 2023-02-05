using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruffleSpawner : NetworkBehaviour
{
    public int maxTruffleCount = 5;
    #region Singleton shenanigans
    public static TruffleSpawner Instance { get; private set; }    

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (Instance == null)
            Instance = this;
        else
            throw new System.Exception("How on earth do we have two truffle spawners?!");        
    }

    public void BeginRound()
    {
        foreach (var pig in FindObjectsOfType<PigController>())
            pig.trufflesGathered = 0;

        for (int i = 0; i < maxTruffleCount; i++)
            SpawnTruffle();
    }

    public void RoundCleanup()
    {
        var remainingTruffles = FindObjectsOfType<ProximityAlert>();
        foreach(var truffle in remainingTruffles)        
            Destroy(truffle.gameObject);        
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        if (Instance == this)
            Instance = null;
    }
    #endregion

    public ProximityAlert trufflePrefab;
    public GameObject molePrefab;
    public float moleLifetime = 5f;
    public List<Transform> spawnPoints = new List<Transform>();

    private void SpawnTruffle()
    {
        if (spawnPoints.Count > 0)
        {
            var point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            spawnPoints.Remove(point);

            var newTruffle = Instantiate(trufflePrefab, point.position, point.rotation);
            newTruffle.spawnPoint = point;

            NetworkServer.Spawn(newTruffle.gameObject);
        }
    }

    public void RestoreSpawnPoint(Transform point)
    {
        //Um, how did we spawn this many
        if (spawnPoints.Count > 0 && (RoundManager.Instance?.IsRoundOngoing).GetValueOrDefault())
            SpawnTruffle();

        spawnPoints.Add(point);
    }

    public void ExpireTruffle(GameObject truffle)
    {
        var mole = Instantiate(molePrefab, truffle.transform.position, truffle.transform.rotation);

        NetworkServer.Spawn(mole);

        Destroy(mole, moleLifetime);

        Destroy(truffle);
    }
}
