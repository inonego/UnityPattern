## 오브젝트 풀링

### 개요

`오브젝트 풀링`은 자주 생성·삭제되는 객체를 미리 만들어 두고 재사용하여,  
**GC 할당과 Instantiate/Destroy 비용을 줄이는** 시스템입니다.

- 일반 C# 클래스 및 Unity 컴포넌트(`Component`) 모두에 대해 공통 패턴으로 풀링 지원
- 동기/비동기 방식 모두 지원 (`Awaitable<T>` 기반)
- 현재 사용 중인 객체와 풀에 반환된 객체를 각각 추적
- `게임 오브젝트 프로바이더`와 연동하여 다양한 생성 전략 사용 가능

> ⚠ **주의**  
> 오브젝트 풀링 관련 클래스(`PoolBase<T>`, `Pool<T>`, `GOCompPool<T>` 등)는  
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

- **`GOCompPool<T>`**
  - Unity 전용 **컴포넌트 및 GameObject 풀** (`where T : Component`)
  - `IGameObjectProvider`를 사용하여 객체를 생성하며, `Parent` / `Pool` 설정을 통해 부모 관리 및 활성/비활성을 자동으로 제어
  - 컴포넌트(`T`)를 직접 풀링하므로 `Acquire()`, `Release()` 시 별도의 `GetComponent` 호출 없이 즉시 사용 가능

---

### 공통 동작 및 컬렉션

모든 풀은 `PoolBase<T>`를 통해 다음 컬렉션을 공통으로 관리합니다.

- **`Released`**
  - 현재 **풀에 반환되어, 다시 사용할 수 있는 객체 목록**
  - 내부적으로는 `Queue<T>`로 구현되어 FIFO 방식으로 재사용

- **`Acquired`**
  - 현재 **사용 중인 객체 목록**
  - 내부적으로는 `HashSet<T>`로 구현되어, 중복 방지 및 빠른 포함 여부 체크 가능

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
  - Unity용 풀인 `GOCompPool<T>`의 경우, 꺼낸 객체를 자동으로 활성화하고 설정된 `Parent` 하위로 이동시킵니다.

- **`AcquireAsync()`**
  - 비동기 방식으로 객체를 획득
  - 내부적으로 `AcquireInternalAsync`와 `AcquireNewAsync`를 사용하여 Addressables 등 비동기 생성에도 대응

#### Release 계열

- **`Release(T item, bool pushToReleased = true)`**
  - 사용 중인 객체를 **풀로 반환**합니다.
  - `pushToReleased`가 `false`인 경우, `Acquired` 목록에서는 제거하지만 `Released` 큐에는 넣지 않습니다. (소유권 이전 시 사용)
  - `acquired` 집합에 존재하지 않는 객체를 반환하려고 하면 예외가 발생

- **`ReleaseAll()`**
  - 현재 **사용 중인 모든 객체를 순회하면서 풀로 반환**합니다.
  - 각각에 대해 `ReleaseInternal`을 호출한 뒤, 최종적으로 `acquired.Clear()`로 정리

#### 풀 직접 제어

- **`PushToReleased(T item)`**
  - 외부에서 생성했거나 소유권이 이전된 객체를 풀의 **대기(Released) 상태로 직접 추가**합니다.
- **`PopFromReleased()`**
  - 풀의 대기 큐에서 객체를 하나 꺼냅니다. 큐가 비어있으면 `AcquireNew()`를 통해 생성합니다.

---

### GOCompPool<T> 관리 기능

Unity용 풀링 클래스인 `GOCompPool<T>`는 다음과 같은 특징을 가집니다.

- **GameObject 생성 및 관리**
  - `IGameObjectProvider`를 통해 실제 `GameObject`를 생성
  - 프리팹, Addressables 등 다양한 전략을 동일한 인터페이스로 사용 가능

- **위치/부모 및 활성 상태 관리**
  - `Parent` : 사용 중인 오브젝트의 부모
  - `Pool` : 반환된 오브젝트를 쌓아둘 부모(비활성 오브젝트 보관용)
  - `WorldPositionStays` : `SetParent` 호출 시 월드 좌표 유지 여부
  - `Acquire` 시 자동 활성화(`SetActive(true)`), `Release` 시 자동 비활성화(`SetActive(false)`)

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
        // Acquire()를 사용하여 컴포넌트 타입으로 즉시 획득
        var enemy = enemyPool.Acquire();
        enemy.Init(position);
    }

    public void DespawnEnemy(Enemy enemy)
    {
        // Release()를 사용하여 반환
        enemyPool.Release(enemy);
    }
}
```

---

### 정리

- `PoolBase<T>`를 중심으로, **일반 클래스 / Component**를 모두 같은 패턴으로 풀링할 수 있습니다.
- `GOCompPool<T>`는 `게임 오브젝트 프로바이더` 시스템과 연동되어,  
  **생성 전략(프리팹, Addressables 등)을 쉽게 교체하면서도 동일한 풀 인터페이스를 유지**할 수 있도록 설계되어 있습니다.
