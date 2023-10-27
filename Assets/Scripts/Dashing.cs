using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Camera playerCam;
    public PlayerCamera playerCameraScript;
    private Rigidbody rb;
    private PlayerMovement pm;
    private float fovValue;

    [Header("Dashing")]
    public float dashForce;
    public float dashDuration;

    [Header("Cooldown")]
    public float dashCooldown;
    private float dashCooldownTimer;
    private bool canDash = true;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    private void Start()
    {
        fovValue = playerCameraScript.originalFov;
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(dashKey) && canDash && !pm.grounded) 
        {
            Dash();
        }
        if(dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void Dash()
    {
        if (dashCooldownTimer > 0) return;

        else dashCooldownTimer = dashCooldown;
        pm.dashing = true;



        if (!pm.grounded)
        {
            canDash = false;
        }

        pm.playerAudioSource.PlayOneShot(pm.emptySound);
        pm.playerAudioSource.pitch = 1f;
        pm.playerAudioSource.PlayOneShot(pm.dashSound);

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 forceToApply = orientation.forward * dashForce;

        playerCam.DOFieldOfView(80f, 0.1F);

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        pm.dashing = false;
        playerCam.DOFieldOfView(fovValue, 0.1F);
        rb.velocity = new Vector3(0f, 0f, 0f); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        canDash = true;
    }
}
