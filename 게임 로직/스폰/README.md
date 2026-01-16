# 스폰 레지스트리 (Spawn Registry)

- 게임 객체의 생성(Spawn)과 제거(Despawn)를 체계적으로 관리하는 레지스트리 시스템입니다.
- 키(Key) 기반으로 스폰된 객체를 추적하고, 다양한 이벤트 및 생명주기 후크를 제공하여 유연한 확장이 가능합니다.

---

### 🏗️ 핵심 클래스 (Core Classes)

- **[SpawnRegistryBase<TKey, T>]**: 모든 스폰 레지스트리의 최상위 추상 클래스입니다.
- **[SpawnRegistry<TKey, T>]**: 매개변수 없이 객체를 생성하고 스폰하는 기본 레지스트리입니다.
- **[SpawnRegistry<TKey, T, TParam>]**: 초기화 매개변수(`TParam`)를 받아 객체를 생성하고 초기화하는 레지스트리입니다.

---

### ⚙️ 주요 기능 (Key Features)

- **`Spawn()` / `Spawn(TParam)`**: 객체를 생성하고 레지스트리에 등록합니다. 내부적으로 `Acquire()`를 호출합니다.
- **`Despawn()`**: 확장 메서드를 통해 `entity.Despawn()` 형태로 객체를 안전하게 제거합니다.
- **`DespawnAll()`**: 레지스트리에 등록된 모든 객체를 일괄 디스폰합니다.
- **`Find(key)` / `Find(IKeyable)`**: 키 또는 `IKeyable` 인터페이스를 통해 활성화된 객체를 조회합니다.

---

### 🔗 이벤트 및 후크 (Events & Hooks)

#### 1. 레지스트리 이벤트 (Registry Events)
- **`OnSpawn`**: 객체가 스폰된 **후** 호출되는 이벤트입니다.
- **`OnDespawn`**: 객체가 디스폰된 **후** 호출되는 이벤트입니다.

#### 2. 레지스트리 후크 (Registry Hooks)
- `OnBeforeSpawn(T)`, `OnAfterSpawn(T)`
- `OnBeforeDespawn(T)`, `OnAfterDespawn(T)`
- `OnInit(T, TParam)` (매개변수 버전 전용)

#### 3. 객체 자체 후크 (Object Hooks)
- `spawnable.OnBeforeSpawn()`, `spawnable.OnAfterSpawn()`
- `spawnable.OnBeforeDespawn()`, `spawnable.OnAfterDespawn()`
- `spawnable.Init(TParam)` (매개변수 버전 전용)

---

### 🔄 메서드 호출 순서 (Call Order)

#### 스폰 시 순서 (Spawn Sequence)
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
| 9 | 딕셔너리 등록 | 스폰 목록(`spawned`) 추가 |
| 10 | `OnAfterSpawn(spawnable)` | 레지스트리 후처리 |
| 11 | `spawnable.OnAfterSpawn()` | 객체 후처리 |
| 12 | `OnSpawn` 이벤트 | 스폰 완료 알림 |

#### 디스폰 시 순서 (Despawn Sequence)
| 순서 | 메서드 | 설명 |
|:---:|:---|:---|
| 1 | 유효성 검증 | null, IsSpawned, HasKey 확인 |
| 2 | `OnDespawn` 이벤트 | 디스폰 시작 알림 |
| 3 | `despawnable.OnBeforeDespawn()` | 객체 전처리 |
| 4 | `OnBeforeDespawn(despawnable)` | 레지스트리 전처리 |
| 5 | 딕셔너리 해제 | 스폰 목록 제거 **(조건부)** |
| 6 | 상태 해제 | `IsSpawned = false`, `DespawnFromRegistry = null` |
| 7 | `despawnable.OnAfterDespawn()` | 객체 후처리 |
| 8 | `OnAfterDespawn(despawnable)` | 레지스트리 후처리 |

---

### 📝 사용 예시 (Example)

```csharp
public class MySpawnRegistry : SpawnRegistry<int, MyEntity>
{
    protected override MyEntity Acquire() => new MyEntity();
    protected override void OnBeforeSpawn(MyEntity spawnable) { /* 전처리 */ }
    protected override void OnAfterDespawn(MyEntity despawnable) { /* 정리 */ }
}

// 사용
var registry = new MySpawnRegistry();
var entity = registry.Spawn();
entity.Despawn(); // 확장 메서드 사용
```

---

### ⚠️ 주의사항 (Cautions)

- **TParam 예외**: `Spawn` 과정(3~6번)에서 예외 발생 시 `DespawnInternal()`이 자동 호출되어 정리됩니다.
- **딕셔너리 제거**: `DespawnAll()` 사용 시 성능을 위해 개별 딕셔너리 제거를 생략하고 마지막에 일괄 처리합니다.
