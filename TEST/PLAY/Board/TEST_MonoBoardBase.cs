using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using inonego;

using Object = UnityEngine.Object;

// ============================================================
/// <summary>
/// MonoBoardBase 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================
public class TEST_MonoBoardBase
{
    // ------------------------------------------------------------
    /// <summary>
    /// Space키 입력을 체크합니다.
    /// </summary>
    // ------------------------------------------------------------
    private bool IsSpaceKeyPressed()
    {
    
    #if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    #else
        return Input.GetKeyDown(KeyCode.Space);
    #endif
    
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 기물입니다.
    /// </summary>
    // ------------------------------------------------------------
    public class TestPiece
    {
        public string Name;
        public TestPiece() {}
        public TestPiece(string name) { Name = name; }
        public override string ToString() => Name;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 공간입니다.
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    public class TestSpace : BoardSpace<TestPiece>
    {
        public TestSpace() {}
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 타일입니다.
    /// </summary>
    // ------------------------------------------------------------
    public class TestMonoTile : MonoBehaviour { }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 MonoBoardBase 구현체입니다.
    /// </summary>
    // ------------------------------------------------------------
    public class TestMonoBoard2D : MonoBoardBase<Board2D<int, TestSpace, TestPiece>, Vector2Int, int, TestSpace, TestPiece, TestMonoTile>
    {
        [SerializeField]
        private float lTileSize = 1f;

        public override Vector3 ToLocalPos(Vector2Int vector)
        {
            return new Vector3(vector.x * lTileSize, 0f, vector.y * lTileSize);
        }

        public override Vector3 ToLocalPos(Vector2Int vector, int index)
        {
            return ToLocalPos(vector) + Vector3.zero;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타일 프로바이더를 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void SetTileProvider(IGameObjectProvider provider)
        {
            lTileProvider = provider;
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 타일 프로바이더를 생성합니다.
    /// </summary>
    // ------------------------------------------------------------
    private PrefabGameObjectProvider CreateTileProvider()
    {
        // 테스트용 프리팹 생성
        var prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prefab.AddComponent<TestMonoTile>();

        prefab.name = "TilePrefab";
        prefab.transform.localScale = Vector3.one * 0.9f;
        
        // PrefabGameObjectProvider 생성 및 설정
        var provider = new PrefabGameObjectProvider(prefab, null);
        
        return provider;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 카메라 오브젝트를 생성합니다.
    /// </summary>
    // ------------------------------------------------------------
    private GameObject CreateCameraObject()
    {
        var cameraObject = new GameObject("Camera");

        var camera = cameraObject.AddComponent<Camera>();
        camera.transform.position = new Vector3(0f, 5f, -10f);
        camera.transform.rotation = Quaternion.Euler(30f, 0f, 0f);

        return cameraObject;
    }

    private MonoForTEST monoForTEST;
    private GameObject cameraObject;

    // ------------------------------------------------------------
    /// <summary>
    /// 각 테스트 시작 전에 실행됩니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        monoForTEST = new GameObject("MonoForTEST").AddComponent<MonoForTEST>();

        cameraObject = CreateCameraObject();

        yield return null;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 각 테스트 종료 후에 실행됩니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (monoForTEST != null)
        {
            Object.Destroy(monoForTEST.gameObject);
        }

        if (cameraObject != null)
        {
            Object.Destroy(cameraObject);
        }

        // 모든 게임 오브젝트 정리
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.name != "Main Camera" && obj.name != "Directional Light")
            {
                Object.Destroy(obj);
            }
        }

        yield return null;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 MonoBoard를 생성합니다.
    /// </summary>
    // ------------------------------------------------------------
    private TestMonoBoard2D CreateMonoBoard()
    {
        var monoBoardObject = new GameObject("TestMonoBoard2D");
        var monoBoard = monoBoardObject.AddComponent<TestMonoBoard2D>();

        var tileProvider = CreateTileProvider();
        monoBoard.SetTileProvider(tileProvider);

        return monoBoard;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Space 키 입력을 기다립니다.
    /// </summary>
    // ------------------------------------------------------------
    private IEnumerator WaitForSpaceKey(string message)
    {
        Debug.Log(message);

        while (!IsSpaceKeyPressed())
        {
            yield return null;
        }

        Debug.Log("테스트 완료!");
    }

    [UnityTest]
    public IEnumerator MonoBoardBase_종합_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // 3x3 보드 생성 (초기 자동 공간 생성 비활성화)
        // ------------------------------------------------------------
        var board = new Board2D<int, TestSpace, TestPiece>(3, 3, init: false);
        var monoBoard = CreateMonoBoard();

        var basePoints = new[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        };

        foreach (var point in basePoints)
        {
            board.AddSpace(point);
        }

        // ------------------------------------------------------------
        // 초기 상태 확인 및 보드 초기화
        // ------------------------------------------------------------
        Assert.That(monoBoard.Board, Is.Null);
        Assert.That(monoBoard.TileMap.Count, Is.EqualTo(0));

        monoBoard.Connect(board);

        yield return new WaitForSeconds(0.5f);

        Assert.That(monoBoard.Board, Is.EqualTo(board));
        Assert.That(monoBoard.TileMap.Count, Is.EqualTo(basePoints.Length));

        foreach (var point in basePoints)
        {
            var lTile = monoBoard.TileMap[point];
            var expectedPos = monoBoard.ToLocalPos(point);

            Assert.That(monoBoard.TileMap.ContainsKey(point), Is.True);
            Assert.That(lTile, Is.Not.Null);
            Assert.That(Vector3.Distance(lTile.transform.localPosition, expectedPos), Is.LessThan(0.01f));
        }

        // ------------------------------------------------------------
        // 공간 추가/제거 이벤트 확인
        // ------------------------------------------------------------
        // 추가 공간 생성 이벤트 확인
        var newPointA = new Vector2Int(2, 0);
        board.AddSpace(newPointA);

        yield return null;

        Assert.That(monoBoard.TileMap.ContainsKey(newPointA), Is.True);

        // 추가 공간 생성 이벤트 확인
        var newPointB = new Vector2Int(2, 1);
        board.AddSpace(newPointB);

        yield return null;

        // 제거 공간 이벤트 확인
        var removePoint = new Vector2Int(0, 0);
        board.RemoveSpace(removePoint);

        yield return null;

        Assert.That(monoBoard.TileMap.ContainsKey(removePoint), Is.False);

        // ------------------------------------------------------------
        // ReloadTileMap 검증
        // ------------------------------------------------------------
        monoBoard.ReloadTileMap();

        yield return null;

        int spaceCount = 0;

        foreach (var kvp in board)
        {
            var point = kvp.Key;
            var space = kvp.Value;
        
            spaceCount++;

            var lTile = monoBoard.TileMap[point];
            var expectedPos = monoBoard.ToLocalPos(point);
        
            Assert.That(monoBoard.TileMap.ContainsKey(point), Is.True);
            Assert.That(lTile, Is.Not.Null);
            Assert.That(Vector3.Distance(lTile.transform.localPosition, expectedPos), Is.LessThan(0.01f));
        }

        Assert.That(monoBoard.TileMap.Count, Is.EqualTo(spaceCount));

        // ------------------------------------------------------------
        // Disconnect 검증
        // ------------------------------------------------------------
        monoBoard.Disconnect();

        yield return null;

        Assert.That(monoBoard.Board, Is.Null);
        Assert.That(monoBoard.TileMap.Count, Is.EqualTo(0));

        // ------------------------------------------------------------
        // Space바를 눌러서 종료
        // ------------------------------------------------------------
        yield return WaitForSpaceKey("초기화/이벤트/동기화/해제 통합 테스트 성공! Space바를 눌러서 종료하세요.");
    }
}

