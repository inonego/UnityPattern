using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 제네릭 GroundChecker 클래스
    /// </summary>
    // ============================================================
    public abstract class GroundChecker<TRigidbody, TCollider> : GroundCheckerBase
        where TRigidbody : Component
        where TCollider : Component
    {
        [SerializeField]
        public GroundCheckerConfig Config;

        [SerializeField, ReadOnly]
        private GameObject gameObject = null;
        public override GameObject GameObject => gameObject;

        [SerializeField]
        private TRigidbody rigid = null;
        public TRigidbody Rigid => rigid;
        
        [SerializeField]
        private TCollider[] colliders = null;
        public TCollider[] Colliders => colliders;

        [SerializeField, ReadOnly]
        private TRigidbody groundRigid = null;
        public TRigidbody GroundRigid => groundRigid;

        // -------------------------------------------------------------
        /// <summary>
        /// 리지드바디에서 선형 속도를 가져옵니다.
        /// </summary>
        // -------------------------------------------------------------
        protected abstract Vector3 GetLinearVelocity(TRigidbody rigidbody);
        protected abstract bool CheckColliderAvailable(TCollider collider);

        public override Vector3 Velocity
        {
            get => rigid != null ? GetLinearVelocity(rigid) : Vector3.zero;
        }

        public override Vector3 GroundVelocity
        {
            get => groundRigid != null ? GetLinearVelocity(groundRigid) : Vector3.zero;
        }

        // -------------------------------------------------------------
        /// <summary>
        /// 생성자
        /// </summary>
        // -------------------------------------------------------------
        public GroundChecker(GameObject gameObject)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException("GameObject가 null입니다. 생성자에서 초기화해주세요.");
            }

            this.gameObject = gameObject;
            
            this.rigid = gameObject.GetComponent<TRigidbody>();
            this.colliders = gameObject.GetComponents<TCollider>();
            
            if (rigid == null)
            {
                throw new ArgumentNullException($"{typeof(TRigidbody).Name}가 null입니다. GameObject에 {typeof(TRigidbody).Name} 컴포넌트가 필요합니다.");
            }

            if (colliders == null || colliders.Length == 0)
            {
                throw new ArgumentNullException($"{typeof(TCollider).Name}가 null입니다. GameObject에 {typeof(TCollider).Name} 컴포넌트가 필요합니다.");
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 바닥을 처리합니다.
        /// </summary>
        // ------------------------------------------------------------------------------------------------------------------------
        protected override void ProcessGround(GameObject prev, ref GameObject next)
        { 
            groundRigid = null;

            if (next == null) 
            {
                return;
            }

            // 바닥 오브젝트의 리지드바디를 가져옵니다.
            // 없는 경우에는 바닥의 속도를 0으로 설정합니다.
            var nextGroundRigid = next.GetComponent<TRigidbody>();
            var groundVelocity = nextGroundRigid != null ? GetLinearVelocity(nextGroundRigid) : Vector3.zero;

            var (velocity, gravity) = (GetLinearVelocity(rigid), Gravity);

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

        // -------------------------------------------------------------
        /// <summary>
        /// 바닥을 감지합니다.
        /// </summary>
        // -------------------------------------------------------------
        protected override GameObject Detect(float deltaTime)
        {
            foreach (var collider in Colliders)
            {
                if (!CheckColliderAvailable(collider)) continue;

                var detected = DetectWithCollider(collider, deltaTime);

                // 바닥이 감지되면 즉시 반환
                if (detected != null)
                {
                    return detected;
                }
            }

            return null;
        }

        // -------------------------------------------------------------
        /// <summary>
        /// 컬라이더를 사용하여 바닥을 감지합니다.
        /// </summary>
        // -------------------------------------------------------------
        protected abstract GameObject DetectWithCollider(TCollider collider, float deltaTime);

        // -------------------------------------------------------------
        /// <summary>
        /// 체크할 깊이를 구합니다.
        /// </summary>
        // -------------------------------------------------------------
        protected float GetDepth(Vector3 vector, float deltaTime)
        {
            var depthByGround = Vector3.Dot(GroundVelocity - Velocity, vector.normalized) * deltaTime;
            return Config.Depth + Mathf.Max(0f, depthByGround);
        }
    }
}
