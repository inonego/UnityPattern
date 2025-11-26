## 보드

### 개요

`보드`는 격자(Grid) 형태의 공간에 **객체를 배치/이동/제거**하기 위한 핵심 자료구조입니다.

- 공통 로직을 담고 있는 제네릭 추상 클래스 `BoardBase`
- 2D / 3D 전용 구현인 `Board2D`, `Board3D`
- 한 좌표 내 여러 칸을 관리하는 `BoardSpace`

으로 구성되어, 보드 게임, 타일맵, 퍼즐, 배치형 게임 등 다양한 상황에 재사용할 수 있도록 설계되어 있습니다.

---

### 주요 타입

- **`BoardBase<TVector, TIndex, TSpace, TPlaceable>`**
  - 모든 보드 구현의 **공통 베이스 클래스**입니다.
  - 제네릭 매개변수:
    - `TVector` : 보드 상의 위치를 나타내는 벡터 타입 (예: `Vector2Int`, `Vector3Int`)
    - `TIndex` : 한 공간(`TSpace`) 안에서의 슬롯/인덱스를 나타내는 타입 (예: `int`)
    - `TSpace` : 한 좌표에 대응하는 “공간” 타입 (`BoardSpaceBase<TIndex, TPlaceable>` 파생형)
    - `TPlaceable` : 보드 위에 배치되는 객체 타입
  - 주요 역할:
    - 좌표 → 공간(`spaceMap`), 객체 → 좌표(`pointMap`) 매핑 관리
    - 공간 추가/제거 (`AddSpace`, `RemoveSpace`, `RemoveSpaceAll`)
    - 객체 배치/제거 (`Place`, `Remove`, `RemoveAll`)
    - 배치 가능 여부 확인 (`CanPlace`)
    - 배치/제거/공간 추가/삭제 이벤트 발행

- **`Board2D<TPlaceable>` / `Board2D<TIndex, TSpace, TPlaceable>`**
  - 2D 격자(Width, Height)를 다루는 보드 구현입니다.
  - `Vector2Int`를 `TVector`로 사용하며,  
    `Width`, `Height`, `Size` 프로퍼티와 범위 체크(`IsValidVector`)를 제공합니다.
  - 생성자에서 (width, height)를 받아 모든 유효 좌표에 대해 자동으로 공간을 생성(`Init`)할 수 있습니다.

- **`Board3D<TPlaceable>` / `Board3D<TIndex, TSpace, TPlaceable>`**
  - 3D 격자(Width, Height, Depth)를 다루는 보드 구현입니다.
  - `Vector3Int`를 `TVector`로 사용하며,  
    `Width`, `Height`, `Depth`, `Size` 프로퍼티와 범위 체크(`IsValidVector`)를 제공합니다.
  - 마찬가지로 생성자에서 (width, height, depth)를 받아 3D 전체 공간을 초기화할 수 있습니다.

- **`BoardSpace<TPlaceable>`**
  - 정수 인덱스(`int`) 기반의 기본 보드 공간 구현입니다.
  - `BoardSpaceBase<int, TPlaceable>`를 상속받으며, 한 좌표 내 여러 슬롯(예: 여러 개의 유닛, 스택 등)을 관리할 수 있습니다.

---

### 핵심 동작

#### 인덱서(접근자)

- `board[vector]`
  - 해당 위치의 공간(`TSpace`)을 반환합니다.
  - 유효하지 않은 벡터이거나 공간이 없으면 `null` 반환.

- `board[vector, index]`
  - 해당 위치/인덱스에 배치된 `TPlaceable` 객체를 반환합니다.

- `board[point]` (`IBoardPoint<TVector, TIndex>`)
  - 포인트 인터페이스를 사용해 위치를 간접적으로 참조합니다.

- `board[placeable]`
  - 특정 객체가 보드상 어디에 있는지(`Point?`) 조회할 수 있습니다.

#### 공간 관리

- `AddSpace(TVector vector, bool invokeEvent = true)`
  - 지정된 위치에 새로운 공간을 추가합니다.
  - 이미 공간이 존재하면 예외를 발생시켜, 중복 생성을 방지합니다.

