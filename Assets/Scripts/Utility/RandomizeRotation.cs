using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeRotation : MonoBehaviour
{
    public Vector3 axis = Vector3.up;

    private void Start()
    {
        this.transform.localRotation *= Quaternion.AngleAxis(Random.value * 360f, axis);
    }
}
