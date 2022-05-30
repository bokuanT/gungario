using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeCustom controller;
    public Animator animator;
    public GameObject playerModel;
    public GameObject firePoint;
    public Camera cam;
    Vector2 mouseInputVector = Vector2.zero;
    private const int UP = 0;
    private const int RIGHT = 1;
    private const int DOWN = 2;
    private const int LEFT = 3;
    public bool isRight = true;
 
    private void Awake()
    {
        controller = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }


    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            //Move
            Vector2 moveDirection = networkInputData.movementInput;
            
            moveDirection.Normalize();
        
            controller.Move(moveDirection);

        }

            //Rotate character
            // Vector2 mouseDirection = networkInputData.mouseInput;
            // int direction = Utils.getDirection(controller.transform.position, mouseDirection);
            // Debug.Log("Direction is " + direction);
            // setDirections(direction);
    }
    //decides what value to feed to animator so sprite faces correct way
    private void setDirections(Vector2 mouseDirection) { 
        // Direction of mouse 
        Vector2 lookDir = Vector2.zero;
        lookDir.x = mouseDirection.x - controller.transform.position.x;
        lookDir.y = mouseDirection.y - controller.transform.position.y;
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
