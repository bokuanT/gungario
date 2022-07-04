using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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
    float retreatDistance = 4.0f;
    float moveSpeed = 4.5f;
    float shootingRange = 4.5f;
    float chasingRange = 10f;
    bool isChasing = false;
    private float rand1;
    private float rand2;
    private float rand3;

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
        if (targetLocation != null && enemyTransform != null) { // target exists
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

  public override void FixedUpdateNetwork() 
    {
        // reset target if target exists, once target dies
        // OR this enemy is dead, reset everything
        if ((target != null && target.state == Player.State.Dead) || enemyInput.state == Player.State.Dead)
        {
            targetLocation = null;
            target = null;
            isChasing = false;
        }

        if (target != null) Debug.Log(target);

        moveDirection.x = aimDirection.x - enemyTransform.position.x;
        moveDirection.y = aimDirection.y - enemyTransform.position.y;
        
        // ============= ISSUE =============
        // Shooting is fked up here
        // =================================
        moveDirection.Normalize();

        // shoot if player within range
        if (isChasing == true)
        {
            if (Vector2.Distance(enemyTransform.position, targetLocation.position) <= retreatDistance) {
                controller.Move(moveDirection * -1, moveSpeed);
            } else if (Vector2.Distance(enemyTransform.position, targetLocation.position) <= shootingRange && enemyInput.state != Player.State.Dead) {
                enemyInput.Shoot(moveDirection);

                // if (rand <= 0.4) { // move along x vector
                //     controller.Move(new Vector2(moveDirection.x, 0), moveSpeed);
                // } else if (rand >= 0.6) { // move along y vector
                //     controller.Move(new Vector2(0, moveDirection.x), moveSpeed);
                // } else {
                //     // Stay
                // }
            } else {
                controller.Move(moveDirection, moveSpeed);
            }
        }
    }

    // Aims at target, uses Player.cs to set mouse and facing direction
    private void SetDirection(Transform target)
    {
        rand1 = Random.value;
        
        // 40% chance for enemy to misfire
        if (rand1 <= 0.4)
        {
            aimDirection = LousyShooting(targetLocation.position);
        } else
        {
            aimDirection = targetLocation.position;
        }

        enemyInput.setMouse(aimDirection);
    }

    public void SetTarget(GameObject target) 
    {
        if (targetLocation == null) {
            Debug.Log("Target Set");
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

        dir.x += rand1 * 4;
        dir.y += rand2 * 4;

        return dir;
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
