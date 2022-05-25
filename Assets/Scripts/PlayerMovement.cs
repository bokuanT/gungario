
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Rigidbody2D gun;
    public Camera cam;
    public Animator animator;
    public GameObject playerModel;
    public GameObject firePoint;

    Vector2 movement;
    Vector2 mousePos;
    public bool isRight = true;

    private const int UP = 0;
    private const int RIGHT = 1;
    private const int DOWN = 2;
    private const int LEFT = 3;

    

    void Start()
    {
        cam = Camera.main;

        cam.GetComponentInChildren<CinemachineVirtualCamera>().Follow = gameObject.transform;
        
    }

    // Keypress
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Gets the mouse position
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        //animator.SetFloat("Speed", Mathf.Abs(movement.x));
        setDirections();  

        //Debug.Log(firePointTransform.right);
    }

    // Movement variables
    void FixedUpdate() {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

    }

    //decides what value to feed to animator so sprite faces correct way
    private void setDirections() { 
        // Direction of mouse 
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg;
        int direction = getDirection(angle);

        if (direction == RIGHT || direction == LEFT) {
            animator.SetFloat("Speed", 1); //to update, 1 is temp value
            if (!isRight && direction == RIGHT) {
                FlipHorizontal();
                isRight = true;
            } else if (isRight && direction == LEFT){
                FlipHorizontal();
                isRight = false;
            }
        } else {
            animator.SetFloat("Speed", 0); //to update, 0 is temp value
        }
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

    private void FlipHorizontal() {
        Vector3 curScalePlayer = playerModel.transform.localScale;
        curScalePlayer.x *= -1;
        playerModel.transform.localScale = curScalePlayer;

        Vector3 curScaleGun = firePoint.transform.localScale;
        curScaleGun.x *= -1;
        curScaleGun.y *= -1;
        firePoint.transform.localScale = curScaleGun;
    }
}