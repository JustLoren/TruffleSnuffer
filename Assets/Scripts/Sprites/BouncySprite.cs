using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncySprite : MonoBehaviour
{
    private Vector3 lastPosition;

    private float targetElevation;
    private Quaternion targetRotation;

    public float minBounceElevation;
    public float maxBounceElevation;
    public float minBounceAngle;
    public float maxBounceAngle;


    private float currentBounceFactor = 0f;
    public float bounceFadeFactor = 5f;
    public float bounceSpeed = 5f;
    public float rotationSpeed = 25f;
    public float rotationFrequency = 2f;
    
    private float bounceProgress = 0f;
    private bool bounceDirectionUp;
    private float rotationProgress = 0f;
    private bool rotationDirectionMax;
    void Update()
    {
        if (transform.parent.position != lastPosition)
        {
            lastPosition = transform.parent.position;
            currentBounceFactor = Mathf.Clamp01(currentBounceFactor + Time.deltaTime * bounceFadeFactor);
        }
        else
        {
            currentBounceFactor = Mathf.Clamp01(currentBounceFactor - Time.deltaTime * bounceFadeFactor);
        }

        if (rotationDirectionMax)
        {
            rotationProgress = Mathf.Clamp01(rotationProgress + Time.deltaTime * rotationFrequency);
            targetRotation = Quaternion.Euler(0, 0, minBounceAngle);
            if (rotationProgress == 1f)
                rotationDirectionMax = false;
        } else
        {
            rotationProgress = Mathf.Clamp01(rotationProgress - Time.deltaTime * rotationFrequency);
            targetRotation = Quaternion.Euler(0, 0, maxBounceAngle);
            if (rotationProgress == 0f)
                rotationDirectionMax = true;
        }

        if (bounceDirectionUp)
        {
            bounceProgress = Mathf.Clamp01(bounceProgress + Time.deltaTime);
            
            targetElevation = Random.Range(minBounceElevation, maxBounceElevation);
            if (bounceProgress == 1f)
                bounceDirectionUp = false;
        }
        else
        {
            bounceProgress = Mathf.Clamp01(bounceProgress - Time.deltaTime);
            
            targetElevation = Random.Range(minBounceElevation, maxBounceElevation);
            if (bounceProgress == 0f)
                bounceDirectionUp = true;
        }
        var pos = transform.localPosition;
        pos.y = Mathf.MoveTowards(pos.y, targetElevation, Time.deltaTime * bounceSpeed * currentBounceFactor);
        transform.localPosition = pos;

        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed * currentBounceFactor);
    }
}
