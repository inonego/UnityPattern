## 빌보드

### 개요

`빌보드`는 오브젝트가 **카메라(또는 임의의 타겟)를 향하도록 회전**시키는 간단한 비주얼 유틸리티입니다.

- 전역 설정을 담당하는 `BillboardManager`와
- 각 오브젝트에 붙는 `Billboard` 컴포넌트로 구성됩니다.

빌보드는 다음과 같은 상황에서 유용합니다.

- 2D 스프라이트/월드 스페이스 UI가 항상 카메라를 보도록 할 때
- HP 바, 이름표 등 3D 공간에 배치된 UI를 카메라 방향으로 정렬할 때

---

### 주요 타입

- **`BillboardLookMode`**
  - 빌보드가 타겟을 바라보는 방식을 결정하는 열거형입니다.
  - 값:
    - `Parallel` : 타겟이 바라보는 방향과 평행이 되도록 회전 (카메라와 평행)
    - `HardLook` : 타겟의 위치를 직접 바라보도록 회전 (정확히 타겟 쪽을 향함)

- **`BillboardManager`**
  - `MonoSingleton<BillboardManager>`를 상속한 싱글톤 매니저입니다.
   - 모든 `Billboard`가 공통으로 사용하는 **기본 설정 값**을 전역으로 제공합니다.
   - 예: 기본 시선 타겟(`LookTarget`), 기본 모드(`Mode`), Up 벡터(`Up`), 축 잠금(`LockAxis`) 등

- **`Billboard`**
  - 개별 오브젝트에 부착되는 컴포넌트입니다.
   - `BillboardManager`가 제공하는 **공통 설정을 기본으로 사용**하면서,
     각 인스턴스마다 `Override*` 필드를 통해 일부 속성만 개별적으로 오버라이드할 수 있습니다.

---

### 동작 방식

1. `Billboard.LateUpdate`에서
   - `Override*` 값이 설정되어 있으면 그것을 사용하고,
   - 없으면 `BillboardManager.Instance`의 전역 값을 사용합니다.
2. `BillboardLookMode`에 따라 목표 회전을 계산합니다.
   - **Parallel**
     - `Quaternion.LookRotation(lookTarget.forward, up)`  
       → 타겟과 같은 `forward` 방향을 향하도록 회전
   - **HardLook**
     - `lookTarget.position - transform.position` 방향을 정규화하여  
       `Quaternion.LookRotation(delta, up)`으로 회전
3. `LockAxis` 설정에 따라
   - 기존 회전과 목표 회전을 Euler 각도 단위로 혼합하여
   - 각 축별로 잠글지(`true`)/따라갈지(`false`)를 결정합니다.