using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeCustom controller;

    Vector2 mouseInputVector = Vector2.zero;
    
 
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

            controller.SetDirections(networkInputData.mouseInput);

        }

            //Rotate character
            // Vector2 mouseDirection = networkInputData.mouseInput;
            // int direction = Utils.getDirection(controller.transform.position, mouseDirection);
            // Debug.Log("Direction is " + direction);
            // setDirections(direction);
    }
    

    
    
    
}
