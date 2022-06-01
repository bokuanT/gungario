using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;

    // cant be here since there is no input when only the mouse is moving
    public Vector2 mouseInput;
    
    public float rotationInput;

    public byte MOUSEBUTTON1;

    public byte buttons;

    
}
