## 싱글톤

### 개요

`싱글톤` 폴더는 프로젝트 전반에서 공통으로 사용할 수 있는 **싱글톤 패턴 기반 베이스 클래스**들을 제공합니다.

- **일반 C# 클래스용 싱글톤** (`Singleton<T>`)
- **`MonoBehaviour` 기반 싱글톤** (`MonoSingleton<T>`)
- **씬 전환에도 파괴되지 않는 영속 싱글톤** (`PersistentMonoSingleton<T>`)
- 스레드 세이프한 인스턴스 생성(`lock`) 및 중복 인스턴스 방지 처리 포함

---

### 주요 클래스

- **`Singleton<T>`**
  - 아무 클래스나(참조 타입) 싱글톤으로 만들고 싶을 때 사용하는 **일반 C# 클래스용 베이스**입니다.
  - `where T : Singleton<T>, new()` 제약을 통해 **자기 자신을 제네릭 인자로 가지는** 패턴을 강제합니다.
  - `new T()`로 인스턴스를 생성하므로, **매개변수가 없는 기본 생성자**가 필요합니다.

- **`MonoSingleton<T>`**
  - `MonoBehaviour`를 상속받는 컴포넌트를 **씬 상의 싱글톤**으로 만들고 싶을 때 사용하는 베이스 클래스입니다.
  - `Instance` 접근 시
    1. 씬 안에서 해당 타입의 인스턴스를 찾고 (`FindAnyObjectByType<T>()`)
    2. 없다면 새 `GameObject`를 만들고 `AddComponent<T>()`로 붙여 생성합니다.
  - `Awake()`에서 이미 다른 인스턴스가 존재하면 **경고를 출력하고 자기 자신을 파괴**하여, 씬에 항상 하나만 존재하도록 보장합니다.
  - `isDontDestroyOnLoad` 플래그에 따라 `DontDestroyOnLoad` 적용 여부를 제어할 수 있습니다.

- **`PersistentMonoSingleton<T>`**
  - `MonoSingleton<T>`를 상속한 **영속 싱글톤 전용 베이스 클래스**입니다.
  - `isDontDestroyOnLoad`를 `true`로 오버라이드하여, 인스턴스가 생성될 때 자동으로 `DontDestroyOnLoad`가 적용됩니다.
  - 씬 전환이 자주 발생하는 시스템(예: 게임 매니저, 오디오 매니저 등)에 적합합니다.

---

### 공통 동작

- **`Instance` 프로퍼티**
  - 세 클래스 모두 `static T Instance` 프로퍼티를 통해 싱글톤 인스턴스에 접근합니다.
  - 내부적으로 `lock(typeof(T))`를 사용하여 **스레드 세이프한 지연 초기화(lazy initialization)**를 수행합니다.
  - 인스턴스가 아직 없을 경우에만 생성 로직이 수행됩니다.

- **`ClearInstance()`**
  - `Singleton<T>` / `MonoSingleton<T>` 모두 `ClearInstance()` 메서드를 제공합니다.
  - 정적 인스턴스 참조를 강제로 초기화해야 할 때(예: 테스트 환경, 도메인 리로드 등) 사용할 수 있습니다.

---

### 각 클래스의 특징과 차이점

- **`Singleton<T>` (일반 클래스 싱글톤)**
  - **Unity 엔진과 무관한 순수 C# 클래스**를 싱글톤으로 사용할 때 적합합니다.
  - `GameObject`나 `MonoBehaviour` 컨텍스트가 필요 없는 매니저, 설정 객체 등에 사용할 수 있습니다.

- **`MonoSingleton<T>` (씬 기반 MonoBehaviour 싱글톤)**
  - 씬 위에 존재하는 게임 오브젝트/컴포넌트를 싱글톤으로 관리합니다.
  - 인스턴스 생성 방식:
    - 씬에 이미 존재하면 그 인스턴스를 사용
    - 없으면 새 `GameObject`를 생성 후 `T` 컴포넌트를 추가하여 사용
  - `isDontDestroyOnLoad` 기본값은 `false`이므로, 기본적으로 **씬 전환 시 파괴**됩니다.

- **`PersistentMonoSingleton<T>` (DontDestroyOnLoad 싱글톤)**
  - `MonoSingleton<T>`의 동작은 그대로 유지하면서,  
    `isDontDestroyOnLoad`를 항상 `true`로 설정하여 **씬이 바뀌어도 인스턴스를 유지**합니다.
  - 예: `AudioManager`, `GameManager`, `InputManager` 등 전역 시스템에 적합

---

### 사용 예시 (개념)

- **일반 클래스 싱글톤**
  - 전역 설정, 데이터 캐시, 순수 로직 매니저 등 **엔진에 의존하지 않는 로직**에 사용
  - 예: `ConfigManager : Singleton<ConfigManager>`

```csharp
public class ConfigManager : Singleton<ConfigManager>
{
    public float MasterVolume { get; set; } = 1.0f;
}

// 사용 예시
void SomeMethod()
{
    ConfigManager.Instance.MasterVolume = 0.5f;
    var volume = ConfigManager.Instance.MasterVolume;
}
```

- **MonoBehaviour 싱글톤**
  - 씬 안에서만 유효한 매니저, UI 컨트롤러 등
  - 예: `SceneUIManager : MonoSingleton<SceneUIManager>`

```csharp
public class SceneUIManager : MonoSingleton<SceneUIManager>
{
    public void ShowGameOver()
    {
        // 게임 오버 UI 표시
    }
}

// 아무 곳에서나
void OnPlayerDead()
{
    SceneUIManager.Instance.ShowGameOver();
}
```

- **Persistent MonoBehaviour 싱글톤**
  - 게임 전체 동안 유지되어야 하는 전역 매니저
  - 예: `AudioManager : PersistentMonoSingleton<AudioManager>`

```csharp
public class AudioManager : PersistentMonoSingleton<AudioManager>
{
    [SerializeField]
    private AudioSource bgmSource = null;

    public void PlayBgm(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}

// 씬 어디에서든
void PlayStageBgm(AudioClip clip)
{
    AudioManager.Instance.PlayBgm(clip);
}
```

---

### 주의사항

- 싱글톤은 **전역 상태(Global State)**를 만들기 때문에 남용 시 테스트/유지보수가 어려워질 수 있습니다.
- 가능한 한 **명확한 책임을 가진 매니저/서비스 단위**에만 사용하고,  
  의존성이 복잡해지지 않도록 DI, 이벤트, 메시지 시스템과 함께 사용하면 좋습니다.


