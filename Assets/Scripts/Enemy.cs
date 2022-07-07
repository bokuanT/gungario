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
    Vector2 aimDirection;
    Vector2 moveDirection;
    public NetworkCharacterControllerPrototypeCustom controller;
    float retreatDistance = 4.0f;
    float moveSpeed = 4.5f;
    float shootingRange = 4.5f;
    float chasingRange = 10f;
    bool isChasing = false;

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
        if (enemyInput.state == Player.State.Active)
        {
            if (targetLocation != null && enemyTransform != null)
            { // target exists
              // sets Mouse location
                setDirection(targetLocation);

                // stops chasing the target if exits range 
                if (Vector2.Distance(enemyTransform.position, targetLocation.position) > chasingRange)
                {
                    Debug.Log("Target Lost");
                    targetLocation = null;
                    // enemyRb.velocity = moveDirection * 0;
                    isChasing = false;
                }
            }
        }
    }

  public override void FixedUpdateNetwork() 
    {
        if (enemyInput.state == Player.State.Active)
        {
            moveDirection.x = aimDirection.x - enemyTransform.position.x;
            moveDirection.y = aimDirection.y - enemyTransform.position.y;

            // ============= ISSUE =============
            // Shooting is fked up here
            // =================================
            moveDirection.Normalize();

            // shoot if player within range
            if (isChasing == true)
            {
                if (Vector2.Distance(enemyTransform.position, targetLocation.position) <= retreatDistance)
                {
                    controller.Move(moveDirection * -1, moveSpeed);
                }
                else if (Vector2.Distance(enemyTransform.position, targetLocation.position) <= shootingRange && enemyInput.state != Player.State.Dead)
                {
                    //enemyInput.Shoot(moveDirection);
                    float rand = Random.value;
                    // if (rand <= 0.4) { // move along x vector
                    //     controller.Move(new Vector2(moveDirection.x, 0), moveSpeed);
                    // } else if (rand >= 0.6) { // move along y vector
                    //     controller.Move(new Vector2(0, moveDirection.x), moveSpeed);
                    // } else {
                    //     // Stay
                    // }
                }
                else
                {
                    controller.Move(moveDirection, moveSpeed);
                }
            }
        }
    }

    // Aims at target, uses Player.cs to set mouse and facing direction
    private void setDirection(Transform target)
    {
        aimDirection = targetLocation.position;
        enemyInput.setMouse(aimDirection);
    }

    public void setTarget(GameObject target) 
    {
        if (targetLocation == null) {
            Debug.Log("Target Set");
            targetLocation = target.transform;
            isChasing = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // ============= TO IMPLEMENT =============
        // No targetting of each other
        // =======================================
        // Debug.Log("Enemy has sensed " + col.name);
        setTarget(col.gameObject);
        
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
