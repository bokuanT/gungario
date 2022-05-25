using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //local only
    }

    public override void FixedUpdateNetwork() 
    {
        if (GetInput(out NetworkInputData networkInputData))
        {
            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y
                + transform.right * networkInputData.movementInput.x;
            
            moveDirection.Normalize();
        

            networkCharacterControllerPrototypeCustom.Move(moveDirection);

        }
    }
}
