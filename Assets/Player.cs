using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{

    [Header("Visuals")] 
	[SerializeField] private SpriteRenderer sprite;

    private NetworkCharacterControllerPrototypeCustom _cc;
    public Animator animator;
    public GameObject firePoint;

    private Vector2 mouseDirection;
    
    [Networked(OnChanged = nameof(OnStateChanged))]
    private Direction direction { get; set; }

    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototypeCustom>();
    }

    /// <summary>
    /// Render is the Fusion equivalent of Unity's Update() and unlike FixedUpdateNetwork which is very different from FixedUpdate,
    /// Render is in fact exactly the same. It even uses the same Time.deltaTime time steps. The purpose of Render is that
    /// it is always called *after* FixedUpdateNetwork - so to be safe you should use Render over Update if you're on a
    /// SimulationBehaviour.
    ///
    /// Here, we use Render to update visual aspects of the Tank that does not involve changing of networked properties.
    /// </summary>
    public override void Render()
    {
        SetDirections();
    }

    public virtual void setMouse(Vector2 mouseDirection) 
    {
        this.mouseDirection = mouseDirection;
    }

    public virtual void SetDirections()
    {
        Vector2 lookDir = Vector2.zero;
        lookDir.x = mouseDirection.x - _cc.transform.position.x;
        lookDir.y = mouseDirection.y - _cc.transform.position.y;
        float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg;
        //left is 180/-180, right is 0. top is 90, bottom is -90
        //return values: up is 0, right is 1, down is 2, left is 3
        if (angle >= 45f && angle < 135f) {
            direction = Direction.UP;
        } else if (angle < 45f && angle >= -45f) {
            direction = Direction.RIGHT;
        } else if (angle < -45f && angle >= -135f) {
            direction = Direction.DOWN;
        } else {
            direction = Direction.LEFT;
        }
    }

    public static void OnStateChanged(Changed<Player> changed)
		{
			if(changed.Behaviour)
				changed.Behaviour.setAnimation();
		}

    private void setAnimation() {
        switch (direction)
			{
				case Direction.UP:
                    animator.SetFloat("Speed", 0);
                    sprite.flipX = false;
                    break;
				case Direction.RIGHT:
                    animator.SetFloat("Speed", 1);
                    sprite.flipX = false;
					break;
				case Direction.DOWN:
                    animator.SetFloat("Speed", 0);
                    sprite.flipX = true;
					break;
				case Direction.LEFT:
                    animator.SetFloat("Speed", 1);
                    sprite.flipX = true;
					break;
			}
    }

    // private void animate(int direction) {
    //     if (direction == RIGHT || direction == LEFT) {
    //         animator.SetFloat("Speed", 1); //to update, 1 is temp value

    //     if (!isRight && direction == RIGHT) {
    //         FlipHorizontal();
    //         isRight = true;
    //     } else if (isRight && direction == LEFT){
    //         FlipHorizontal();
    //         isRight = false;
    //     }
    //     } else {
    //         animator.SetFloat("Speed", 0); //to update, 0 is temp value
    //     }
    // }

  private void FlipHorizontal() {
    sprite.flipX = !sprite.flipX;

    Vector3 curScaleGun = firePoint.transform.localScale;
    curScaleGun.x *= -1;
    curScaleGun.y *= -1;
    firePoint.transform.localScale = curScaleGun;
  }     
}
