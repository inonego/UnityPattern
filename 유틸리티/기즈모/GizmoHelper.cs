using UnityEngine;
using UnityEditor;

namespace inonego
{
    public static class GizmoHelper
    {        
        // ------------------------------------------------------------
        /// <summary>
        /// 선을 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawLine(Vector3 p0, Vector3 p1, Color color)
        {
            var originalColor = Handles.color;
            Handles.color = color;

            Handles.DrawLine(p0, p1);

            Handles.color = originalColor;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 큐브를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawWireCube(Vector3 position, Quaternion rotation, Vector3 size, Color color)
        {
            var matrix = Matrix4x4.TRS(position, rotation, size);

            DrawWireCube(matrix, color);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 큐브를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawWireCube(Matrix4x4 matrix, Color color)
        {
            var (originalMatrix, originalColor) = (Handles.matrix, Handles.color);
            (Handles.matrix, Handles.color) = (matrix, color);

            Handles.DrawWireCube(Vector3.zero, Vector3.one);

            (Handles.matrix, Handles.color) = (originalMatrix, originalColor);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 구를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawWireSphere(Vector3 position, Quaternion rotation, Vector3 size, Color color)
        {
            var matrix = Matrix4x4.TRS(position, rotation, size);

            DrawWireSphere(matrix, color);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 구를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawWireSphere(Matrix4x4 matrix, Color color)
        {
            var (originalMatrix, originalColor) = (Handles.matrix, Handles.color);
            (Handles.matrix, Handles.color) = (matrix, color);

            Handles.DrawWireDisc(Vector3.zero, new(1f, 0f, 0f), 1);
            Handles.DrawWireDisc(Vector3.zero, new(0f, 1f, 0f), 1);
            Handles.DrawWireDisc(Vector3.zero, new(0f, 0f, 1f), 1);

            (Handles.matrix, Handles.color) = (originalMatrix, originalColor);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 반구를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawWireHemisphere(Vector3 position, Quaternion rotation, float radius, Color color)
        {
            var size = new Vector3(radius, radius, radius);

            var matrix = Matrix4x4.TRS(position, rotation, size);

            DrawWireHemisphere(matrix, color);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 반구를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawWireHemisphere(Matrix4x4 matrix, Color color)
        {
            var (originalMatrix, originalColor) = (Handles.matrix, Handles.color);
            (Handles.matrix, Handles.color) = (matrix, color);

            Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1f);
            
            Handles.DrawWireArc(Vector3.zero, new(1f, 0f, 0f), new(0f, 0f, 1f), -180f, 1f);
            Handles.DrawWireArc(Vector3.zero, new(0f, 0f, 1f), new(1f, 0f, 0f), +180f, 1f);

            (Handles.matrix, Handles.color) = (originalMatrix, originalColor);
        }
        
        // ------------------------------------------------------------
        /// <summary>
        /// 캡슐을 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawWireCapsule(Vector3 position, Quaternion rotation, float radius, float height, Color color)
        {
            var lHeight = Mathf.Max(0f, height - radius * 2f);
            
            // 상단과 하단 반구의 위치 계산
            var upVector = position + rotation * new Vector3(0f, lHeight * 0.5f, 0f);
            var downVector = position - rotation * new Vector3(0f, lHeight * 0.5f, 0f);
            
            // 상단 반구 (위쪽을 향함)
            DrawWireHemisphere(upVector, rotation, radius, color);
            
            // 하단 반구 (아래쪽을 향함) - 180도 회전
            var downRotation = rotation * Quaternion.Euler(180f, 0f, 0f);
            DrawWireHemisphere(downVector, downRotation, radius, color);
            
            // 원통 부분 연결선 그리기 (원통 높이가 있을 때만)
            if (lHeight > 0f)
            {
                var right = rotation * Vector3.right * radius;
                var forward = rotation * Vector3.forward * radius;
                
                // 4개의 세로선으로 원통 연결
                DrawLine(upVector + right, downVector + right, color);
                DrawLine(upVector - right, downVector - right, color);
                DrawLine(upVector + forward, downVector + forward, color);
                DrawLine(upVector - forward, downVector - forward, color);
            }
        }
    }
}