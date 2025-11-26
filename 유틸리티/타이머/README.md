## 타이머

### 개요

`타이머`는 게임 로직에서 자주 사용하는 **카운트다운/지연 처리**를  
일관되게 관리하기 위한 유틸리티입니다.

- 순수 C# 타이머 구현 (`MonoBehaviour`에 의존하지 않음)
- `Update` 메서드를 통해 **원하는 타이밍에서 직접 갱신**
- 일시정지/재개/정지/리셋 등 상태 제어 지원
- 경과 시간, 남은 시간, 0~1 정규화 값(비율) 제공

---

### 주요 타입

- **`TimerState`**
  - 타이머의 상태를 나타내는 열거형입니다.
  - 값:
    - `Ready` : 대기 상태 (시작 전 또는 정지 후)
    - `Run` : 작동 중
    - `Pause` : 일시정지 중

- **`TimerEndEventArgs`**
  - 타이머 종료 시(`OnEnd`)에 전달되는 이벤트 인자 구조체입니다.
  - 현재는 별도 필드를 가지지 않으며, “끝났다”는 시점만을 알리기 위한 역할을 합니다.

- **`IReadOnlyTimer`**
  - 외부에서 **읽기만 가능한 타이머 정보 인터페이스**입니다.
  - 상태/시간/이벤트를 구독만 할 수 있고, 직접 값을 변경할 수 없습니다.

- **`ITimer`**
  - 타이머 제어를 위한 **전체 인터페이스**입니다.
  - 시간 값 설정, 업데이트, 시작/중지/일시정지/재개/리셋 등의 기능을 모두 포함합니다.

- **`Timer`**
  - `ITimer`, `IReadOnlyTimer`를 모두 구현한 **실제 타이머 클래스**입니다.
  - Unity 없이도 사용할 수 있는 순수 C# 타입이나,  
    `Update()`에서 `Time.deltaTime`을 사용하여 Unity와 자연스럽게 연동됩니다.

---

### 인터페이스 요약

#### 공통 상태/이벤트 (`IReadOnlyTimer`, `ITimer`)

- **상태**
  - `bool IsRunning` : 현재 작동 중인지 여부 (`TimerState.Run`)
  - `bool IsPaused` : 현재 일시정지 상태인지 여부 (`TimerState.Pause`)
  - `TimerState Current` : 현재 상태

- **이벤트**
  - `event EventHandler<TimerEndEventArgs> OnEnd`
    - 타이머가 끝났을 때(경과 시간이 Duration에 도달했을 때) 한 번 호출됩니다.
  - `event ValueChangeEventHandler<TimerState> OnStateChange`
    - 상태가 변경될 때마다 이전/현재 상태를 담아 호출됩니다.

#### 시간 관련 프로퍼티

- **`Duration`**
  - 타이머의 총 지속 시간입니다.
  - `Timer`에서는 `Run` 또는 `Pause` 상태에서만 변경할 수 있으며, 0 미만으로 설정 시 `InvalidTimeException`이 발생합니다.

- **`ElapsedTime`**
  - 경과 시간입니다.
  - 작동 중일 때 `Update`를 통해 증가하며, 0~`Duration` 사이로 클램핑됩니다.

- **`RemainingTime`**
  - 남은 시간입니다. `Duration - ElapsedTime`으로 계산됩니다.

- **정규화 값**
  - `ElapsedTime01` : `ElapsedTime / Duration`
  - `RemainingTime01` : `RemainingTime / Duration`

---

### Timer 클래스 동작

- **업데이트**
  - `Update(TValue deltaTime)`
    - 외부에서 전달한 `deltaTime`만큼 타이머를 진행시킵니다.
    - `IsRunning` 상태일 때만 동작하며, `elapsedTime`을 증가시킵니다.
    - 경과 시간이 `Duration`에 도달하면:
      1. `OnEnd` 이벤트를 호출
      2. `Stop()`을 호출하여 상태를 `Ready`로 되돌립니다.
  - `Update()`
    - 내부적으로 `Update(Time.deltaTime)`을 호출하는 편의 메서드입니다.

