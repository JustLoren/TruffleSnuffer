using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAlert : MonoBehaviour
{
    public Transform spawnPoint;
    public SphereCollider sensor;
    private void OnTriggerStay(Collider other)
    {
        var pig = other.GetComponentInParent<PigController>();

        if (pig != null && pig.isLocalPlayer)
        {
            var distance = (other.transform.position - transform.position).magnitude;
            var distancePct = Mathf.Clamp01(distance / (sensor.radius + .5f));

            pig.AddTruffle(this.gameObject, distancePct);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var pig = other.GetComponentInParent<PigController>();

        if (pig != null && pig.isLocalPlayer)
        {
            pig.RemoveTruffle(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        TruffleSpawner.Instance?.RestoreSpawnPoint(spawnPoint);
    }
}
