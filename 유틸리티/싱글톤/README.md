# 싱글톤 (Singleton)

- 프로젝트 전반에서 공통으로 사용할 수 있는 **싱글톤 패턴 기반 베이스 클래스**들을 제공합니다.
- 스레드 세이프(Thread-Safe)한 지연 초기화와 중복 인스턴스 방지 처리가 포함되어 있습니다.

---

### 🏗️ 주요 클래스 (Key Classes)

- **[Singleton<T>]**: 일반 C# 클래스(참조 타입)를 싱글톤으로 만들 때 사용합니다. Unity 엔진과 무관한 로직에 적합합니다.
- **[MonoSingleton<T>]**: `MonoBehaviour` 기반의 씬 상의 싱글톤입니다. 인스턴스가 없을 경우 새 `GameObject`를 자동 생성하여 컴포넌트를 추가합니다.
- **[PersistentMonoSingleton<T>]**: `MonoSingleton<T>`를 상속받으며, `DontDestroyOnLoad`가 자동으로 적용되어 씬 전환 시에도 파괴되지 않습니다.

---

### ⚙️ 공통 동작 및 특징 (Common Behaviors)

- **`Instance` 프로퍼티**: `static T Instance`를 통해 접근하며, `lock`을 사용한 지연 초기화를 수행합니다.
- **`ClearInstance()`**: 정적 인스턴스 참조를 강제로 초기화해야 할 때(테스트, 도메인 리로드 등) 사용합니다.
- **중복 방지**: `MonoSingleton` 계열은 `Awake()` 시점에 이미 인스턴스가 존재하면 자신을 파괴하여 유일성을 보장합니다.

---

### 🔄 클래스별 차이점 요약 (Comparison)

| 클래스 | 대상 | Unity 의존성 | 씬 전환 시 | 인스턴스 생성 방식 |
|:---|:---|:---:|:---:|:---|
| `Singleton<T>` | 순수 C# 클래스 | X | 유지 | `new T()` |
| `MonoSingleton<T>` | MonoBehaviour | O | 파괴 | 씬 검색 후 없으면 자동 생성 |
| `PersistentMonoSingleton<T>` | MonoBehaviour | O | **유지** | 씬 검색 후 없으면 자동 생성 |

---

### 📝 사용 예시 (Examples)

#### 1. 일반 클래스 싱글톤 (설정 매니저)
```csharp
public class ConfigManager : Singleton<ConfigManager> {
    public float MasterVolume = 1.0f;
}
// 사용: ConfigManager.Instance.MasterVolume = 0.5f;
```

#### 2. 씬 기반 싱글톤 (UI 매니저)
```csharp
public class SceneUIManager : MonoSingleton<SceneUIManager> {
    public void ShowGameOver() { /* UI 표시 */ }
}
```

#### 3. 영속 싱글톤 (오디오 매니저)
```csharp
public class AudioManager : PersistentMonoSingleton<AudioManager> {
    public void PlayBgm(AudioClip clip) { /* 배경음 재생 */ }
}
```

---

### ⚠️ 주의사항 (Cautions)

- **전역 상태**: 싱글톤은 전역 상태를 형성하므로 결합도가 높아질 수 있습니다. 명확한 책임을 가진 매니저 단위에만 사용하십시오.
- **의존성**: 복잡한 시스템에서는 DI(Dependency Injection)나 이벤트 시스템과 함께 사용하는 것을 권장합니다.