- **시작/정지/일시정지/재개/리셋**
  - `Start(duration)`
    - `Ready` 상태에서만 호출 가능 (`Run`/`Pause` 상태에서 호출 시 `AlreadyRunningException`)
    - 상태를 `Run`으로 변경하고, `Duration`을 설정한 뒤 `ElapsedTime`을 0으로 초기화합니다.
  - `Stop()`
    - `Run` 또는 `Pause` 상태에서 호출 가능
    - 상태를 `Ready`로 변경합니다.
  - `Pause()`
    - `Run` 상태에서만 호출 가능
    - 상태를 `Pause`로 변경하며, 경과 시간은 유지됩니다.
  - `Resume()`
    - `Pause` 상태에서만 호출 가능
    - 상태를 다시 `Run`으로 변경합니다.
  - `Reset()`
    - `Ready` 상태에서만 호출 가능 (`Run`/`Pause` 상태에서 호출 시 `FailedToResetException`)
    - `Duration`, `ElapsedTime`을 기본값(0)으로 초기화합니다.

---

### 예외 타입

타이머는 잘못된 사용을 방지하기 위해 전용 예외를 제공합니다.  
모두 `Timer.Exception`을 상속합니다.

- **`AlreadyRunningException`**
  - 타이머가 이미 `Run` 또는 `Pause` 상태인데 다시 `Start`를 호출했을 때 발생합니다.
  - 메시지: “타이머가 이미 작동 중입니다. 중지 후 시작해주세요.”

- **`FailedToResetException`**
  - 타이머가 `Run` 또는 `Pause` 상태일 때 `Reset`을 호출했을 때 발생합니다.
  - 메시지: “타이머가 작동 중이거나 일시정지 중입니다. 정지 후 리셋해주세요.”

- **`InvalidTimeException`**
  - `Duration`, `ElapsedTime`, `RemainingTime`에 0 미만의 값을 설정하려고 할 때 발생합니다.
  - 메시지: “시간 값은 0 이상이어야 합니다.”

---

### 사용 예시 (개념)

- **Update 루프에서 사용**
  - 게임 루프(예: `MonoBehaviour.Update`)에서 `timer.Update(Time.deltaTime)`를 호출하여  
    일정 시간이 지난 후 콜백을 실행하는 용도로 사용할 수 있습니다.

```csharp
using UnityEngine;

public class SimpleTimerExample : MonoBehaviour
{
    private Timer timer = new();

    private void Start()
    {
        // 3초 타이머 시작
        timer.Start(3.0f);
        timer.OnEnd += OnTimerEnd;
    }

    private void Update()
    {
        timer.Update(Time.deltaTime);
    }

    private void OnTimerEnd(object sender, TimerEndEventArgs e)
    {
        Debug.Log("3초 타이머 종료!");
    }
}
```

- **일시정지 가능한 카운트다운**
  - 스킬 쿨타임, 버프 지속 시간 등
  - UI에는 `RemainingTime` 또는 `RemainingTime01`을 사용해 남은 시간을 표시할 수 있습니다.

```csharp
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    [SerializeField]
    private Text cooldownText = null;

    private Timer cooldownTimer = new();

    public void UseSkill()
    {
        if (cooldownTimer.IsRunning || cooldownTimer.IsPaused)
        {
            // 아직 쿨타임이 끝나지 않음
            return;
        }

        // 스킬 사용 로직 ...

        cooldownTimer.Start(5.0f); // 5초 쿨타임
    }

    private void Update()
    {
        cooldownTimer.Update(Time.deltaTime);

        if (cooldownTimer.IsRunning || cooldownTimer.IsPaused)
        {
            cooldownText.text = cooldownTimer.RemainingTime.ToString("0.0");
        }
        else
        {
            cooldownText.text = string.Empty;
        }
    }
}
```

- **일시정지 / 재개 / 정지 흐름**
  - `Pause`, `Resume`, `Stop`를 이용해 타이머를 제어하는 예시입니다.

```csharp
using UnityEngine;

public class PauseableTimerExample : MonoBehaviour
{
    private Timer timer = new();

    private void Start()
    {
        timer.Start(10.0f); // 10초 타이머 시작
    }

    private void Update()
    {
        // 스페이스로 일시정지/재개 토글
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (timer.IsRunning)
            {
                timer.Pause();
                Debug.Log("타이머 일시정지");
            }
            else if (timer.IsPaused)
            {
                timer.Resume();
                Debug.Log("타이머 재개");
            }
        }

        // S 키로 정지
        if (Input.GetKeyDown(KeyCode.S))
        {
            timer.Stop();
            Debug.Log("타이머 정지");
        }

        timer.Update(Time.deltaTime);
    }
}
```

- **Editor 연동**
  - `cachedStartDuration` 필드는 에디터에서 시작 시간을 캐시하기 위한 용도로 사용됩니다.  
    런타임 로직에는 직접적인 영향을 주지 않습니다.


