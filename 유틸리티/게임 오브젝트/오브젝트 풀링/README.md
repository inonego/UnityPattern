# 오브젝트 풀링 (Object Pooling)

- 자주 생성·삭제되는 객체를 미리 만들어 두고 재사용하여 **GC 할당과 Instantiate/Destroy 비용을 줄이는** 시스템입니다.
- 일반 C# 클래스 및 Unity 컴포넌트(`Component`) 모두에 대해 공통 패턴으로 풀링을 지원합니다.
- 동기 및 비동기 방식(`Awaitable<T>` 기반)을 모두 지원하여 Addressables 등 다양한 생성 전략에 대응 가능합니다.

---

### 🏗️ 핵심 클래스 (Core Classes)

- **[PoolBase<T>]**: 모든 풀 구현의 공통 베이스 추상 클래스입니다. 내부에서 **사용 중(Acquired)**, **반환됨(Released)** 컬렉션을 관리합니다.
- **[Pool<T>]**: `new()` 제약을 가진 **일반 참조 타입(class)** 전용 풀입니다. `new T()`를 통해 객체를 생성하는 가장 단순한 풀 구현입니다.
- **[GOCompPool<T>]**: Unity 전용 **컴포넌트 및 GameObject 풀**입니다. (`where T : Component`)
  - `IGameObjectProvider`를 사용하여 객체를 생성하며, `Parent` / `Pool` 설정을 통해 부모 관리 및 활성/비활성을 자동으로 제어합니다.
  - 컴포넌트(`T`)를 직접 풀링하므로 획득/반환 시 별도의 `GetComponent` 호출 없이 즉시 사용 가능합니다.

---

### ⚙️ 주요 기능 (Key Features)

#### 1. 객체 획득 (Acquire)
- **`Acquire()`**: 풀에서 사용 가능한 객체를 하나 꺼내서 반환합니다.
  1. `Released` 큐에 객체가 있으면 `Dequeue`로 꺼냄
  2. 없으면 `AcquireNew()`를 호출하여 새로 생성
  3. `Acquired` 집합에 추가 후 `OnAcquire(item)` 훅 호출
- **`AcquireAsync()`**: 비동기 방식으로 객체를 획득합니다. Addressables 등 비동기 생성에 대응합니다.

#### 2. 객체 반환 (Release)
- **`Release(T item, bool pushToReleased = true)`**: 사용 중인 객체를 풀로 반환합니다.
  - `pushToReleased`가 `false`인 경우, `Acquired` 목록에서는 제거하지만 `Released` 큐에는 넣지 않습니다. (소유권 이전 시 사용)
- **`ReleaseAll()`**: 현재 사용 중인 모든 객체를 순회하면서 풀로 반환합니다.

#### 3. 풀 직접 제어
- **`PushToReleased(T item)`**: 외부에서 생성했거나 소유권이 이전된 객체를 풀의 **대기(Released) 상태로 직접 추가**합니다.
- **`PopFromReleased()`**: 풀의 대기 큐에서 객체를 하나 꺼냅니다. 큐가 비어있으면 새로 생성합니다.

---

### 🛠️ Unity 전용 관리 기능 (`GOCompPool<T>`)

- **생성 전략 추상화**: `IGameObjectProvider`를 통해 프리팹, Addressables 등 다양한 방식으로 `GameObject`를 생성합니다.
- **위치 및 상태 관리**:
  - `Parent`: 사용 중인 오브젝트의 부모
  - `Pool`: 반환된 오브젝트를 보관할 부모 (비활성 오브젝트 보관용)
  - `WorldPositionStays`: `SetParent` 호출 시 월드 좌표 유지 여부
  - `Acquire` 시 자동 활성화(`SetActive(true)`), `Release` 시 자동 비활성화(`SetActive(false)`)

---

### 📝 사용 예시 (Examples)

#### 일반 클래스 풀
```csharp
public class BulletData { public int Damage; public float Speed; }

private Pool<BulletData> bulletDataPool = new();

void UseBulletData()
{
    var data = bulletDataPool.Acquire();
    data.Damage = 10;
    // ... 사용 ...
    bulletDataPool.Release(data);
}
```

#### Component 풀 (`GOCompPool<T>`)
```csharp
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GOCompPool<Enemy> enemyPool = null;

    public void SpawnEnemy(Vector3 position)
    {
        var enemy = enemyPool.Acquire();
        enemy.transform.position = position;
    }

    public void DespawnEnemy(Enemy enemy) => enemyPool.Release(enemy);
}
```

---

### ⚠️ 주의사항 (Cautions)

- **직렬화 미지원**: 풀의 상태(Released/Acquired 목록)는 런타임 동안에만 유효합니다. **저장 데이터로 보존하거나 복원하는 용도로 사용하지 마십시오.**
- **중복 반환 방지**: `Acquired` 집합에 존재하지 않는 객체를 반환하려고 하면 예외가 발생합니다.
