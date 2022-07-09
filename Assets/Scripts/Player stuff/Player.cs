using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour, ICanTakeDamage
{

    [Header("Visuals")] 
	[SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer weaponSprite;
    [SerializeField] public SpriteRenderer hatSprite;

    [SerializeField] private const float RESPAWN_TIME = 3f;
    [SerializeField] private float _pickupRadius;
    [SerializeField] private LayerMask _pickupMask;

    [Networked(OnChanged = nameof(OnStateChanged))]
	public State state { get; set; }

    private WeaponManager weaponManager;
    private Collider _collider;
	private HitboxRoot _hitBoxRoot;
    public Animator animator;
    public Transform player;
    private Transform gun;
    private Transform firePoint;
    private Vector2 mouseDirection;
    private Vector2 lookDir;
    private DeathManager _deathManager;
    private PlayerRef thisPlayerRef;
    public Scoreboard_item scoreboard_item;
    private GameObject scoreboardItemManager;
    public GameObject cursor;
    private Collider[] _overlaps = new Collider[1];

    // Temporary variable to move shooting here
    public float moveSpeed = 5f;
    public const byte MAX_HEALTH = 100;

    [Networked(OnChanged = nameof(OnStateChanged))]
    private Direction direction { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public float gunDirection { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
	public byte life { get; set; }

    [Networked]
	private TickTimer respawnTimer { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
	public byte kills { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
	public byte deaths { get; set; }

    [Networked]
    public string playerName { get; set; }

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
        weaponManager = GetComponentInChildren<WeaponManager>();
        _collider = GetComponentInChildren<Collider>();
		_hitBoxRoot = GetComponent<HitboxRoot>();
        _deathManager = GetComponent<DeathManager>();
        
        // this avoids crashing enemy.cs since enemies do not have a cursor 
        if (cursor != null) Instantiate(cursor);
    }

    public override void Spawned()
    {
        base.Spawned();
        weaponManager.InitNetworkState();
    }

    public void InitNetworkState(PlayerRef pr)
    {
        life = MAX_HEALTH;
        state = State.Active;
        thisPlayerRef = pr;
        // playerName = name;
        kills = 0;
        deaths = 0;
        
    }

    // =========================== BUG ============================ 
    // Currently, enemies have are not spawned in when round starts,
    // meaning that damage taken pre-round is kept when round start
    public void InitEnemyState()
    {
        life = MAX_HEALTH;
        state = State.Active;
        weaponManager.InitNetworkState();
    }

    public override void FixedUpdateNetwork()
    {
        // if (Object.HasStateAuthority)
        // {
        if (state == State.Dead)
            { 
                /*
                if (respawnTimer.IsRunning)
                {
                    state = State.Spawning;
                }
                */
                if (respawnTimer.Expired(Runner))
                {
                    Transform thisTransform = GetComponent<Transform>();
                    thisTransform.position = Utils.GetRandomSpawnPoint(); //can make this follow Tell dont ask principle better
                    life = MAX_HEALTH;
                    state = State.Active;
                    setVisuals(true);

                }
            }

        //}

        CheckForWeaponPickup();

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
        if (gun != null && firePoint != null)
            SetDirections();

        if (state == State.Active)
            weaponManager.ShowCorrectWeapon();
        //_collider.enabled = state != State.Dead;
		_hitBoxRoot.enabled = state == State.Active;
    }

    public virtual void setMouse(Vector2 mouseDirection) 
    {
        this.mouseDirection = mouseDirection;
        lookDir.x = mouseDirection.x - player.position.x;
        lookDir.y = mouseDirection.y - player.position.y;
        
        // set networked variable 
        gunDirection = Vector2.SignedAngle(Vector2.up, lookDir) * Mathf.Deg2Rad * -1;
    }

    private void SetDirections()
    {

        // locally, set your own weapon
        gun.right = Vector2.Lerp(gun.right, new Vector2(lookDir.x, lookDir.y), Runner.DeltaTime * 5f);
        firePoint.right = Vector2.Lerp(firePoint.right, new Vector2(lookDir.x, lookDir.y), Runner.DeltaTime * 5f);
        firePoint.position = gun.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        
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
            changed.Behaviour.setState();
    }

    // controls player and gun sprite direction
    private void setAnimation() {
 
        // update remote players that you dont have authority for with their networked variable
        if (!Object.HasInputAuthority)
        {
            gun.right = Vector2.Lerp(gun.right, new Vector2(Mathf.Sin(gunDirection), Mathf.Cos(gunDirection)), Runner.DeltaTime * 5f);
            firePoint.right = Vector2.Lerp(firePoint.right, new Vector2(Mathf.Sin(gunDirection), Mathf.Cos(gunDirection)), Runner.DeltaTime * 5f);
        }


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
                // TEMPORARY FIX --> WONT NEED ONCE ENEMY PREFAB IS UPDATED
                if (hatSprite != null) hatSprite.flipX = true;
                break;
			case Direction.DOWN:
                animator.SetFloat("Speed", 0);
                sprite.flipX = true;
				break;
			case Direction.LEFT:
                animator.SetFloat("Speed", 1);
                sprite.flipX = true;
                weaponSprite.flipY = true;
                // TEMPORARY FIX --> WONT NEED ONCE ENEMY PREFAB IS UPDATED
                if (hatSprite != null) hatSprite.flipX = false;
				break;
		}
  
    }

    public void setState()
    {
        switch (state)
        {
            case State.Spawning:
                //TODO make spawn in animation
                break;
            case State.Active:
                //TODO add end spawn animation
                //hacky solution to spawner spawning player with z != 0
                Vector3 spawnPt = transform.position;
                spawnPt.z = 0;
                transform.position = spawnPt;
                break;
            case State.Dead:
                //TODO: add death animation
                // _deathExplosionInstance.transform.position = transform.position;
                // _deathExplosionInstance.SetActive(false); // dirty fix to reactivate the death explosion if the particlesystem is still active
                // _deathExplosionInstance.SetActive(true);
                _deathManager.OnDeath(Runner, Object.InputAuthority);
                setVisuals(false);
                StartRespawnSequence();
                break;
            case State.Despawned:
                //_teleportOut.StartTeleport();
                break;
        }
    }

    public virtual void Shoot(Vector2 mvDir)
    {
        var deltaTime = Runner.DeltaTime;
        if (weaponManager != null)
            weaponManager.Fire(Runner, Object.InputAuthority, mvDir * moveSpeed * deltaTime);
    } 

    //Apply impulse for future updates- visual feedback from taking dmg by moving
    public void ApplyDamage(Vector3 impulse, byte damage, PlayerRef attacker)
    {
        // if (!isActivated) //TODO implement invulnerability
        // {	
        //     Debug.Log("not activated");	
        //     return;
        // }

       
        //Player attackingPlayer = PlayerInfoManager.Get(NetworkRunner.GetRunnerForGameObject(gameObject), attacker);
        //Spawner attackingPlayertmp = Runner.gameObject.GetComponent<Spawner>();
        Player attackingPlayer = PlayerInfoManager.Get(Runner,attacker);
        

        if (attackingPlayer != null && attackingPlayer == this)
        {    
            return;
        }

        if (damage >= life)
        {
            if (state == State.Active)
            {
                life = 0;
                GetKilled();
                attackingPlayer.GetKill();
                state = State.Dead;
            }
        }
        
        else 
        {
        life -= damage;
		//Debug.Log($"Player {this} took {damage} damage, life = {life}");
        }
    }

    private void StartRespawnSequence()
    {
        respawnTimer = TickTimer.CreateFromSeconds(Runner, RESPAWN_TIME);
    }

    private void setVisuals(bool boolean)
    {
        Transform[] visuals = new Transform[3];
        visuals[0] = transform.Find("HealthUI");
        visuals[1] = weaponManager.GetActiveWeapon().transform.parent;
        visuals[2] = transform.Find("InterpolationRoot");
        
        foreach (Transform visual in visuals)
        {
            visual.gameObject.SetActive(boolean);
        }
    }

    public void GetKill()
    {
        this.kills += 1;
    }

    public void GetKilled()
    {
        this.deaths += 1;
    }

    public void ToggleOnScoreboard()
    {
        if (scoreboard_item != null)
        {
            if (scoreboardItemManager == null)
            {
                scoreboardItemManager = scoreboard_item.transform.parent.gameObject;
                
            }
            scoreboardItemManager.transform.localScale = new Vector3(1, 1, 1);
        }
        
    }

    public void ToggleOffScoreboard()
    {
        if (scoreboard_item != null)
        {
            if (scoreboardItemManager == null)
            {
                scoreboardItemManager = scoreboard_item.transform.parent.gameObject;

            }
            scoreboardItemManager.transform.localScale = new Vector3(0, 0, 0);
        }

    }

    public void ForceMakeHealthySetSpawn(Vector3 spawnPoint)
    {
        NetworkCharacterControllerPrototypeCustom cc = Object.GetComponent
            <NetworkCharacterControllerPrototypeCustom> ();
        cc.TeleportToPosition(spawnPoint);
        life = MAX_HEALTH;
    }

    public void Pickup(WeaponSpawnScript wepSpawner)
    {
        if (!wepSpawner)
            return;
        NetworkRunner runner = Runner;
        NetworkWeapon pickedUp = wepSpawner.Pickup();

        if (pickedUp == null)
            return;

        weaponManager.SetActiveWeapon(pickedUp, false);

    }

    private void CheckForWeaponPickup()
    {
        PhysicsScene scene = Runner.GetPhysicsScene();
        int overlaps = scene.OverlapSphere(transform.position, _pickupRadius, _overlaps, _pickupMask, QueryTriggerInteraction.Collide);
        if (state == State.Active && overlaps > 0)
        {
            Pickup(_overlaps[0].GetComponentInChildren<WeaponSpawnScript>());
        }
    }

    public void SetGunTransforms(NetworkWeapon wep)
    {
        gun = wep.transform.parent;
        firePoint = wep.transform;
    }

     
}
