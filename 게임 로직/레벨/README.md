## 레벨

### 개요

`레벨` 시스템은 게임에서 **레벨 값과 레벨 업/경험치**를 일관되게 관리하기 위한 유틸리티입니다.

- 레벨 값을 읽기/쓰기 위한 인터페이스 제공
- 순수 레벨 전용 구현과 경험치(_EXP_) 연동 구현 분리
- 최대 레벨 제한, 레벨 업 차단 플래그 등 공통 기능 내장

---

### 주요 타입

- **`IReadOnlyLevel`**
  - 레벨 정보를 **읽기 전용으로 노출**하기 위한 인터페이스입니다.
  - UI, HUD, 로그 등 “보기 전용” 코드에서 사용합니다.

- **`ILevel`**
  - 실제로 레벨을 **변경/제어**하는 쪽에서 사용하는 인터페이스입니다.
  - `Value`, `LimitMax`, `BlockLevelUp`을 변경할 수 있고, `LevelUp(int amount)` 메서드를 제공합니다.

- **`LevelBase`**
  - `IReadOnlyLevel`/`ILevel` 공통 로직을 담은 추상 클래스입니다.
  - 공통 규칙:
    - 레벨은 0부터 시작
    - `Min = 0`, `Max = min(LimitMax, FullMax)`
    - `CanLevelUp`이 true일 때만 레벨 업 가능
  - `FullMax`는 파생 클래스에서 구현합니다.

- **`Level`**
  - **경험치 없이 레벨 숫자만 관리**하는 구현입니다.
  - 생성자에서 최대 레벨(`FullMax`)을 정수 하나로 지정합니다.

- **`LevelxEXP`**
  - **경험치 테이블 기반으로 자동 레벨 업**을 처리하는 구현입니다.
  - 레벨/최대 레벨 정보는 `LevelBase`를 따르고,
  - 추가로 경험치 관련 필드를 제공합니다.

---

### 주요 기능

- **공통 인터페이스 기능 (`IReadOnlyLevel`, `ILevel`)**
  - `Value` : 현재 레벨
  - `Min` / `Max` : 최소 / 현재 도달 가능한 최대 레벨
  - `LimitMax` : 동적으로 조절 가능한 최대 레벨 상한
  - `FullMax` : 절대 최대 레벨 (생성 시 결정)
  - `BlockLevelUp` : 레벨 업 차단 여부
  - `CanLevelUp` : 현재 상태에서 레벨 업 가능 여부
  - `OnLevelUp` : 레벨이 1 오를 때마다 호출되는 이벤트

- **경험치 기반 기능 (`LevelxEXP`)**
  - `int[] requiredEXPToLevelUpArray`
    - i번째 값 = 레벨 i → i+1로 가기 위해 필요한 EXP
  - `int EXP`
    - 값을 증가시키면, 내부에서 `CanLevelUp`과 테이블을 참고해  
      필요한 만큼 레벨 업을 수행하고 남은 EXP를 보정합니다.
  - `int MaxEXP`
    - 현재 레벨에서 다음 레벨로 가기 위해 필요한 EXP

---

### 사용 예시 (개념)

- **단순 레벨 사용 (`Level`)**

```csharp
// 최대 레벨 10인 레벨 객체 생성
var level = new Level(10);

// 레벨 업 이벤트 구독
level.OnLevelUp += (sender, args) =>
{
    Debug.Log($"레벨 업! 현재 레벨: {args.Level}");
};

level.LevelUp();      // 0 → 1
level.LevelUp(2);     // 1 → 3
```

- **경험치 기반 레벨 사용 (`LevelxEXP`)**

```csharp
// 레벨 0→1, 1→2, 2→3에 필요한 EXP 테이블
int[] expTable = { 10, 20, 30 };
var levelExp = new LevelxEXP(expTable);

// 경험치 누적
levelExp.EXP += 15; 
// 0레벨에서 MaxEXP = 10 → 레벨 1, 남은 EXP = 5

Debug.Log($"레벨: {levelExp.Value}, EXP: {levelExp.EXP} / {levelExp.MaxEXP}");
```


