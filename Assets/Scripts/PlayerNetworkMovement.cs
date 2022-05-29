using Fusion;

public class PlayerNetworkMovement : NetworkBehaviour
{
  private NetworkCharacterControllerPrototypeCustom _cc;

  private void Awake()
  {
    _cc = GetComponent<NetworkCharacterControllerPrototypeCustom>();
  }

  public override void FixedUpdateNetwork()
  {
    if (GetInput(out NetworkInputData data))
    {
      data.movementInput.Normalize();
      _cc.Move(5*data.movementInput*Runner.DeltaTime);
    }
  }
}