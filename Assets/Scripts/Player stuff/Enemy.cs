using System;
using UnityEngine;
using Fusion;
using Random = UnityEngine.Random;
/** 
* Enemy AI Tiers (Defined by Ryan).
* Level 1: Able to chase and shoot the Player
* Level 2: Able to dynamically move, avoid obstacles and choose whether to chase the player.
* Level 3: Teamplay with other enemies
*/
public class Enemy : NetworkBehaviour
{

    Transform targetLocation;
    Player enemyInput;
    public Transform enemyTransform;
    Player target;
    Vector2 aimDirection;
    Vector2 moveDirection;
    public NetworkCharacterControllerPrototypeCustom controller;
    float retreatDistance = 4.5f;
    float moveSpeed = 4.5f;
    float shootingRange = 6.0f;
    float chasingRange = 10f;
    bool isChasing = false;
    private float rand1;
    private float rand2;
    private float rand3;
    int wanderCount = 0;
    public Collider2D wallDetection;
    private RaycastHit2D[] _obstacles = new RaycastHit2D[1];


    void Start()
    {
        enemyInput = GetComponent<Player>();
    }

    public override void Spawned()
    {
        base.Spawned();
        enemyInput.InitEnemyState();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            // reset target if target exists, once target dies
            // OR this enemy is dead, reset everything
            if ((target != null && target.state == Player.State.Dead) || enemyInput.state == Player.State.Dead)
            {
                targetLocation = null;
                target = null;
                isChasing = false;
            }

            if (targetLocation != null && enemyTransform != null)
            { // target exists
              // sets Mouse location
                SetDirection(targetLocation);

                // stops chasing the target if exits range 
                if (Vector2.Distance(enemyTransform.position, targetLocation.position) > chasingRange)
                {
                    Debug.Log("Target Lost");
                    targetLocation = null;
                    // enemyRb.velocity = moveDirection * 0;
                    target = null;
                    isChasing = false;
                }
            }
        }
        catch (InvalidOperationException e)
        {
            return;
        }
    }

  public override void FixedUpdateNetwork() 
    {
        moveDirection.x = aimDirection.x - enemyTransform.position.x;
        moveDirection.y = aimDirection.y - enemyTransform.position.y;
        
        moveDirection.Normalize();

        // shoot if player within range
        if (isChasing == true)
        {
            // if distance to target is less than desired distance, retreat & shoot
            if (Vector2.Distance(enemyTransform.position, targetLocation.position) <= retreatDistance) 
            {
                //// check for obstacles in the retreat direction 
                //wallDetection.Raycast(moveDirection * -1, _obstacles, 2.0f);

                //// obstacles exist, rush diagonally at the player
                //if (wallDetection.Raycast(moveDirection * -1, _obstacles, 1.0f) > 0)
                //{
                //    Debug.Log("Obstacle detected");
                //    controller.Move(Vector2.Lerp(moveDirection, Vector2.Perpendicular(moveDirection), 0.5f), moveSpeed);

                //// no obstacles, retreat
                //}
                //else
                //{
                    controller.Move(moveDirection * -1, moveSpeed);
                    enemyInput.Shoot(moveDirection);
                //}


                // if distance is suitable for shooting, shoot
            } else if (Vector2.Distance(enemyTransform.position, targetLocation.position) <= shootingRange && enemyInput.state != Player.State.Dead) 
            {
                enemyInput.Shoot(moveDirection);

            // else, move closer 
            } else {
                controller.Move(moveDirection, moveSpeed);
            }
        // wandering in a fixed direction, with no enemies
        } else if (wanderCount > 0) {

        }
    }

    // Aims at target, uses Player.cs to set mouse and facing direction
    private void SetDirection(Transform target)
    {
        rand1 = Random.value;

        //// 50% chance for enemy to misfire
        //if (rand1 <= 0.5)
        //{
        //    aimDirection = LousyShooting(targetLocation.position);
        //} else
        //{
        //    aimDirection = targetLocation.position;
        //}
        aimDirection = targetLocation.position;
        enemyInput.setMouse(aimDirection);
    }

    public void SetTarget(GameObject target) 
    {
        if (targetLocation == null) {
            Debug.Log("Target Set");
            Debug.Log(target.name);
            targetLocation = target.transform;
            this.target = target.GetComponent<Player>();
            isChasing = true;
        }
    }

    // Adding a chance to miss to enemyAI
    public Vector2 LousyShooting(Vector2 dir)
    {
        // constant for sufficient displacement in world
        rand1 = Random.value;
        rand2 = Random.value;

        // random displacement
        rand3 = Random.value;
        if (rand3 > 0.5)
        {
            rand1 *= -1;
        } 

        dir.x += rand1 * 3;
        dir.y += rand2 * 3;

        return dir;
    }
    
    // To be Implemented
    public void Wander()
    {

    }


    // Enemy tier 1 characteristics:
    // Shoot at the player
    // Walk towards the player
    // Detect the player
    // Wander in playable area (implement when map bounds are set)

    // Steps:
    // Wander in the map
    // if player enters aggression zone, target that player
}
