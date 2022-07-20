using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [Header("Visuals")] 
	[SerializeField] private Transform _bulletVisualParent;
    [SerializeField] ExplosionFX _explosionFX;

    [Header("Settings")]
    [SerializeField] private BulletSettings _bulletSettings;

    [Serializable]
    public class BulletSettings 
    {
        public LayerMask hitMask;
        public float areaRadius;
        public float areaImpulse;
        public byte areaDamage;
        public float speed;
        public float radius;
        public float gravity;
        public float timeToLive;
        public float timeToFade;
        public float ownerVelocityMultiplier;
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

    [Networked]
	public TickTimer bulletDespawnTimer { get; set; }
    private const float DELAY = 0.15f;

    private List<LagCompensatedHit> _areaHits = new List<LagCompensatedHit>();

    public void InitNetworkState(Vector3 ownervelocity, Transform gun)
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, _bulletSettings.timeToLive + _bulletSettings.timeToFade);
        fadeTimer = TickTimer.CreateFromSeconds(Runner, _bulletSettings.timeToFade);
       
        destroyed = false;
        
        velocity = gun.right * _bulletSettings.speed + ownervelocity;
        
    }

    public override void Spawned()
    {
        if (_explosionFX != null)
        {
            _explosionFX.ResetExplosion();
        }
        _bulletVisualParent.gameObject.SetActive(true);

        if (transform.Find("Explosion") != null)
            transform.Find("Explosion").gameObject.SetActive(true);

        GetComponent<NetworkTransform>().InterpolationDataSource = InterpolationDataSources.NoInterpolation;

    }

    public override void FixedUpdateNetwork()
    {
        MoveBullet();
        if (bulletDespawnTimer.Expired(Runner))
        {
            _bulletVisualParent.gameObject.SetActive(false);
            NetworkObject self = GetComponent<NetworkObject>();
            Runner.Despawn(self, false);
        }
    }

    private void MoveBullet()
    {
        Transform xfrm = transform;
        float dt = Runner.DeltaTime;
        Vector3 vel = velocity;
        float speed = vel.magnitude;
        Vector3 pos = xfrm.position;

        if (!destroyed)
        {
            if (lifeTimer.Expired(Runner))
            {
                Detonate(transform.position);
            }
            else 
            {
                //transform.position += velocity * Runner.DeltaTime; 
                velocity = vel;
                pos += dt * velocity;

                xfrm.position = pos;
                Vector3 dir = vel.normalized;
                
                if (Runner.LagCompensation.Raycast(transform.position, dir, Mathf.Max(_bulletSettings.radius, speed * dt), Object.InputAuthority, out var hitinfo, _bulletSettings.hitMask.value, HitOptions.IncludePhysX))
                {
                    //Debug.Log("hit something");
                    vel = HandleImpact(hitinfo);
                    pos = hitinfo.Point;
                }
            }
        } 
        else 
        {
            velocity = Vector3.zero;
        }

        
        // if(vel.sqrMagnitude>0)
        //     _bulletVisualParent.position = vel.normalized;
    }

    private void Detonate(Vector3 hitPoint)
    {
        if (destroyed)
            return;
        // Mark the bullet as destroyed.
        // This will trigger the OnDestroyedChanged callback which makes sure the explosion triggers correctly on all clients.
        // Using an OnChange callback instead of an RPC further ensures that we don't trigger the explosion in a different frame from
        // when the bullet stops moving (That would lead to moving explosions, or frozen bullets)
        destroyed = true;

        if (_bulletSettings.areaRadius > 0)
        {
            ApplyAreaDamage(hitPoint); 
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
            if (_explosionFX != null)
            {
                //transform.up = Vector3.up;
                _explosionFX.PlayExplosion();
            }
            bulletDespawnTimer = TickTimer.CreateFromSeconds(Runner, DELAY);
            
        }
    }

    private void ApplyAreaDamage(Vector3 hitPoint)
    {
        var inputauth = Object.InputAuthority;
        var hbm = Runner.LagCompensation;
        int cnt = hbm.OverlapSphere(hitPoint, _bulletSettings.areaRadius, inputauth, _areaHits, _bulletSettings.hitMask, HitOptions.IncludePhysX);
        
        if (cnt > 0)
        {
            for (int i = 0; i < cnt; i++)
            {
                GameObject other = _areaHits[i].GameObject;
               
                if (other && other.tag == "Player")
                {
                    
                    ICanTakeDamage target = other.GetComponent<ICanTakeDamage>();
                    if (target != null)
                    {
                        
                        Vector3 impulse = other.transform.position - hitPoint;
                        float l = Mathf.Clamp(_bulletSettings.areaRadius - impulse.magnitude, 0, _bulletSettings.areaRadius);
                        impulse = _bulletSettings.areaImpulse * l * impulse.normalized;
                        target.ApplyDamage(impulse, _bulletSettings.areaDamage, Object.InputAuthority);
                    }
                }
            }
        }
    }

    private Vector3 HandleImpact(LagCompensatedHit hit)
    {
        
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