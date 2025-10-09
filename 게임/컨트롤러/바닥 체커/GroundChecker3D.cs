using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    [RequireComponent(typeof(Rigidbody))]
    public class GroundChecker3D : GroundChecker<Rigidbody, Collider>
    {

    #region 필드

        public override Vector3 Gravity => Physics.gravity;
        
        // GC 할당 방지를 위한 재사용 가능한 콜라이더 배열
        private readonly Collider[] overlappingColliders = new Collider[1];

    #endregion

    #region 생성자

        public GroundChecker3D(GameObject gameObject) : base(gameObject) {}

    #endregion

    #region 메서드

        protected override Vector3 GetLinearVelocity(Rigidbody rigidbody) => rigidbody.linearVelocity;
        protected override bool CheckColliderAvailable(Collider collider) => collider.enabled;

        // -------------------------------------------------------------
        /// <summary>
        /// Collider를 사용하여 바닥을 감지합니다.
        /// </summary>
        // -------------------------------------------------------------
        protected override GameObject DetectWithCollider(Collider collider, float deltaTime)
        {
            if (collider is BoxCollider boxCollider)
            {
                return DetectWithBoxCollider(boxCollider, deltaTime);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                return DetectWithSphereCollider(sphereCollider, deltaTime);
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                return DetectWithCapsuleCollider(capsuleCollider, deltaTime);
            }

            return null;
        }


        // ------------------------------------------------------------
        /// <summary>
        /// <br/>BoxCollider를 사용하여 바닥을 감지합니다.
        /// <br/>바닥면에서 시작해서 BoxCast를 수행합니다.
        /// </summary>
        // ------------------------------------------------------------
        private GameObject DetectWithBoxCollider(BoxCollider boxCollider, float deltaTime)
        {
            var info = GetBoxColliderDetectionInfo(boxCollider, deltaTime);

            var center = info.Center - info.Direction * GroundCheckerConfig.Thickness * 0.5f;
            var size = new Vector3(info.Size.x, GroundCheckerConfig.Thickness, info.Size.z);

            var orientation = boxCollider.transform.rotation;

            // ------------------------------------------------------------
            // 먼저 초기 위치에서 OverlapBox 체크
            // ------------------------------------------------------------
            int overlapCount = Physics.OverlapBoxNonAlloc(center, size * 0.5f, overlappingColliders, orientation, Config.Layer);
            
            if (overlapCount > 0)
            {
                return overlappingColliders[0].gameObject;
            }

            // ------------------------------------------------------------
            // OverlapBox에서 감지되지 않으면 BoxCast 수행
            // ------------------------------------------------------------
            if (Physics.BoxCast(center, size * 0.5f, info.Direction, out RaycastHit hit, orientation, info.Depth, Config.Layer, QueryTriggerInteraction.Ignore))
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// <br/>SphereCollider를 사용하여 바닥을 감지합니다.
        /// <br/>먼저 초기 위치에서 OverlapSphere를 체크하고, 없으면 SphereCast를 수행합니다.
        /// </summary>
        // ------------------------------------------------------------
        private GameObject DetectWithSphereCollider(SphereCollider sphereCollider, float deltaTime)
        {
            var info = GetSphereColliderDetectionInfo(sphereCollider, deltaTime);
            
            // ------------------------------------------------------------
            // 먼저 초기 위치에서 OverlapSphere 체크
            // ------------------------------------------------------------
            int overlapCount = Physics.OverlapSphereNonAlloc(info.Center, info.Radius, overlappingColliders, Config.Layer);
            
            if (overlapCount > 0)
            {
                return overlappingColliders[0].gameObject;
            }

            // ------------------------------------------------------------
            // OverlapSphere에서 감지되지 않으면 SphereCast 수행
            // ------------------------------------------------------------
            if (Physics.SphereCast(info.Center, info.Radius, info.Direction, out RaycastHit hit, info.Depth, Config.Layer, QueryTriggerInteraction.Ignore))
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        // ------------------------------------------------------------------------------
        /// <summary>
        /// <br/>CapsuleCollider를 사용하여 바닥을 감지합니다.
        /// <br/>먼저 초기 위치에서 OverlapSphere를 체크하고, 없으면 SphereCast를 수행합니다.
        /// </summary>
        // ------------------------------------------------------------------------------
        private GameObject DetectWithCapsuleCollider(CapsuleCollider capsuleCollider, float deltaTime)
        {
            var info = GetCapsuleColliderDetectionInfo(capsuleCollider, deltaTime);
            
            // ------------------------------------------------------------
            // 수직 캡슐
            // ------------------------------------------------------------
            if (info.Flag)
            {
                // ------------------------------------------------------------
                // 먼저 초기 위치에서 OverlapSphere 체크
                // ------------------------------------------------------------
                int overlapCount = Physics.OverlapSphereNonAlloc(info.Center, info.Radius, overlappingColliders, Config.Layer);
                
                if (overlapCount > 0)
                {
                    return overlappingColliders[0].gameObject;
                }

                // ------------------------------------------------------------
                // OverlapSphere에서 감지되지 않으면 SphereCast 수행
                // ------------------------------------------------------------
                if (Physics.SphereCast(info.Center, info.Radius, info.Direction, out RaycastHit hit, info.Depth, Config.Layer, QueryTriggerInteraction.Ignore))
                {
                    return hit.collider.gameObject;
                }
            }
            // ------------------------------------------------------------
            // 수평 캡슐
            // BoxCast 사용 (빈 구현)
            // ------------------------------------------------------------
            else
            {
                // 수평 캡슐은 현재 구현되지 않음
            }

            return null;
        }

    #endregion

    #region Box 범위 계산

        // ------------------------------------------------------------
        /// <summary>
        /// BoxCollider의 바닥 감지 계산 정보를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection GetBoxColliderDetectionInfo(BoxCollider boxCollider, float deltaTime)
        {
            return GetBoxColliderDetectionInfo(boxCollider.transform, boxCollider.center, boxCollider.size, deltaTime);
        }

        private GroundCheckerDetection GetBoxColliderDetectionInfo(Transform boxTransform, Vector3 boxCenter, Vector3 boxSize, float deltaTime)
        {
            var info = GroundCheckerCalculation.Create(boxTransform);

            // ------------------------------------------------------------
            // 방향 계산
            // ------------------------------------------------------------
            var worldDirection = info.WorldDirection;

            // ------------------------------------------------------------
            // 크기 계산
            // ------------------------------------------------------------
            var scale = Vector3.Scale(boxSize, boxTransform.lossyScale);
            
            var size = new Vector3(scale.x, 0f, scale.z);

            // ------------------------------------------------------------
            // 중심점 계산
            // ------------------------------------------------------------
            Vector3 localCenter = boxCenter;
            Vector3 worldCenter = boxTransform.TransformPoint(localCenter);
            
            // 바닥면의 중심점을 계산합니다.
            worldCenter += worldDirection * scale.y * 0.5f;

            // ------------------------------------------------------------
            // 깊이 계산
            // ------------------------------------------------------------
            var depth = GetDepth(worldDirection, deltaTime);

            return new GroundCheckerDetection(worldCenter, size, worldDirection, depth);
        }

    #endregion

    #region Sphere 범위 계산

        // ------------------------------------------------------------
        /// <summary>
        /// SphereCollider의 바닥 감지 계산 정보를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection GetSphereColliderDetectionInfo(SphereCollider sphereCollider, float deltaTime)
        {
            return GetSphereColliderDetectionInfo(sphereCollider.transform, sphereCollider.center, sphereCollider.radius, deltaTime);
        }

        private GroundCheckerDetection GetSphereColliderDetectionInfo(Transform sphereTransform, Vector3 sphereCenter, float sphereRadius, float deltaTime)
        {
            var info = GroundCheckerCalculation.Create(sphereTransform);

            // ------------------------------------------------------------
            // 중심점 계산
            // ------------------------------------------------------------
            Vector3 localCenter = sphereCenter;
            Vector3 worldCenter = sphereTransform.TransformPoint(localCenter);

            // ------------------------------------------------------------
            // 방향 계산
            // ------------------------------------------------------------
            var worldDirection = info.WorldDirection;

            // ------------------------------------------------------------
            // 반지름 계산
            // ------------------------------------------------------------
            var worldScale = sphereTransform.lossyScale;
            
            // 월드 스케일 적용된 반지름
            var worldRadius = sphereRadius * Mathf.Max(worldScale.x, worldScale.y, worldScale.z);

            // ------------------------------------------------------------
            // 깊이 계산
            // ------------------------------------------------------------
            var depth = GetDepth(worldDirection, deltaTime);

            return new GroundCheckerDetection(worldCenter, worldRadius, worldDirection, depth);
        }

    #endregion

    #region Capsule 범위 계산

        // ------------------------------------------------------------
        /// <summary>
        /// CapsuleCollider의 바닥 감지 계산 정보를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection GetCapsuleColliderDetectionInfo(CapsuleCollider capsuleCollider, float deltaTime)
        {
            var capsuleTransform = capsuleCollider.transform;
            var (capsuleCenter, capsuleRadius, capsuleHeight) = (capsuleCollider.center, capsuleCollider.radius, capsuleCollider.height);

            if (capsuleCollider.direction == 1) // Y축 (수직)
            {
                var info = GroundCheckerCalculation.Create(capsuleTransform);

                // ------------------------------------------------------------
                // 중심점 계산
                // ------------------------------------------------------------
                Vector3 localCenter = capsuleCenter;
                Vector3 worldCenter = capsuleTransform.TransformPoint(localCenter);

                // ------------------------------------------------------------
                // 방향 계산
                // ------------------------------------------------------------
                var worldDirection = info.WorldDirection;

                // ------------------------------------------------------------
                // 크기 계산
                // ------------------------------------------------------------
                var worldScale = capsuleTransform.lossyScale;
                
                var worldRadius = capsuleRadius * Mathf.Max(worldScale.x, worldScale.z);
                var worldHeight = capsuleHeight * worldScale.y;

                var yOffset = Mathf.Max(0f, worldHeight * 0.5f - worldRadius);

                // 바닥면의 중심점을 계산합니다.
                worldCenter += worldDirection * yOffset;

                // ------------------------------------------------------------
                // 깊이 계산
                // ------------------------------------------------------------
                var depth = GetDepth(worldDirection, deltaTime);

                return new GroundCheckerDetection(worldCenter, worldRadius, worldDirection, depth, true);
            }
            else
            {
                // 수평 캡슐 - 빈 구현
                return new GroundCheckerDetection(Vector3.zero, Vector3.zero, 0f, Vector3.down, 0f, false);
            }
        }

    #endregion
    
    }
}
