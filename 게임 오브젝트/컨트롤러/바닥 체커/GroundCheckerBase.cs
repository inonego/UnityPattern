using System;
using UnityEngine;

namespace inonego
{

using inonego.util;

public abstract class GroundCheckerBase<TRigidbody, TCollider> : MonoBehaviour

where TRigidbody    : Component
where TCollider     : Component
{

#region 이벤트

    protected Event<GroundCheckerBase<TRigidbody, TCollider>, GroundEventArgs> OnGroundEnterEvent = new();
    public event Action<GroundCheckerBase<TRigidbody, TCollider>, GroundEventArgs> OnGroundEnter { add => OnGroundEnterEvent += value; remove => OnGroundEnterEvent -= value; }

    protected Event<GroundCheckerBase<TRigidbody, TCollider>, GroundEventArgs> OnGroundExitEvent = new();
    public event Action<GroundCheckerBase<TRigidbody, TCollider>, GroundEventArgs> OnGroundExit { add => OnGroundExitEvent += value; remove => OnGroundExitEvent -= value; }

    public struct GroundEventArgs
    {
        public GameObject GameObject;
        public TRigidbody Rigidbody;
        public TCollider Collider;
    }

#endregion

    public float Depth;
    
    public bool IsGrounded { get; private set; } = false;

    protected GroundEventArgs? currentGround;
    
    protected new TRigidbody rigidbody;
    protected new TCollider  collider;
    
    protected bool IsTypeValid() => (typeof(TRigidbody) == typeof(Rigidbody) || typeof(TRigidbody) == typeof(Rigidbody2D)) &&
                                    (typeof(TCollider)  == typeof(Collider)  || typeof(TCollider)  == typeof(Collider2D));
    
    protected virtual void Awake()
    {
        if (!IsTypeValid())
        {
            throw new Exception("TRigidbody에는 Rigidbody나 Rigidbody2D, TCollider에는 Collider나 Collider2D으로만 타입을 지정할 수 있습니다.");
        }
        
        rigidbody = GetComponent<TRigidbody>();
        collider  = GetComponent<TCollider>();
    }

    protected virtual void FixedUpdate()
    {
        Check();
    }

    protected void Check()
    {
        SetCurrentGround(Cast());
    }

    protected void SetCurrentGround(TCollider collider)
    {
        bool previousIsGrounded = IsGrounded;

        GroundEventArgs? previousGround = currentGround;

        bool IsCurrentGroundChanged() => previousGround?.GameObject != collider?.gameObject;

        if (IsCurrentGroundChanged())
        {
            if (collider is not null)
            {
                currentGround = new GroundEventArgs()
                {
                    GameObject  = collider.gameObject,
                    Rigidbody   = collider.GetComponent<TRigidbody>(),
                    Collider    = collider,
                };
            }
            else
            {
                currentGround = null;
            }
        }
        
        IsGrounded = currentGround is not null && IsHeadingToGround();
        
        if (previousGround is not null)
        {
            if (previousIsGrounded && (!IsGrounded || IsCurrentGroundChanged()))
            {
                OnGroundExitEvent?.InvokeHere(this, previousGround.Value);
            }
        }

        if ( currentGround is not null)
        {
            if (IsGrounded && (!previousIsGrounded || IsCurrentGroundChanged()))
            { 
                OnGroundEnterEvent?.InvokeHere(this, currentGround.Value);
            }
        }
    }

    protected abstract TCollider Cast();
    
    protected bool IsHeadingToGround()
    {
        return Vector3.Dot(GetVelocity() - GetGroundVelocity(), GetGravity()) <= 0f;
    }

    protected abstract Vector3 GetVelocity();
    protected abstract Vector3 GetGroundVelocity();
    protected abstract Vector3 GetGravity();
    
}

}