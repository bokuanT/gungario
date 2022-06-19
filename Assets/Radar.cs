using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public Enemy enemy;

    void OnTriggerStay2D(Collider2D col)
    {
        // Debug.Log("Enemy has sensed " + col.name);
        
        // dont target fellow enemy, don't target (for now, til teams are implemented)
        // DOESNT WORK, STILL TARGETS ENEMIES
        if (col.gameObject.GetComponent<Enemy>() == null) 
        {
            enemy.setTarget(col.gameObject);
        }
    }
}
