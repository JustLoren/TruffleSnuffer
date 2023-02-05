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
    public Vector3 spawnMin, spawnMax;

    private void SpawnTruffle()
    {
        var point = GetSpawnPoint();
        var newTruffle = Instantiate(trufflePrefab, point, Quaternion.identity);            

        NetworkServer.Spawn(newTruffle.gameObject);        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnMin, new Vector3(spawnMin.x, spawnMin.y, spawnMax.z));
        Gizmos.DrawLine(spawnMin, new Vector3(spawnMax.x, spawnMin.y, spawnMin.z));
        Gizmos.DrawLine(spawnMax, new Vector3(spawnMin.x, spawnMin.y, spawnMax.z));
        Gizmos.DrawLine(spawnMax, new Vector3(spawnMax.x, spawnMin.y, spawnMin.z));
    }
        
    public int spawnLayer;
    private Vector3 GetSpawnPoint()
    {
        Vector3 result = Vector3.zero;
        RaycastHit hit;
        bool foundLocation = false;
        int maxAttempts = 100;
        int currentAttempts = 0;
        do
        {
            var spawnPt = new Vector3(Random.Range(spawnMin.x, spawnMax.x), Random.Range(spawnMin.y, spawnMax.y), Random.Range(spawnMin.z, spawnMax.z));
            spawnPt.y = 100f;            
            if (Physics.Raycast(spawnPt, Vector3.down, out hit, 101, Physics.AllLayers, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject.layer == spawnLayer)
                {
                    Debug.DrawRay(spawnPt, Vector3.down * 100, Color.green, 10f);
                    foundLocation = true;
                    result = hit.point;
                } else
                {
                    Debug.DrawRay(spawnPt, Vector3.down * 100, Color.red, 10f);
                }
            } else
            {
                Debug.DrawRay(spawnPt, Vector3.down * 100, Color.yellow, 10f);
            }
            
            if (++currentAttempts >= maxAttempts)
                throw new System.Exception("We couldn't find a spawn location. wtf, guys.");

        } while (!foundLocation);

        return result;
    }

    public void RestoreSpawnPoint()
    {
        //Um, how did we spawn this many
        if ((RoundManager.Instance?.IsRoundOngoing).GetValueOrDefault())
            SpawnTruffle();
    }

    public void ExpireTruffle(GameObject truffle)
    {
        var mole = Instantiate(molePrefab, truffle.transform.position, truffle.transform.rotation);

        NetworkServer.Spawn(mole);

        Destroy(mole, moleLifetime);

        Destroy(truffle);
    }
}
