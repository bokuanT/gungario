using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLogic : MonoBehaviour
{
    public Rigidbody2D gun;
    public Rigidbody2D rb;
    public Camera cam;
    public Animator animator;

    Vector2 mousePos;

    private const int UP = 0;
    private const int RIGHT = 1;
    private const int DOWN = 2;
    private const int LEFT = 3;
    
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
         // Gets the mouse position
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
    }

    void FixedUpdate() {
        setDirectionsGun();
    }

    private int getDirection(float angle) {
        //left is 180/-180, right is 0. top is 90, bottom is -90
        //return values: up is 0, right is 1, down is 2, left is 3
        if (angle >= 45f && angle < 135f) {
            return 0;
        } else if (angle < 45f && angle >= -45f) {
            return 1;
        } else if (angle < -45f && angle >= -135f) {
            return 2;
        } else {
            return 3;
        }
    }

    private void setDirectionsGun() {
        // Direction of mouse 
        Vector2 lookDir = mousePos - gun.position;
        float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg;
        gun.rotation = angle;
        
        gun.MoveRotation(gun.rotation + 50.0f * Time.fixedDeltaTime);

        int direction = getDirection(angle);

        if (direction == RIGHT || direction == LEFT) {
            animator.SetBool("IsVertical", false); //to update, 1 is temp value
            float magnitude = direction == RIGHT ? 0.5f : -0.5f;
            Vector2 vector = new Vector2(magnitude, 0);
            gun.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime + vector);
        } else {
            animator.SetBool("IsVertical", true); //to update, 0 is temp value
            float magnitude = direction == UP ? 0.8f : -0.8f;
            Vector2 vector = new Vector2(0, magnitude);
            gun.MovePosition(rb.position + rb.velocity * Time.fixedDeltaTime + vector);
        }

    }

}
