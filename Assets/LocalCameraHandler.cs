using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    Camera localCamera;

    // Update is called once per frame
    private void Awake()
    {
        localCamera = GetComponent<Camera>();
    }

    void Start() 
    {
        //Detach Camera
        if (localCamera.enabled)
        {
            localCamera.transform.parent = null;
        }

        Camera[] arr = Camera.allCameras;
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i] != localCamera) arr[i].enabled = false;
        }
    }

    void LateUpdate() 
    {
        if (cameraAnchorPoint == null) return;

        if (!localCamera.enabled) return;

        // Move the camera to position
        localCamera.transform.position = cameraAnchorPoint.position;


    }
}
