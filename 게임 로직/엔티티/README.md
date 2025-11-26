## 엔티티

### 개요

`엔티티`는 게임 안에서 **스폰/디스폰될 수 있고, 팀·체력 등의 상태를 가지는 개체**를 표현하기 위한 기반 코드입니다.

- 순수 C# 레벨의 엔티티 (`Entity`)
- Unity `MonoBehaviour`와 연결해주는 어댑터 (`MonoEntity`)
- 엔티티에 종속된 부가 컴포넌트 베이스 (`MonoEntityBehaviour`)

으로 구성됩니다.

---

### 설계 의도

- **게임 로직과 Unity 의존 코드 분리**
  - `Entity`는 스폰 레지스트리, 팀, 체력, 데미지/힐 등 **순수 게임 규칙**만을 담당합니다.
  - `MonoEntity`는 `MonoBehaviour`와 연결하는 **표현/생명주기 어댑터**로, Unity에 종속된 코드를 이쪽에만 모읍니다.

- **여러 표현 방식에서 공통 엔티티 재사용**
  - 하나의 `Entity`를 UI, 이펙트, AI, 입력 처리 등 여러 컴포넌트(`MonoEntityBehaviour`)에서 참조할 수 있습니다.
  - 이렇게 하면 **동일한 게임 상태를 기준으로 서로 다른 뷰/로직을 얹기 쉬워집니다.**

- **SpawnRegistry와의 연동**
  - `Entity`와 `IMonoEntity`는 모두 `ISpawnRegistryObject<ulong>`를 구현하므로,  
    `SpawnRegistry<ulong, Entity>` 또는 `SpawnRegistry<ulong, PlayerEntity>`와 바로 연동할 수 있습니다.
  - 스폰 시에는 `OnBeforeSpawn`/`OnAfterDespawn`이 자동으로 호출되고,  
    `entity.Despawn()` 확장 메서드를 쓰면 레지스트리가 등록해 둔 `DespawnFromRegistry` 콜백을 통해 안전하게 디스폰됩니다.

---

### 주요 타입

- **`Entity`**
  - 스폰 레지스트리와 연동되는, **게임에서 스폰/디스폰되는 개체의 베이스 클래스**입니다.
  - 특징:
    - 스폰 레지스트리 키: `ulong Key`, `HasKey`, `SetKey`, `ClearKey`
    - 스폰 상태: `bool IsSpawned`
    - 팀 정보: `Team Team`
    - 체력: `HP HP`
    - 스폰/디스폰 훅:
      - `OnBeforeSpawn()` : HP 이벤트 등록 및 상태 초기화
      - `OnAfterDespawn()` : HP 이벤트 해제 및 사망 처리
    - HP 이벤트와 연동:
      - `OnHPStateChange`에서 죽으면(`HP.State.Dead`) 자동으로 `Despawn()` 호출
    - 데미지/힐:
      - `ApplyDamage(int damage, Entity source = null)`
      - `ApplyHeal(int amount, Entity source = null)`

- **`IMonoEntity` / `IMonoEntity<TEntity>`**
  - Unity 오브젝트와 `Entity` 인스턴스를 연결하기 위한 인터페이스입니다.
  - `Entity` 프로퍼티를 통해 대응되는 논리 엔티티에 접근할 수 있습니다.

- **`MonoEntity<TEntity>`**
  - `MonoBehaviour`와 `Entity`를 묶는 **어댑터 베이스 클래스**입니다.
  - 특징:
    - `Key`, `HasKey`, `IsSpawned`를 통해 스폰 레지스트리와 동일한 정보를 노출
    - `TEntity Entity` : 실제 논리 엔티티 인스턴스
    - `Init(TEntity entity)` : 외부에서 생성한 엔티티를 주입
    - `OnBeforeSpawn()`, `OnAfterDespawn()` : 스폰 레지스트리 훅
    - `visualGO` 필드:
      - 엔티티의 시각적 표현을 별도 `GameObject`로 분리할 수 있음
      - `DetachVisualGO()`로 월드에서 떼어낼 수 있음

- **`MonoEntityBehaviour`**
  - 엔티티에 **종속된 추가 컴포넌트**를 만들기 위한 베이스 클래스입니다.
  - 특징:
    - 부모 계층에서 `IMonoEntity`를 찾아 자동으로 연결
    - `MonoEntity` / `Entity` 프로퍼티를 통해 논리 엔티티에 접근

---

### 사용 예시 (개념)

- **EntitySpawnRegistry + MonoEntitySpawnRegistry 조합**

```csharp
// 1. 스폰 가능한 엔티티 정의 (Entity는 이미 ISpawnRegistryObject<ulong>를 구현)
public class PlayerEntity : Entity
{
    public void Attack(Entity target)
    {
        target.ApplyDamage(10, this);
    }
}

// 2. PlayerEntity를 관리하는 엔티티 스폰 레지스트리
public class PlayerEntitySpawnRegistry : EntitySpawnRegistry<PlayerEntity>
{
    protected override PlayerEntity Acquire()
    {
        // 풀에서 가져오거나 새로 생성
        return new PlayerEntity();
    }
}

// 3. PlayerEntity에 대응하는 MonoEntity
public class PlayerMonoEntity : MonoEntity<PlayerEntity>
{
    // 필요한 경우 OnBeforeSpawn / OnAfterDespawn 등을 오버라이드
}

// 4. PlayerMonoEntity를 관리하는 모노 엔티티 스폰 레지스트리
public class PlayerMonoEntitySpawnRegistry : MonoEntitySpawnRegistry<PlayerMonoEntity, PlayerEntity>
{
    protected override void OnInit(PlayerMonoEntity spawnable, PlayerEntity entity)
    {
        // MonoEntity와 Entity 사이 추가 초기화가 필요하다면 여기에서 처리
    }
}

// 5. 사용 예시 (예: 게임 시작 시 플레이어 스폰 및 동기화)
public class PlayerBootstrap : MonoBehaviour
{
    [SerializeField]
    private PlayerMonoEntitySpawnRegistry monoRegistry = null;

    private PlayerEntitySpawnRegistry entityRegistry = new PlayerEntitySpawnRegistry();

    private void Start()
    {
        // 1) 엔티티 레지스트리와 모노 레지스트리를 연결
        monoRegistry.Connect(entityRegistry);

        // 2) 엔티티 스폰 → Connect된 모노 레지스트리가 자동으로 MonoEntity도 스폰
        var playerEntity = entityRegistry.Spawn();

        // 이후에는
        // - 순수 로직은 playerEntity (또는 entityRegistry)에서,
        // - 시각/입력/충돌 등은 MonoEntity/MonoEntityBehaviour 쪽에서 처리합니다.
    }
}
```

- **엔티티에 종속된 컴포넌트 (`MonoEntityBehaviour`)**

```csharp
public class HpBar : MonoEntityBehaviour
{
    private void Update()
    {
        // Entity/HP에 직접 접근 가능
        var ratio = (float)Entity.HP.Current / Entity.HP.Max;
        // ratio에 따라 HP 바 스케일/이미지 갱신 등
    }
}
```


