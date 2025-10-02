using System;

using UnityEngine;

namespace inonego
{
    public class GroundChecker2D : GroundCheckerBase
    {        

    #region 필드
    
        [SerializeField]
        private Transform groundCheckOrigin = null;
        
        [SerializeField]
        private LayerMask groundLayer = 0;
        public LayerMask GroundLayer
        {
            get => groundLayer;
            set => groundLayer = value;
        }

        [SerializeField]
        private float depth = 0.1f;
        public float Depth
        {
            get => depth;
            set => depth = value;
        }

        [SerializeField]
        private float radius = 0.1f;
        public float Radius
        {
            get => radius;
            set => radius = value;
        }

        public override Vector3 GroundVelocity => groundRigid != null ? groundRigid.linearVelocity : Vector2.zero;
        public override Vector3 Gravity => Physics2D.gravity;
        
        private Rigidbody2D rigid = null;
        private Rigidbody2D groundRigid = null;

    #endregion

    #region 생성자

        public GroundChecker2D(Rigidbody2D rigid)
        {
            if (rigid == null)
            {
                throw new ArgumentNullException("Rigidbody2D가 null입니다. 생성자에서 초기화해주세요.");
            }

            this.rigid = rigid;
        }

    #endregion

    #region 메서드

        protected override GameObject Detect()
        {
            if (groundCheckOrigin == null)
            {
                return null;
            }
            
            var direction = Gravity.sqrMagnitude > 0f ? Gravity.normalized : Vector3.down;

            var hit = Physics2D.CircleCast(groundCheckOrigin.position, radius, direction, depth, groundLayer);

            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        protected override void ProcessGround(GameObject prev, ref GameObject next)
        { 
            groundRigid = null;

            if (next == null) 
            {
                return;
            }

            var nextGroundRigid = next.GetComponent<Rigidbody2D>();
            var groundVelocity = nextGroundRigid != null ? nextGroundRigid.linearVelocity : Vector2.zero;

            var (velocity, gravity) = (rigid.linearVelocity, Gravity);

            // 바닥으로 향하고 있는지 확인합니다.
            var isHeadingToGround = IsHeadingToGround(velocity, groundVelocity, gravity);

            if (!isHeadingToGround)
            {
                next = null;
                groundRigid = null;

                return;
            }

            groundRigid = nextGroundRigid;
        }

    #endregion
    
    }
}