# 빌보드 (Billboard)

- 오브젝트가 항상 카메라 또는 임의의 타겟을 향하도록 회전시키는 비주얼 유틸리티입니다.
- 2D 스프라이트, HP 바, 이름표 등 월드 스페이스 UI를 카메라 방향으로 정렬할 때 사용합니다.

---

### 🏗️ 주요 타입 (Key Types)

- **`BillboardLookMode`**: 타겟을 바라보는 방식을 결정합니다.
  - `Parallel`: 타겟(카메라)이 바라보는 방향과 평행하게 회전합니다.
  - `HardLook`: 타겟의 정확한 위치를 직접 바라보도록 회전합니다.
- **[BillboardManager]**: 전역 설정을 관리하는 싱글톤입니다. 모든 빌보드가 사용할 기본 타겟, 모드, Up 벡터 등을 제공합니다.
- **[Billboard]**: 개별 오브젝트에 부착되는 컴포넌트입니다. 매니저의 공통 설정을 따르거나 개별 속성을 오버라이드하여 동작합니다.

---

### 🔄 동작 방식 (Workflow)

1. **설정 결정**: `LateUpdate` 시점에 각 빌보드는 `Override` 필드가 있으면 해당 값을 사용하고, 없으면 `BillboardManager`의 전역 값을 참조합니다.
2. **회전 계산**:
   - **Parallel**: `Quaternion.LookRotation(lookTarget.forward, up)`
   - **HardLook**: 타겟과 내 위치 사이의 벡터를 계산하여 `LookRotation` 적용
3. **축 잠금 (LockAxis)**: 계산된 목표 회전을 Euler 각도로 변환한 뒤, `LockAxis` 설정에 따라 특정 축의 회전을 잠그고 기존 회전을 유지합니다.

---

### 📝 사용 예시 (Example)

```csharp
// 전역 설정 (메인 카메라 타겟)
BillboardManager.Instance.LookTarget = Camera.main.transform;
BillboardManager.Instance.Mode = BillboardLookMode.HardLook;

// 특정 오브젝트에서 Y축만 회전하도록 설정 (X, Z 잠금)
var billboard = GetComponent<Billboard>();
billboard.OverrideLockAxis = new Vector3Bool(true, false, true);
```

---

### ⚠️ 주의사항 (Cautions)

- **LateUpdate**: 회전 떨림을 방지하기 위해 모든 연산은 `LateUpdate`에서 수행됩니다.
- **매니저 의존성**: 싱글톤인 `BillboardManager`가 씬에 존재하거나 인스턴스화되어 있어야 정상 동작합니다.
