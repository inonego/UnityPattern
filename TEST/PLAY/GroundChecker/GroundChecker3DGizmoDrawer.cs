using UnityEngine;
using UnityEditor;

using inonego;

// ============================================================================
/// <summary>
/// GroundChecker3D의 기즈모를 그리는 컴포넌트입니다.
/// </summary>
// ============================================================================
public class GroundChecker3DGizmoDrawer : MonoBehaviour
{

#region 생성자

    private GroundChecker3D groundChecker;
    
    public void Initialize(GroundChecker3D checker)
    {
        groundChecker = checker;
    }

#endregion
    
#region 그리기 업데이트
    
    private void OnDrawGizmos()
    {
        if (groundChecker == null) return;
        
        // IsOnGround 상태에 따라 색상 결정
        var color = groundChecker.IsOnGround ? Color.green : Color.red;
        
        Gizmos.color = color;
        
        // GroundChecker3D의 각 Collider에 대해 기즈모 그리기
        var colliders = groundChecker.GameObject.GetComponents<Collider>();
        
        foreach (var collider in colliders)
        {
            if (collider is BoxCollider boxCollider)
            {
                DrawBoxColliderGizmo(boxCollider);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                DrawSphereColliderGizmo(sphereCollider);
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                DrawCapsuleColliderGizmo(capsuleCollider);
            }
        }
    }
    
    private void DrawBoxColliderGizmo(BoxCollider boxCollider)
    {
        // GroundChecker3D의 계산 로직 사용
        var info = groundChecker.GetBoxColliderDetectionInfo(boxCollider, Time.fixedDeltaTime);
        
        var depthSize = new Vector3(info.Size.x, info.Depth, info.Size.z);
        var position = info.Center + info.Direction * info.Depth * 0.5f;
        var color = groundChecker.IsOnGround ? Color.green : Color.red;

        var rotation = boxCollider.transform.rotation;

        GizmoHelper.DrawWireCube(position, rotation, depthSize, color);
        
        // 바닥 방향을 노란색으로 그리기
        var (start, end) = (info.Center, info.Center + info.Direction * info.Depth);
        GizmoHelper.DrawLine(start, end, Color.yellow);
    }
    
    private void DrawSphereColliderGizmo(SphereCollider sphereCollider)
    {
        // GroundChecker3D의 계산 로직 사용
        var info = groundChecker.GetSphereColliderDetectionInfo(sphereCollider, Time.fixedDeltaTime);
        
        var rotation = Quaternion.LookRotation(Vector3.forward, info.Direction);
        var color = groundChecker.IsOnGround ? Color.green : Color.red;
        
        var height = info.Depth + 2 * info.Radius;
        var center = info.Center + info.Direction * info.Depth * 0.5f;
        GizmoHelper.DrawWireCapsule(center, rotation, info.Radius, height, color);
        
        // 바닥 방향을 노란색으로 그리기
        var (start, end) = (info.Center, info.Center + info.Direction * info.Depth);
        GizmoHelper.DrawLine(start, end, Color.yellow);
    }
    
    private void DrawCapsuleColliderGizmo(CapsuleCollider capsuleCollider)
    {
        // GroundChecker3D의 계산 로직 사용
        var info = groundChecker.GetCapsuleColliderDetectionInfo(capsuleCollider, Time.fixedDeltaTime);
        
        if (info.Flag)
        {
            var rotation = capsuleCollider.transform.rotation;
            var color = groundChecker.IsOnGround ? Color.green : Color.red;

            var height = info.Depth + 2 * info.Radius;
            var center = info.Center + info.Direction * info.Depth * 0.5f;
            GizmoHelper.DrawWireCapsule(center, rotation, info.Radius, height, color);
        }
        else
        {
            // NONE
        }
        
        // 바닥 방향을 노란색으로 그리기
        var (start, end) = (info.Center, info.Center + info.Direction * info.Depth);
        GizmoHelper.DrawLine(start, end, Color.yellow);
    }

#endregion

}
