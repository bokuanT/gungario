using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour, ICanTakeDamage
{

    [Header("Visuals")] 
	[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer weaponSprite;

    [Networked]//(OnChanged = nameof(OnStateChanged))]
	public State state { get; set; }

    private NetworkWeapon networkWeapon;
    private Collider _collider;
	private HitboxRoot _hitBoxRoot;
    public Animator animator;
    public Transform player;
    public Transform gun;
    private Vector2 mouseDirection;
    private Vector2 aimDirection;

    // Temporary variable to move shooting here
    public float moveSpeed = 5f;
    public const byte MAX_HEALTH = 100;

    [Networked(OnChanged = nameof(OnStateChanged))]
    private Direction direction { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
	public byte life { get; set; }

    [Networked]
	private TickTimer respawnTimer { get; set; }

    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public enum State
    {
        New,
        Despawned,
        Spawning,
        Active,
        Dead
    }
    public bool isActivated => (gameObject.activeInHierarchy && (state == State.Active || state == State.Spawning));
	public bool isDead => state == State.Dead;
	public bool isRespawningDone => state == State.Spawning && respawnTimer.Expired(Runner);

    void Awake()
    {
        networkWeapon = GetComponentInChildren<NetworkWeapon>();
        _collider = GetComponentInChildren<Collider>();
		_hitBoxRoot = GetComponent<HitboxRoot>();
    }

    public void InitNetworkState()
    {
        life = MAX_HEALTH;
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
        //_collider.enabled = state != State.Dead;
		_hitBoxRoot.enabled = state == State.Active;
    }

    public virtual void setMouse(Vector2 mouseDirection) 
    {
        this.mouseDirection = mouseDirection;
    }

    private void SetDirections()
    {
        aimDirection.x = mouseDirection.x - player.position.x;
        aimDirection.y = mouseDirection.y - player.position.y;

        // Gun direction
        // Current Issue :
        // In multiplayer, other players' guns keep pointing towards origin
        // I believe this is because aimDirection is deafult to (0,0) for other players
        gun.right = Vector2.Lerp(gun.right, new Vector2(aimDirection.x,aimDirection.y), Runner.DeltaTime * 5f);

        float angle = Mathf.Atan2(aimDirection.y ,aimDirection.x) * Mathf.Rad2Deg;
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
        // player and gun sprite direction
        switch (direction)
			{
				case Direction.UP:
                    animator.SetFloat("Speed", 0);
                    sprite.flipX = false;
                    break;
				case Direction.RIGHT:
                    animator.SetFloat("Speed", 1);
                    sprite.flipX = false;
                    weaponSprite.flipY = false;
					break;
				case Direction.DOWN:
                    animator.SetFloat("Speed", 0);
                    sprite.flipX = true;
					break;
				case Direction.LEFT:
                    animator.SetFloat("Speed", 1);
                    sprite.flipX = true;
                    weaponSprite.flipY = true;
					break;
			}
  
    }

    public virtual void Shoot(Vector2 mvDir)
    {
        var deltaTime = Runner.DeltaTime;
        networkWeapon.Fire(Runner, Object.InputAuthority, mvDir * moveSpeed * deltaTime);
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

//   private void FlipHorizontal() {
//     sprite.flipX = !sprite.flipX;

//     Vector3 curScaleGun = firePoint.transform.localScale;
//     curScaleGun.x *= -1;
//     curScaleGun.y *= -1;
//     firePoint.transform.localScale = curScaleGun;
//   } 
    //Apply impulse for future updates- visual feedback from taking dmg by moving
    public void ApplyDamage(Vector3 impulse, byte damage, PlayerRef attacker)
    {
        // if (!isActivated) //TODO implement invulnerability
		// {	
        //     Debug.Log("not activated");	
        //     return;
        // }

        Player attackingPlayer = Spawner.Get(attacker);
        
        if (attackingPlayer != null && attackingPlayer == this)
        {    
            return;
        }
        
        life -= damage;
		Debug.Log($"Player {this} took {damage} damage, life = {life}");

    }
}
