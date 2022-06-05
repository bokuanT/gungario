using System;
using Fusion;
using UnityEngine;
using System.Collections;

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

}
