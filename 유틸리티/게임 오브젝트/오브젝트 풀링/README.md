## 오브젝트 풀링

### 개요

`오브젝트 풀링`은 자주 생성·삭제되는 객체를 미리 만들어 두고 재사용하여,  
**GC 할당과 Instantiate/Destroy 비용을 줄이는** 시스템입니다.

- 일반 C# 클래스, `GameObject`, `Component` 모두에 대해 공통 패턴으로 풀링 지원
- 동기/비동기 방식 모두 지원 (`Awaitable<T>` 기반)
- 현재 사용 중인 객체와 풀에 반환된 객체를 각각 추적
- `게임 오브젝트 프로바이더`와 연동하여 다양한 생성 전략 사용 가능

> ⚠ **주의**  
> 오브젝트 풀링 관련 클래스(`PoolBase<T>`, `Pool<T>`, `GOPool`, `GOCompPool<T>` 등)는  
> **직렬화를 통한 세이브/로드를 지원하지 않습니다.**  
> 풀의 상태(Released/Acquired 목록)는 런타임 동안에만 유효한 정보로,  
> 저장 데이터로 보존하거나 복원하는 용도로 사용하면 안 됩니다.

---

### 주요 클래스

- **`PoolBase<T>`**
  - 모든 풀 구현의 공통 베이스 추상 클래스
  - 내부에서 **사용 중(acquired)**, **반환됨(released)** 컬렉션을 관리
  - 실제 객체 생성 부분(`AcquireNew`)만 하위 클래스에서 구현

- **`Pool<T>`**
  - `new()` 제약을 가진 **일반 참조 타입(class)** 전용 풀
  - `new T()`를 통해 객체를 생성하는 가장 단순한 풀 구현

- **`GOPool`**
  - `GameObject` 전용 풀
  - `IGameObjectProvider`를 사용해 게임 오브젝트를 생성
  - `Parent` / `Pool` / `WorldPositionStays` 설정을 통해
    - 사용 중인 오브젝트의 부모
    - 반환된 오브젝트를 쌓아둘 풀 부모
    - 부모 변경 시 위치 유지 여부
    를 제어

- **`GOCompPool<T>`**
  - `Component` 전용 풀 (`where T : Component`)
  - 내부적으로 `GOPool`을 사용하여 `GameObject`를 풀링하면서,
    해당 오브젝트에서 `T` 컴포넌트를 찾아 함께 관리
  - 컴포넌트 기준으로 `AcquireComp`, `ReleaseComp` 사용

---

### 공통 동작 및 컬렉션

모든 풀은 `PoolBase<T>`를 통해 다음 컬렉션을 공통으로 관리합니다.

- **`Released`**
  - 현재 **풀에 반환되어, 다시 사용할 수 있는 객체 목록**
  - 내부적으로는 `Queue<T>`로 구현되어 FIFO 방식으로 재사용

- **`Acquired`**
  - 현재 **사용 중인 객체 목록**
  - 내부적으로는 `HashSet<T>`로 구현되어, 중복 방지 및 빠른 포함 여부 체크 가능

`GOCompPool<T>`는 여기에 더해 다음 컬렉션을 관리합니다.

- **`ReleasedComp` / `AcquiredComp`**
  - `GameObject` ↔ `Component` 매핑용 `Dictionary<GameObject, T>` 기반 컬렉션
  - IPool 인터페이스(`IPool<T>.Released`, `IPool<T>.Acquired`)는 이 컴포넌트 기준 컬렉션을 노출

---

### 주요 기능

#### Acquire 계열

- **`Acquire()`**
  - 풀에서 **사용 가능한 객체를 하나 꺼내서 반환**합니다.
  - 동작 순서:
    1. `released` 큐에 남아있는 객체가 있으면 `Dequeue`로 꺼냄
    2. 없으면 `AcquireNew()`를 호출하여 새로 생성
    3. `acquired` 집합에 추가
    4. `OnAcquire(item)` 훅 호출

- **`AcquireAsync()`**
  - 비동기 방식으로 객체를 획득
  - `GOPool`, `GOCompPool<T>`에서 사용
  - 내부적으로 `AcquireInternalAsync`와 `AcquireNewAsync`를 사용하여
    Addressables 등 비동기 생성에도 대응

#### Release 계열

- **`Release(T item)`**
  - 사용 중인 객체를 **풀로 반환**합니다.
  - `acquired` 집합에 존재하지 않는 객체를 반환하려고 하면 예외가 발생
  - 정상 반환 시:
    - `acquired`에서 제거
    - `released` 큐에 Enqueue
    - `OnRelease(item)` 훅 호출

- **`ReleaseAll()`**
  - 현재 **사용 중인 모든 객체를 순회하면서 풀로 반환**합니다.
  - 각각에 대해 `ReleaseInternal`을 호출한 뒤, 최종적으로 `acquired.Clear()`로 정리

---

### GOPool 전용 동작

