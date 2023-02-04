using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PigController : NetworkBehaviour
{
    public float runSpeed = 1f;
    public GameObject localObjects;

    private bool selectInput;
    public void OnSelect(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            selectInput = false;
        else
            selectInput = true;
    }

    private Vector2 movementInput;
    public void OnMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
            movementInput = Vector2.zero;
        else
            movementInput = ctx.ReadValue<Vector2>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLocalPlayer)
        {
            Destroy(localObjects);
        }
    }

    private Transform cameraMain;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        cameraMain = Camera.main.transform;

        var cineMachine = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        cineMachine.Follow = this.transform;
        cineMachine.LookAt = this.transform;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        var movement = new Vector3(movementInput.x, 0f, movementInput.y);
        var m_CamForward = Vector3.Scale(cameraMain.forward, new Vector3(1, 0, 1)).normalized;
        movement = movement.z * m_CamForward + movement.x * cameraMain.right;
        movement.y = 0f;

        this.transform.position = this.transform.position + movement * Time.deltaTime * runSpeed;

        if (movement.magnitude > 0)
            this.transform.LookAt(this.transform.position + movement);

        //Select is only valid for a single frame
        selectInput = false;
    }

}
