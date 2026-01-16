# 엔티티 시스템 (Entity System)

- 게임 내에서 스폰/디스폰이 가능하며 팀, 체력 등의 상태를 가지는 모든 동적 개체의 기반 시스템입니다.
- 순수 로직 계층(`Entity`)과 Unity 표현 계층(`MonoEntity`)을 분리하여 코드 재사용성과 유지보수성을 극대화합니다.

---

### 🎨 설계 의도 (Design Intent)

- **로직과 표현의 분리**: `Entity`는 순수 게임 규칙만 담당하고, `MonoEntity`는 Unity의 시각적 표현 및 생명주기를 담당합니다.
- **다중 뷰 지원**: 하나의 `Entity` 인스턴스를 여러 `MonoEntityBehaviour`에서 참조하여 동일한 상태를 공유하는 다양한 기능을 구현할 수 있습니다.
- **레지스트리 연동**: `ISpawnRegistryObject` 인터페이스 구현을 통해 `SpawnRegistry`와 즉시 연동되어 안전하게 관리됩니다.

---

### 🏗️ 핵심 타입 (Core Types)

- **[Entity]**: 순수 C# 기반의 논리 엔티티 베이스입니다.
  - 키 관리(`ulong Key`), 팀 정보(`Team`), 체력(`HP`)을 포함합니다.
  - `ApplyDamage`, `ApplyHeal` 메서드를 제공하며 사망 시 자동으로 디스폰을 요청합니다.
- **[MonoEntity<T>]**: Unity `MonoBehaviour`와 `Entity`를 연결하는 어댑터입니다.
  - `visualGO`를 통해 시각적 표현을 별도로 분리하거나 런타임에 분리(`Detach`)할 수 있습니다.
- **[MonoEntityBehaviour]**: 엔티티에 종속된 부가 기능을 구현하기 위한 베이스입니다. 부모 계층의 엔티티에 자동으로 연결됩니다.

---

### 📝 사용 예시 (Examples)

#### 1. 레지스트리 연동 (Bootstrap)
```csharp
public class PlayerBootstrap : MonoBehaviour {
    [SerializeField] private PlayerMonoEntitySpawnRegistry monoRegistry;
    private PlayerEntitySpawnRegistry entityRegistry = new();

    private void Start() {
        // 엔티티와 모노 레지스트리 연결
        monoRegistry.Connect(entityRegistry);
        
        // 엔티티 스폰 시 모노 엔티티도 자동 생성 및 동기화
        var playerEntity = entityRegistry.Spawn();
    }
}
```

#### 2. 엔티티 종속 기능 (HP Bar)
```csharp
public class HpBar : MonoEntityBehaviour {
    private void Update() {
        // Entity 프로퍼티를 통해 논리 엔티티에 즉시 접근
        var ratio = Entity.HP.Ratio;
        // UI 업데이트 로직...
    }
}
```

---

### ⚙️ 생명주기 및 훅 (Lifecycle Hooks)

- **`OnBeforeSpawn()`**: HP 이벤트 등록 및 초기 상태 설정을 수행합니다.
- **`OnAfterDespawn()`**: HP 이벤트 해제 및 사망 처리를 수행합니다.
- **`DetachVisualGO()`**: `MonoEntity`에서 시각적 오브젝트만 월드에 남기고 분리할 때 사용합니다. (사망 연출 등)
