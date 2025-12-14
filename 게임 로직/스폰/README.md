# SpawnRegistry

## 개요

`SpawnRegistry`는 게임 객체의 생성(Spawn)과 제거(Despawn)를 체계적으로 관리하는 레지스트리 시스템입니다.

- 키 기반으로 스폰된 객체를 추적하고 관리
- 스폰/디스폰 시 이벤트 및 후크 메서드 제공
- 제네릭 타입을 통한 유연한 확장성

## 주요 클래스

### SpawnRegistryBase<TKey, T>
기본 스폰 레지스트리 추상 클래스

### SpawnRegistry<TKey, T>
매개변수 없이 스폰하는 레지스트리

### SpawnRegistry<TKey, T, TParam>
초기화 매개변수를 받아 스폰하는 레지스트리

## 주요 기능

### Spawn()
- 객체를 생성하고 스폰
- 내부적으로 `Acquire()`를 호출하여 객체 획득

### Spawn(TParam)
- 초기화 매개변수를 받아 객체를 생성하고 스폰
- `SpawnRegistry<TKey, T, TParam>` 버전에서만 사용 가능

### Despawn()
- 확장 메서드로 객체를 디스폰
- `entity.Despawn()` 형태로 사용

### DespawnAll()
- 레지스트리에 등록된 모든 객체를 디스폰
- 예외 발생 시에도 나머지 객체 계속 처리

### Find(key) / Find(IKeyable)
- `SpawnRegistryBase<TKey, T>`에서 제공하는 검색 헬퍼
- `Find(TKey key)`: 키로 바로 객체를 조회, 없으면 `null` 반환
- `Find(IKeyable<TKey> keyable)`: `IKeyable<TKey>` 동일 키를 가진 객체를 조회, 없으면 `null` 반환
- 키 기반 매핑/복제 시 `Spawned.TryGetValue(...)` 직접 사용 대신 `Find(...)` 사용 권장

## 이벤트 및 후크 메서드

### 이벤트 (Event)
- `OnSpawn`: 객체가 스폰된 **후** 호출되는 이벤트
- `OnDespawn`: 객체가 디스폰된 **후** 호출되는 이벤트

### 후크 메서드 (Hook Methods)
- `OnBeforeSpawn(T)`: 레지스트리에서 객체 스폰 **전** 처리
- `OnAfterSpawn(T)`: 레지스트리에서 객체 스폰 **후** 처리
- `OnBeforeDespawn(T)`: 레지스트리에서 객체 디스폰 **전** 처리
- `OnAfterDespawn(T)`: 레지스트리에서 객체 디스폰 **후** 처리
- `OnInit(T, TParam)`: 레지스트리에서 객체 초기화 **전** 처리 (TParam 버전)

### 객체 자체의 메서드
- `spawnable.OnBeforeSpawn()`: 객체가 스폰되기 **전** 호출
- `spawnable.OnAfterSpawn()`: 객체가 스폰된 **후** 호출
- `spawnable.OnBeforeDespawn()`: 객체가 디스폰되기 **전** 호출
- `spawnable.OnAfterDespawn()`: 객체가 디스폰된 **후** 호출
- `spawnable.Init(TParam)`: 객체 초기화 (TParam 버전)

## 사용 예시

```csharp
// 상속하여 구현
public class MySpawnRegistry : SpawnRegistry<int, MyEntity>
{
    protected override MyEntity Acquire()
    {
        // 풀에서 가져오거나 새로 생성
        return new MyEntity();
    }

    protected override void OnBeforeSpawn(MyEntity spawnable)
    {
        // 스폰 전 처리
    }

    protected override void OnAfterDespawn(MyEntity despawnable)
    {
        // 디스폰 후 처리 (풀에 반환 등)
    }
}

// 사용
var registry = new MySpawnRegistry();
registry.OnSpawn += (key, entity) => Debug.Log($"스폰됨: {key}");
registry.OnDespawn += (key, entity) => Debug.Log($"디스폰됨: {key}");

var entity = registry.Spawn();
entity.Despawn(); // SpawnRegistryUtility의 확장 메서드 사용
```

## 메서드 호출 순서

### Spawn 시 호출 순서

| 순서 | 메서드 | 설명 |
|:---:|:---|:---|
| 1 | `Acquire()` | 객체 획득 |
| 2 | 유효성 검증 (1차) | null, IsSpawned 확인 |
| 3 | `OnBeforeSpawn(spawnable)` | 레지스트리 전처리 |
| 4 | `spawnable.OnBeforeSpawn()` | 객체 전처리 |
| 5 | `OnInit(spawnable, param)` | 레지스트리 초기화 **(TParam 버전)** |
| 6 | `spawnable.Init(param)` | 객체 초기화 **(TParam 버전)** |
| 7 | 유효성 검증 (2차) | HasKey, 중복 키 확인 |
| 8 | 상태 설정 | `IsSpawned = true`, `DespawnFromRegistry` 설정 |
| 9 | 딕셔너리 등록 | `spawned.Add()` - 스폰 목록 추가 |
| 10 | `OnAfterSpawn(spawnable)` | 레지스트리 후처리 |
| 11 | `spawnable.OnAfterSpawn()` | 객체 후처리 |
| 12 | `OnSpawn` 이벤트 | 스폰 완료 알림 |

> **참고**: 
> - 5~6번은 `SpawnRegistry<TKey, T, TParam>` 사용 시에만 호출됩니다.
> - 3~6번에서 예외가 발생하면 `DespawnInternal()`이 호출되어 정리 작업이 수행됩니다.

### Despawn 시 호출 순서

| 순서 | 메서드 | 설명 |
|:---:|:---|:---|
| 1 | 유효성 검증 | null, IsSpawned, HasKey, ContainsKey 확인 |
| 2 | `OnDespawn` 이벤트 | 디스폰 시작 알림 |
| 3 | `despawnable.OnBeforeDespawn()` | 객체 전처리 |
| 4 | `OnBeforeDespawn(despawnable)` | 레지스트리 전처리 |
| 5 | 딕셔너리 해제 | `spawned.Remove()` - 스폰 목록 제거 **(조건부)** |
| 6 | 상태 해제 | `IsSpawned = false`, `DespawnFromRegistry = null` |
| 7 | `despawnable.OnAfterDespawn()` | 객체 후처리 |
| 8 | `OnAfterDespawn(despawnable)` | 레지스트리 후처리 |

> **참고**: 
> - 5번 딕셔너리 해제는 `removeFromDictionary`가 `true`일 때만 수행됩니다.
> - `DespawnAll()` 메서드 사용 시 `removeFromDictionary`가 `false`로 설정되어 5번 딕셔너리 해제가 각 객체 디스폰 시점이 아닌 마지막에 일괄 처리됩니다.
