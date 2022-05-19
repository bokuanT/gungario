using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLogic : MonoBehaviour
{
    public Rigidbody2D rb;
    public Camera cam;

    Vector2 mousePos;
    

    // Update is called once per frame
    void Update()
    {
         // Gets the mouse position
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate() {
        setDirections();
    }

    private void setDirections() {
        // Direction of mouse 
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg;
        //Debug.Log(angle);
        rb.rotation = angle;
    }

}
