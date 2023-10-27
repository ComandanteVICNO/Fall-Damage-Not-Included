using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerCamera : MonoBehaviour
{

    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform cameraHolder;

    public Camera playercam;

    public float originalFov;
    private int InvertMouseControls;

    float xRotation;
    float yRotation;

    private void Awake()
    {
        InvertMouseControls = PlayerPrefs.GetInt("InvertMouse");

        sensX = PlayerPrefs.GetFloat("HorizontalSensitivity");
        sensY = PlayerPrefs.GetFloat("VerticalSensitivity");

        //Get Fov Value from PLayerPrefs
        originalFov = PlayerPrefs.GetFloat("Fov");

        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Set Fov
        playercam.fieldOfView = originalFov;

    }

    private void Update()
    {
        //receber input do rato

        if(InvertMouseControls == 0)
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");
            yRotation += mouseX * sensX;
            xRotation -= mouseY * sensY;
        }
        else if(InvertMouseControls == 1) 
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = -1f * Input.GetAxisRaw("Mouse Y");
            yRotation += mouseX * sensX;
            xRotation -= mouseY * sensY;
        }

        //Impedir a camera de olhar
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rodar camera e orientação
        cameraHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

    }
    
    //Fov and Tilt modifier for wallrunning
    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
