
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    public Camera cam;
    public Animator animator;
    public GameObject gameObject;

    Vector2 movement;
    Vector2 mousePos;
    public bool isRight = true;

    // Keypress
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Speed", Mathf.Abs(movement.x));
        
        // Gets the mouse position
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    // Movement variables
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        
        // Direction of mouse 
        // Vector2 lookDir = mousePos - rb.position;
        // float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg - 90f;
        // rb.rotation = angle;

        if (movement.x > 0 && !isRight) {
            FlipHorizontal();
            isRight = true;
        } else if (movement.x < 0 && isRight) {
            FlipHorizontal();
            isRight = false;
        }
    }

    private void FlipHorizontal() {
        Vector3 curScale = gameObject.transform.localScale;
        curScale.x *= -1;
        gameObject.transform.localScale = curScale;
        isRight = !isRight;
    }
}