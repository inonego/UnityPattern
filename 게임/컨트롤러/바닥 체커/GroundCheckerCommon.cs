using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public struct GroundCheckerConfig
    {
        public LayerMask Layer;
        public float Depth;

        public static float Thickness = 0.001f;
    }

    [Serializable]
    public struct GroundCheckerCalculation
    {
        public Vector3 LocalDirection;
        public Vector3 WorldDirection, WorldVector;   
        public (Vector3 X, Vector3 Y, Vector3 Z) Basis;
        public float XAngle2D => GetAngle2D(Basis.X);
        public float YAngle2D => GetAngle2D(Basis.Y);
        public float ZAngle2D => GetAngle2D(Basis.Z);

        private float GetAngle2D(Vector3 vector) => Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

        public static GroundCheckerCalculation Create(Transform transform)
        {
            var result = new GroundCheckerCalculation();

            result.LocalDirection = Vector3.down;
            result.WorldDirection = transform.TransformDirection(result.LocalDirection);
            result.WorldVector = transform.TransformVector(result.LocalDirection);

            var matrix = transform.localToWorldMatrix;

            result.Basis.X = new Vector3(matrix.m00, matrix.m10, matrix.m20); // X축 벡터
            result.Basis.Y = new Vector3(matrix.m01, matrix.m11, matrix.m21); // Y축 벡터
            result.Basis.Z = new Vector3(matrix.m02, matrix.m12, matrix.m22); // Z축 벡터

            return result;
        }
    }

    // ============================================================
    /// <summary>
    /// 바닥 감지 계산 정보를 담는 구조체입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public struct GroundCheckerDetection
    {
        public Vector3 Size;
        public Vector3 Center, Direction;
        public float Depth, Radius;
        public float Angle;
        public bool Flag;

        public Quaternion Rotation;

        // ------------------------------------------------------------
        /// <summary>
        /// BoxCollider2D 전용 생성자
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection(Vector3 center, Vector3 size, float angle, Vector3 direction, float depth)
        {
            Radius = 0f;

            Flag = false;
            Rotation = Quaternion.identity;

            Center = center; Size = size; Angle = angle; Direction = direction; Depth = depth;
        }

         // ------------------------------------------------------------
        /// <summary>
        /// BoxCollider 전용 생성자
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection(Vector3 center, Vector3 size, Vector3 direction, float depth)
        {
            Radius = 0f;

            Angle = 0f;
            
            Flag = false;
            Rotation = Quaternion.identity;

            Center = center; Size = size; Direction = direction; Depth = depth;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// SphereCollider 또는 CircleCollider2D 전용 생성자
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection(Vector3 center, float radius, Vector3 direction, float depth)
        {
            Size = Vector3.zero;
            Angle = 0f;

            Flag = false;
            Rotation = Quaternion.identity;

            Center = center; Radius = radius; Direction = direction; Depth = depth;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// CapsuleCollider2D(수직) 전용 생성자
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection(Vector3 center, float radius, Vector3 direction, float depth, bool flag) :
        this(center, radius, direction, depth)
        {
            Flag = flag;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// CapsuleCollider2D(수평) 전용 생성자
        /// </summary>
        // ------------------------------------------------------------
        public GroundCheckerDetection(Vector3 center, Vector3 size, float angle, Vector3 direction, float depth, bool flag) :
        this(center, size, angle, direction, depth)
        {
            Flag = flag;
        }
    }
}