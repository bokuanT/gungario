using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 mouseInputVector = Vector2.zero;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        // Camera cam = gameObject.GetComponentInChildren<Camera>();
        // cam.transform.position += new Vector3(0,0,-10);
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInputVector.x = Input.GetAxisRaw("Horizontal");
        moveInputVector.y = Input.GetAxisRaw("Vertical");

        // Gets the mouse position
        // somehow redundant
        mouseInputVector = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.movementInput = moveInputVector;
        networkInputData.mouseInput = mouseInputVector;

        return networkInputData;
    }
}
