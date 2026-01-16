# 보드 시스템 (Board System)

- 격자(Grid) 형태의 공간에서 객체를 배치, 이동, 제거하기 위한 핵심 자료구조입니다.
- 보드 게임, 타일맵, 퍼즐 등 다양한 장르에서 재사용 가능한 유연한 구조를 제공합니다.

---

### 🏗️ 주요 타입 (Key Types)

- **[BoardBase<...>]**: 모든 보드 구현의 공통 베이스 클래스입니다. 좌표-공간-객체 간의 매핑을 관리합니다.
- **[Board2D<T>] / [Board3D<T>]**: 2차원(`Vector2Int`) 및 3차원(`Vector3Int`) 격자 전용 구현체입니다. 생성 시 크기를 지정하여 전체 공간을 자동 초기화할 수 있습니다.
- **[BoardSpace<T>]**: 한 좌표 내에서 정수 인덱스 기반으로 여러 객체(슬롯)를 관리할 수 있는 공간 구현체입니다.

---

### ⚙️ 핵심 동작 (Core Methods)

#### 1. 인덱서 및 조회
- **`board[vector]`**: 해당 위치의 공간(`TSpace`)을 반환합니다.
- **`board[vector, index]`**: 특정 좌표/인덱스의 배치된 객체를 반환합니다.
- **`board[placeable]`**: 특정 객체가 위치한 좌표(`Point?`)를 즉시 조회합니다.

#### 2. 공간 및 배치 관리
- **`AddSpace(vector)` / `RemoveSpace(vector)`**: 특정 좌표의 공간을 동적으로 추가하거나 제거합니다.
- **`CanPlace(vector, index, item)`**: 배치가 가능한지 사전에 검증합니다.
- **`Place(vector, index, item)`**: 객체를 지정된 위치에 배치합니다. 이미 다른 곳에 있었다면 이전 위치에서 제거 후 이동합니다.
- **`Remove(item)` / `RemoveAll()`**: 특정 객체 또는 보드 전체의 객체를 제거합니다.

---

### 🔗 이벤트 (Events)

- **`OnPlace` / `OnRemove`**: 객체가 보드에 배치되거나 제거될 때 발생합니다.
- **`OnAddSpace` / `OnRemoveSpace`**: 보드 공간 자체가 추가되거나 삭제될 때 발생합니다.
- 💡 이 이벤트들을 활용하여 비주얼 연출이나 UI를 보드 상태와 쉽게 동기화할 수 있습니다.

---

### 📝 사용 예시 (Examples)

#### 1. 2D 보드 생성 및 배치 (기본 초기화)
```csharp
var board = new Board2D<Tile>(10, 5); // 10x5 크기
var pos = new Vector2Int(3, 2);

if (board.CanPlace(pos, 0)) {
    board.Place(pos, 0, new Tile());
}
```

#### 2. 동적 공간 생성 (수동 초기화)
```csharp
var board = new Board2D<Unit>(10, 5, init: false);
board.AddSpace(new Vector2Int(0, 0)); // 필요한 칸만 생성
board.Place(new Vector2Int(0, 0), 0, new Unit());
```

#### 3. 3D 보드 위치 조회
```csharp
var board3D = new Board3D<Block>(4, 4, 4);
board3D.Place(new Vector3Int(1, 1, 1), 0, block);

var point = board3D[block]; // Block 위치 조회
if (point.HasValue) Debug.Log($"위치: {point.Value.Vector}");
```

---

### ⚠️ 주의사항 (Cautions)

- **공간 중복**: `AddSpace` 시 이미 존재하는 위치라면 예외가 발생합니다.
- **유효성**: 인덱서 접근 시 범위를 벗어나거나 공간이 없는 경우 `null`을 반환하므로 주의가 필요합니다.
