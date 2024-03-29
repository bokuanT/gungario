using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    
    public Vector2 mouseInput;
    
    public float rotationInput;

    public const uint MOUSEBUTTON1 = 1 << 0;

    public uint Buttons;    

    public bool IsUp(uint button)
    {
        return IsDown(button) == false;
    }

    public bool IsDown(uint button)
    {
        return (Buttons & button) == button;
    }

    
}
