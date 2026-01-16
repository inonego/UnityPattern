# 레벨 시스템 (Level System)

- 게임 내에서 레벨 값과 레벨 업/경험치(EXP) 로직을 일관되게 관리하기 위한 모듈입니다.
- 단순한 숫자 레벨 관리부터 경험치 테이블 기반의 자동 성장 시스템까지 지원합니다.

---

### 🏗️ 주요 타입 (Key Types)

- **[IReadOnlyLevel]**: 레벨 정보를 읽기만 가능한 인터페이스입니다. (UI, 로그용)
- **[ILevel]**: 레벨 변경 및 제어(LevelUp, LimitMax 설정 등)를 위한 인터페이스입니다.
- **[LevelBase]**: 공통 로직(최소/최대 레벨 범위, 레벨 업 차단 등)이 담긴 추상 클래스입니다.
- **[Level]**: 경험치 없이 직접 레벨 숫자만 조정하는 구현체입니다.
- **[LevelxEXP]**: 경험치 테이블을 기반으로 자동 레벨 업을 수행하는 구현체입니다.

---

### ⚙️ 주요 기능 (Key Features)

#### 1. 공통 기능
- **`Value`**: 현재 레벨 (0부터 시작)
- **`LimitMax`**: 동적으로 조절 가능한 레벨 상한
- **`BlockLevelUp`**: true 설정 시 레벨 업 조건을 충족해도 레벨이 오르지 않습니다.
- **`OnLevelUp`**: 레벨이 상승할 때마다 호출되는 이벤트입니다.

#### 2. 경험치 연동 (`LevelxEXP`)
- **`EXP`**: 현재 경험치입니다. 값을 증가시키면 필요 경험치 도달 시 자동으로 레벨이 오릅니다.
- **`MaxEXP`**: 현재 레벨에서 다음 레벨로 가기 위해 필요한 총 경험치입니다.
- **경험치 테이블**: 생성 시 `int[]` 배열을 통해 레벨별 필요 경험치를 정의합니다.

---

### 📝 사용 예시 (Examples)

#### 단순 레벨 (Level)
```csharp
var level = new Level(10); // 최대 10레벨
level.OnLevelUp += (s, e) => Debug.Log($"업! 현재: {e.Level}");
level.LevelUp(); // 0 -> 1
```

#### 경험치 기반 레벨 (LevelxEXP)
```csharp
int[] expTable = { 10, 20, 30 }; // 레벨 0->1, 1->2...
var levelExp = new LevelxEXP(expTable);

levelExp.EXP += 15; // 0레벨 필요 10 달성 -> 1레벨(남은 5 이월)
Debug.Log($"레벨: {levelExp.Value}, EXP: {levelExp.EXP}/{levelExp.MaxEXP}");
```

---

### ⚠️ 주의사항 (Cautions)

- **범위**: 레벨은 기본적으로 `0`부터 시작하며, `Min`과 `Max` 사이로 클램핑됩니다.
- **경험치 테이블**: i번째 인덱스 값은 레벨 i에서 i+1로 갈 때 필요한 경험치를 나타냅니다.
