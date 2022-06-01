using System;
using Fusion;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[OrderBefore(typeof(NetworkTransform))]
[DisallowMultipleComponent]
// ReSharper disable once CheckNamespace
public class NetworkCharacterControllerPrototypeCustom : NetworkTransform {
  // [Header("Character Controller Settings")]
  // public float gravity       = 0f;
  // public float jumpImpulse   = 8.0f;
  // public float acceleration  = 10.0f;
  // public float braking       = 10.0f;
  // public float maxSpeed      = 10.0f;
  // public float rotationSpeed = 15.0f;
  public float moveSpeed = 5f;
  private const int UP = 0;
  private const int RIGHT = 1;
  private const int DOWN = 2;
  private const int LEFT = 3;
  public bool isRight = true;

  // [Networked]
  // [HideInInspector]
  // public bool IsGrounded { get; set; }

  // [Networked]
  // [HideInInspector]
  // public Vector3 Velocity { get; set; }

  /// <summary>
  /// Sets the default teleport interpolation velocity to be the CC's current velocity.
  /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToPosition"/>.
  /// </summary>
  //protected override Vector3 DefaultTeleportInterpolationVelocity => Velocity;

  /// <summary>
  /// Sets the default teleport interpolation angular velocity to be the CC's rotation speed on the Z axis.
  /// For more details on how this field is used, see <see cref="NetworkTransform.TeleportToRotation"/>.
  /// </summary>
  //protected override Vector3 DefaultTeleportInterpolationAngularVelocity => new Vector3(0f, 0f, rotationSpeed);

  public CharacterController Controller { get; private set; }
  public Animator animator;
  public GameObject playerModel;
  public GameObject firePoint;

  protected override void Awake() {
    base.Awake();
    CacheEverything();
  }

  public override void Spawned() {
    base.Spawned();
    CacheEverything();

    // Caveat: this is needed to initialize the Controller's state and avoid unwanted spikes in its perceived velocity
    Controller.Move(transform.position);
  }

  private void CacheEverything() {
    if (Controller == null) {
      Controller = GetComponent<CharacterController>();

      //Assert.Check(Controller != null, $"An object with {nameof(NetworkCharacterControllerPrototype)} must also have a {nameof(CharacterController)} component.");
    }
  }

  protected override void CopyFromBufferToEngine() {
    // Trick: CC must be disabled before resetting the transform state
    Controller.enabled = false;

    // Pull base (NetworkTransform) state from networked data buffer
    base.CopyFromBufferToEngine();

    // Re-enable CC
    Controller.enabled = true;
  }

  /// <summary>
  /// Basic implementation of a character controller's movement function based on an intended direction.
  /// <param name="direction">Intended movement direction, subject to movement query, acceleration and max speed values.</param>
  /// </summary>
  public virtual void Move(Vector2 direction) {
    var deltaTime = Runner.DeltaTime;
    Controller.Move(direction * moveSpeed * deltaTime);
  }

  public virtual void SetDirections(Vector2 mouseDirection) {
    // Direction of mouse 
    Vector2 lookDir = Vector2.zero;
    lookDir.x = mouseDirection.x - Controller.transform.position.x;
    lookDir.y = mouseDirection.y - Controller.transform.position.y;
    float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg;
    int direction = getDirection(angle);
    if (direction == RIGHT ) {
      animator.SetFloat("Speed", 1); //to update, 1 is temp value
      // if (!isRight && direction == RIGHT) {
      //   FlipHorizontal();
      //   isRight = true;
      // } else if (isRight && direction == LEFT){
      //   FlipHorizontal();
      //   isRight = false;
      // }
    } else if (direction == LEFT) 
    {
      animator.SetFloat("Speed", -1);
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
