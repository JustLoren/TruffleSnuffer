using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autorotate : MonoBehaviour
{
    public bool randomStartOffset = false;    
    [Range(-100f, 100f)]
    public float minRandomValue, maxRandomValue;
    public bool randomSpeed = false;
    public Vector3 rotationSpeed;
    public Space rotationMode = Space.Self;
    public bool useUnscaledTime;

    private void Start()
    {
        if (randomStartOffset)
        {
            var randomOffset = Random.Range(minRandomValue, maxRandomValue);
            transform.Rotate(-rotationSpeed * randomOffset, rotationMode);

            if (randomOffset < 0)
                rotationSpeed = -rotationSpeed;
        }

        if (randomSpeed)
        {
            rotationSpeed *= UnityEngine.Random.Range(.25f, 1f);

            if (UnityEngine.Random.value > .5f)
                rotationSpeed = -rotationSpeed;
        }
    }

    private void Update()
    {
        if (useUnscaledTime)
        {
            if (Time.unscaledDeltaTime > .5f)
                return;

            transform.Rotate(rotationSpeed * Time.unscaledDeltaTime, rotationMode);
        }
        else
            transform.Rotate(rotationSpeed * Time.deltaTime, rotationMode);

    }

    public void Rotate(float timeAmount)
    {
        if (useUnscaledTime)
        {
            transform.Rotate(rotationSpeed * timeAmount, rotationMode);
        }
        else
            transform.Rotate(rotationSpeed * timeAmount, rotationMode);
    }
}
