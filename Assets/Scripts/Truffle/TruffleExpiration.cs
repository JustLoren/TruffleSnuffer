using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruffleExpiration : MonoBehaviour
{
    public float minLifetime;
    public float maxLifetime;

    private float deathTime;
    private void Awake()
    {
        deathTime = Time.time + Random.Range(minLifetime, maxLifetime);
    }

    private void Update()
    {
        if (deathTime <= Time.time)
        {
            TruffleSpawner.Instance?.ExpireTruffle(this.gameObject);
            Destroy(this);
        }
    }
}