`GOPool`은 `GameObject` 전용 풀로, 다음과 같은 특징을 가집니다.

- **`GameObjectProvider`**
  - `IGameObjectProvider`를 통해 실제 `GameObject`를 생성
  - 프리팹, Addressables 등 다양한 전략을 동일한 인터페이스로 사용 가능

- **위치/부모 관리**
  - `Parent` : 사용 중인 오브젝트의 부모
  - `Pool` : 반환된 오브젝트를 쌓아둘 부모(비활성 오브젝트 보관용)
  - `WorldPositionStays` : `SetParent` 호출 시 월드 좌표 유지 여부

- **활성/비활성 처리**
  - `OnAcquire(GameObject)`
    - `Parent`로 부모 설정 (`SetParent(Parent, WorldPositionStays)`)
    - `SetActive(true)`로 활성화
  - `OnRelease(GameObject)`
    - `Pool`로 부모 설정
    - `SetActive(false)`로 비활성화

---

### GOCompPool<T> 전용 동작

`GOCompPool<T>`는 `GOPool`을 상속하여 **컴포넌트 기준 풀링**을 제공합니다.

- **GameObject ↔ Component 매핑**
  - `OnAcquire` / `OnRelease`를 오버라이드하여
    - `GameObject`에 붙어 있는 `T` 컴포넌트를 찾아
    - `releasedComp`, `acquiredComp` 딕셔너리에 매핑
  - 덕분에 IPool 인터페이스를 사용할 때 **컴포넌트 타입으로 자연스럽게 풀링** 가능

- **컴포넌트 전용 메서드**
  - `AcquireComp()`
    - 내부적으로 `Acquire()`로 `GameObject`를 가져온 후
    - 해당 오브젝트에서 미리 매핑된 `T`를 찾아 반환
  - `AcquireCompAsync()`
    - `AcquireAsync()`와 동일한 패턴의 비동기 버전
  - `ReleaseComp(T comp)`
    - 넘겨 받은 컴포넌트의 `gameObject`를 풀에 반환

---

### 사용 예시 (개념)

> 실제 프로젝트에서는 적절한 프리팹/프로바이더를 세팅한 뒤 사용합니다.

- **일반 클래스 풀**
  - `new()` 제약이 있는 일반 클래스의 생성 비용을 줄이고 싶을 때
  - 예: 경량 데이터 객체, 임시 계산용 버퍼 등

```csharp
// 풀에 넣을 일반 클래스
public class BulletData
{
    public int Damage;
    public float Speed;
}

// 어디서든 풀 생성 후 사용
private Pool<BulletData> bulletDataPool = new();

void UseBulletData()
{
    // 1. Acquire
    var data = bulletDataPool.Acquire();
    data.Damage = 10;
    data.Speed = 5.0f;

    // ... 사용 ...

    // 2. Release
    bulletDataPool.Release(data);
}
```

- **GameObject 풀 (`GOPool`)**
  - 총알, 이펙트, 적 스폰 등 **자주 생성/삭제되는 게임 오브젝트**에 적합
  - `게임 오브젝트 프로바이더`와 함께 사용하면
    - 프리팹 기반
    - Addressables 기반
    등의 생성 방식을 동일한 풀 인터페이스로 다룰 수 있음

```csharp
public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private GOPool bulletPool = null; // 인스펙터에서 PrefabGameObjectProvider 설정

    public void SpawnBullet(Vector3 position)
    {
        var go = bulletPool.Acquire();
        go.transform.position = position;

        // 일정 시간 뒤 자동 반환 예시
        StartCoroutine(DespawnAfterSeconds(go, 2.0f));
    }

    private IEnumerator DespawnAfterSeconds(GameObject go, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        bulletPool.Release(go);
    }
}
```

- **Component 풀 (`GOCompPool<T>`)**
  - 특정 컴포넌트 타입을 직접 다루고 싶을 때
  - 예: `EnemyController`, `Bullet`, `HPBar`와 같은 컴포넌트를 바로 획득/반환

```csharp
public class Enemy : MonoBehaviour
{
    public void Init(Vector3 position)
    {
        transform.position = position;
    }
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GOCompPool<Enemy> enemyPool = null; // Enemy가 최상단에 붙은 프리팹 필요

    public void SpawnEnemy(Vector3 position)
    {
        var enemy = enemyPool.AcquireComp();
        enemy.Init(position);
    }

    public void DespawnEnemy(Enemy enemy)
    {
        enemyPool.ReleaseComp(enemy);
    }
}
```

---

### 정리

- `PoolBase<T>`를 중심으로, **일반 클래스 / GameObject / Component**를 모두 같은 패턴으로 풀링할 수 있습니다.
- `GOPool`, `GOCompPool<T>`는 `게임 오브젝트 프로바이더` 시스템과 연동되어,  
  **생성 전략(프리팹, Addressables 등)을 쉽게 교체하면서도 동일한 풀 인터페이스를 유지**할 수 있도록 설계되어 있습니다.


