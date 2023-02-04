using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform cameraMain;
    void Start()
    {
        cameraMain = Camera.main.transform;
    }
        
    void LateUpdate()
    {
        this.transform.LookAt(cameraMain.position);
    }
}