- `RemoveSpace(TVector vector, bool invokeEvent = true)`
  - 해당 위치의 공간을 제거하고, 그 안에 있는 모든 객체도 함께 제거합니다.

- `RemoveSpaceAll(bool invokeEvent = true)`
  - 모든 위치의 공간과 객체를 한 번에 제거합니다.

#### 배치 및 제거

- `CanPlace(TVector vector, TIndex index, TPlaceable placeable = null)`
  - 해당 위치/인덱스에 객체를 배치할 수 있는지 사전에 확인합니다.
  - 내부적으로 해당 좌표의 `TSpace.CanPlace`를 호출합니다.

- `Place(TVector vector, TIndex index, TPlaceable placeable, bool invokeEvent = true)`
  - 지정된 위치에 객체를 배치합니다.
  - 원래 다른 위치에 있던 객체라면 먼저 제거한 뒤, 새 위치에 배치합니다.
  - 성공 시:
    - 공간 내부에 객체를 기록하고
    - `pointMap`에 객체의 위치를 갱신
    - `OnPlace` 이벤트를 호출합니다.

- `Remove(TVector vector, TIndex index, bool invokeEvent = true)`
  - 특정 위치/인덱스의 객체를 제거합니다.

- `Remove(TPlaceable placeable, bool invokeEvent = true)`
  - 객체를 기준으로 위치를 찾은 뒤, 그 위치에서 제거합니다.

- `Remove(TVector vector, bool invokeEvent = true)`
  - 한 좌표(`TVector`) 안에 있는 **모든 객체**를 제거합니다.

- `RemoveAll(bool invokeEvent = true)`
  - 보드 전체의 모든 객체를 제거합니다.

---

### 이벤트

- `event Action<TVector, TIndex, TPlaceable> OnPlace`
  - 객체가 보드에 배치될 때 호출됩니다.

- `event Action<TVector, TIndex, TPlaceable> OnRemove`
  - 객체가 보드에서 제거될 때 호출됩니다.

- `event Action<TVector> OnAddSpace`
  - 새로운 공간이 추가될 때 호출됩니다.

- `event Action<TVector> OnRemoveSpace`
  - 공간이 제거될 때 호출됩니다.

이 이벤트들을 활용하면, 보드와 별개로 **비주얼, 이펙트, UI, 로그** 등을 쉽게 연동할 수 있습니다.

---

### 사용 예시 (개념)

- **2D 보드 생성 및 배치 (`init = true` 기본 초기화)**

```csharp
// 보드 위에 배치될 객체 타입
public class Tile {}

// 10x5 크기의 2D 보드 생성
var board = new Board2D<Tile>(10, 5);

// 특정 위치에 타일 배치
var tile = new Tile();
var pos = new Vector2Int(3, 2);

if (board.CanPlace(pos, 0, tile))
{
    board.Place(pos, 0, tile);
}
```

- **2D 보드에서 수동으로 공간 초기화 (`init = false` + `AddSpace`)**

```csharp
public class Unit {}

// init = false 이므로 생성 시에는 spaceMap이 비어 있습니다.
var board = new Board2D<Unit>(10, 5, init: false);

// 필요한 위치에만 수동으로 공간을 생성
board.AddSpace(new Vector2Int(0, 0));
board.AddSpace(new Vector2Int(1, 0));

var unit = new Unit();
var pos = new Vector2Int(1, 0);

if (board.CanPlace(pos, 0, unit))
{
    board.Place(pos, 0, unit);
}
```

- **3D 보드에서 객체 위치 조회**

```csharp
public class Block {}

var board3D = new Board3D<Block>(4, 4, 4);
var block = new Block();

var p = new Vector3Int(1, 1, 1);
board3D.Place(p, 0, block);

// block이 어디에 있는지 조회
var point = board3D[block];
if (point.HasValue)
{
    Debug.Log($"Block 위치: {point.Value.Vector}, 인덱스: {point.Value.Index}");
}
```


