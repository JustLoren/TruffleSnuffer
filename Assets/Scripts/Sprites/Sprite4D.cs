using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite4D : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite frontSprite, backSprite, sideSprite;
    private Transform cameraMain;
    private void Awake()
    {
        cameraMain = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 camPos = cameraMain.position;
        this.transform.LookAt(new Vector3(camPos.x, transform.position.y, camPos.z));
                
        var signedAngle = Vector3.SignedAngle(cameraMain.forward, this.transform.parent.forward, Vector3.up);
        Debug.Log("Signed angle: " + signedAngle);

        if (Mathf.Abs(signedAngle) >= 67.5f && Mathf.Abs(signedAngle) <= 112.5f)
        {
            spriteRenderer.sprite = sideSprite;
            if (signedAngle < 0)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;

            if (Mathf.Abs(signedAngle) < 67.5f)
                spriteRenderer.sprite = backSprite;
            else
                spriteRenderer.sprite = frontSprite;
        }
    }
}
