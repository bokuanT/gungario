using System;
using Fusion;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [Header("Visuals")] 
	[SerializeField] private Transform _bulletVisualParent;
    [SerializeField] ExplosionFX _explosionFX;

    [Header("Settings")] 
    [SerializeField] private BulletSettings _bulletSettings = new BulletSettings();

    [Serializable]
    public class BulletSettings 
    {
        public LayerMask hitMask;
        public float areaRadius;
        public float areaImpulse;
        public byte areaDamage;
        public float speed = 10f;
        public float radius = 0.25f;
        public float gravity = -10f;
        public float timeToLive = 3f;
        public float timeToFade = 0.5f;
        public float ownerVelocityMultiplier = 1f;
	}

    [Networked]
    public TickTimer networkedLifeTimer { get; set; }
    private TickTimer _predictedLifeTimer;
    private TickTimer lifeTimer
    {
        get => Object.IsPredictedSpawn ? _predictedLifeTimer : networkedLifeTimer;
        set { if (Object.IsPredictedSpawn) _predictedLifeTimer = value;else networkedLifeTimer = value; }
    }

    [Networked]
    public TickTimer networkedFadeTimer { get; set; }
    private TickTimer _predictedFadeTimer;
    private TickTimer fadeTimer
    {
        get => Object.IsPredictedSpawn ? _predictedFadeTimer : networkedFadeTimer;
        set { if (Object.IsPredictedSpawn) _predictedFadeTimer = value;else networkedFadeTimer = value; }
    }

    [Networked]
    public Vector3 networkedVelocity { get; set; }
    private Vector3 _predictedVelocity;
    public Vector3 velocity
    {
        get => Object.IsPredictedSpawn ? _predictedVelocity : networkedVelocity;
        set { if (Object.IsPredictedSpawn) _predictedVelocity = value; else networkedVelocity = value; }
    }

    [Networked(OnChanged = nameof(OnDestroyedChanged))]
    public NetworkBool networkedDestroyed { get; set; }
    private bool _predictedDestroyed;
    private bool destroyed
    {
        get => Object.IsPredictedSpawn ? _predictedDestroyed : (bool)networkedDestroyed;
        set { if (Object.IsPredictedSpawn) _predictedDestroyed = value; else networkedDestroyed = value; }
    }


    public void InitNetworkState(Vector3 ownervelocity, Transform gun)
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, _bulletSettings.timeToLive + _bulletSettings.timeToFade);
        fadeTimer = TickTimer.CreateFromSeconds(Runner, _bulletSettings.timeToFade);

        destroyed = false;
        
        velocity = gun.right * _bulletSettings.speed + ownervelocity;
        Debug.Log( _bulletSettings.timeToLive + " bspeed");
        
    }

    // public override void Spawned()
    // {
    //     if (_explosionFX != null)
    //         _explosionFX.ResetExplosion();
    //     _bulletVisualParent.gameObject.SetActive(true);

    //     GetComponent<NetworkTransform>().InterpolationDataSource = InterpolationDataSources.Predicted;

    // }
    public override void FixedUpdateNetwork()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        //Debug.Log("MoveBullet. pos: " + gameObject.transform.position);
        
        if (!destroyed)
        {
            if (fadeTimer.Expired(Runner))
            {
                Detonate(transform.position);
            }
            else 
            {
                transform.position += velocity * Runner.DeltaTime; 
            }
        } 
        else 
        {
            velocity = Vector3.zero;
        }
        // Transform xfrm = transform;
        // float dt = Runner.DeltaTime;
        // Vector3 vel = velocity;
        // float speed = vel.magnitude;
        // Vector3 pos = xfrm.position;

        // if (!destroyed)
        // {
        //     if (fadeTimer.Expired(Runner))
        //     {
        //         Detonate(transform.position);
        //     }
        //     else
        //     {
        //         vel.y += dt * _bulletSettings.gravity;

        //         // We move the origin back from the actual position to make sure we can't shoot through things even if we start inside them
        //         Vector3 dir = vel.normalized;
        //         if (Runner.LagCompensation.Raycast(pos -0.5f*dir, dir, Mathf.Max(_bulletSettings.radius, speed * dt), Object.InputAuthority, out var hitinfo, _bulletSettings.hitMask.value, HitOptions.IncludePhysX))
        //         {
        //             vel = HandleImpact(hitinfo);
        //             pos = hitinfo.Point;
        //         }
        //     }
        // } else
        // {
        //     vel = Vector3.zero;
        //     dt = 0;
        // }

        // velocity = vel;
        // pos += dt * velocity;

        // xfrm.position = pos;
        // if(vel.sqrMagnitude>0)
        //     _bulletVisualParent.position = vel.normalized;
    }

    private void Detonate(Vector3 hitPoint)
    {
        Debug.Log("Detonate");
        if (destroyed)
            return;
        // Mark the bullet as destroyed.
        // This will trigger the OnDestroyedChanged callback which makes sure the explosion triggers correctly on all clients.
        // Using an OnChange callback instead of an RPC further ensures that we don't trigger the explosion in a different frame from
        // when the bullet stops moving (That would lead to moving explosions, or frozen bullets)
        destroyed = true;

        if (_bulletSettings.areaRadius > 0)
        {
            //ApplyAreaDamage(hitPoint); 
        }
    }

    public static void OnDestroyedChanged(Changed<NetworkBehaviour> changed)
    {
        ((Projectile)changed.Behaviour)?.OnDestroyedChanged();
    }

    private void OnDestroyedChanged()
    {
        if (destroyed)
        {
            Debug.Log("Destroyed");
            if (_explosionFX != null)
            {
                transform.up = Vector3.up;
                _explosionFX.PlayExplosion();
            }
            _bulletVisualParent.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    private Vector3 HandleImpact(LagCompensatedHit hit)
    {
        Debug.Log("Impact");
        if (hit.Hitbox != null)
        {
            NetworkObject netobj = hit.Hitbox.Root.Object;
            if (netobj != null && Object!=null && netobj.InputAuthority == Object.InputAuthority)
                return velocity; // Don't let us hit ourselves - this is esp. important with lag compensation since, if we move backwards, we're very likely to hit our own ghost from a previous frame.
        }

        Detonate(hit.Point);

        return Vector3.zero;
    }

}
