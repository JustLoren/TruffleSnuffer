using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PigController : NetworkBehaviour
{
    public GameObject localObjects;

    #region Coloring
    [SyncVar] public Color color;
    public MeshRenderer indicator;
    private void Colorize()
    {
        indicator.material.color = color;

        if (!isLocalPlayer)
        {
            InGameUI.Instance.AddCompetitor(this);
        } else
        {
            InGameUI.Instance.ColorizePlayer(color);
        }
    }
    #endregion

    #region Snuffling
    [SyncVar(hook = nameof(UpdateTruffleCount))] 
    public int trufflesGathered = 0;
    public ParticleSystem smellParticples;
    public List<AudioClip> snortClips;
    public AudioClip collectTruffleClip;
    public AudioSource audioSrc;    
    private float particleEmissionRate;

    private Dictionary<GameObject, float> nearbyTruffles = new Dictionary<GameObject, float>();

    public void AddTruffle(GameObject truffle, float distance)
    {
        if (nearbyTruffles.ContainsKey(truffle))
            nearbyTruffles[truffle] = distance;
        else
            nearbyTruffles.Add(truffle, distance);
    }

    public void RemoveTruffle(GameObject truffle)
    {
        nearbyTruffles.Remove(truffle);
    }

    private void Update()
    {
        if (particleEmissionRate <= 0)
        {
            var initEmission = smellParticples.emission;
            particleEmissionRate = initEmission.rateOverTimeMultiplier;
            initEmission.rateOverTimeMultiplier = 0f;
        }

        if (!isLocalPlayer) return;

        (_, var minDistance) = ClosestTruffle();

        var emission = smellParticples.emission;

        var invMinDist = 1 - minDistance;
        emission.rateOverTimeMultiplier = (invMinDist * invMinDist) * particleEmissionRate;

        if (minDistance < 1)
        {
            DoSnort(minDistance);
        }

        //Select is only valid for a single frame
        if (selectInput && !InGameUI.IsPaused)
            TriggerSelect();

        selectInput = false;
    }

    private float lastSnort = 0f;
    private void DoSnort(float distance)
    {
        float delay;
        if (distance < .3f)
            delay = .5f;
        else if (distance < .6f)
            delay = 1f;
        else
            delay = 1.5f;

        if (lastSnort + delay <= Time.time)
        {
            audioSrc.PlayOneShot(snortClips[Random.Range(0, snortClips.Count)]);
            lastSnort = Time.time;
        }
    }

    private (GameObject, float) ClosestTruffle()
    {        
        KeyValuePair<GameObject, float> pair = new KeyValuePair<GameObject, float>(null, 1f);
        foreach(var truffle in nearbyTruffles.Keys.ToList())
        {
            if (truffle == null)
                nearbyTruffles.Remove(truffle);
        }    
        foreach(var truffle in nearbyTruffles)
        {
            if (truffle.Value < pair.Value)
            {
                pair = truffle;
            }
        }

        return (pair.Key, pair.Value);
    }

    [Tooltip("How far away can a truffle be gathered, measured in percentage of truffle detection range.")]
    public float gatherDistance = .1f;
    private void TriggerSelect()
    {
        (var truffle, var distance) = ClosestTruffle();
        if (truffle != null && distance <= gatherDistance)
        {
            GatherTruffle(truffle);
        } 
        else
        {
            Debug.Log("Failed truffle gathering. Womp womp!");
        }
    }

    [Command]
    private void GatherTruffle(GameObject truffle)
    {
        if (truffle != null)
        {
            Destroy(truffle);
            trufflesGathered++;
        }
    }

    private void UpdateTruffleCount(int _old, int _new)
    {
        if (_new > _old)
        {
            audioSrc.PlayOneShot(collectTruffleClip);
        }

        if (isLocalPlayer)
        {
            InGameUI.Instance.SetTruffleCount(_new);

            if (_new > _old)
                InGameUI.Instance.SpawnTruffleGainFX();
        } 
        else
        {
            InGameUI.Instance.SetOpponentScore(this, _new);
        }
    }
    #endregion

    #region Locomotion BS
    public float runSpeed = 1f;
    public new CapsuleCollider collider;
    public Rigidbody rb;
    public PhysicMaterial walkMaterial, standMaterial;

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

        Colorize();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

    private Transform cameraMain;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        InGameUI.Instance.Show();

        cameraMain = Camera.main.transform;

        var cineMachine = FindObjectOfType<Cinemachine.CinemachineFreeLook>();
        cineMachine.Follow = this.transform;
        cineMachine.LookAt = this.transform;
        cineMachine.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;        
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer || InGameUI.IsPaused) return;

        HandleMovement();
    }

    private void HandleMovement()
    {
        if (movementInput == Vector2.zero)
            collider.sharedMaterial = standMaterial;
        else
            collider.sharedMaterial = walkMaterial;        

        var movement = new Vector3(movementInput.x, 0f, movementInput.y);
        var m_CamForward = Vector3.Scale(cameraMain.forward, new Vector3(1, 0, 1)).normalized;
        movement = movement.z * m_CamForward + movement.x * cameraMain.right;
        movement.y = 0f;

        rb.AddForce(movement * runSpeed, ForceMode.Acceleration);

        if (movement.magnitude > 0)
        {
            rb.MoveRotation(Quaternion.LookRotation(movement));
        }
    }
    #endregion

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            InGameUI.Instance?.Hide();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;                        
        } else
        {
            InGameUI.Instance?.RemoveCompetitor(this);
        }
    }
}
