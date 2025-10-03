using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using inonego;

public class TEST_GroundChecker2D
{
    private Sprite CreateSprite()
    {
        return Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 바닥 오브젝트를 생성합니다.
    /// </summary>
    // ------------------------------------------------------------
    private GameObject CreateGroundObject(int layer)
    {
        var groundObject = new GameObject("Ground");
        groundObject.transform.position = new Vector3(0f, -4f, 0f);
        groundObject.transform.localScale = new Vector3(20f, 4f, 20f);
        groundObject.layer = layer;

        var groundRigidbody = groundObject.AddComponent<Rigidbody2D>();
        var groundCollider = groundObject.AddComponent<BoxCollider2D>();
        var spriteRenderer = groundObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = CreateSprite();
        spriteRenderer.color = Color.white;

        groundRigidbody.bodyType = RigidbodyType2D.Kinematic;

        return groundObject;
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
        camera.transform.position = new Vector3(0f, 0f, -10f);

        return cameraObject;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 플레이어 위치를 반환합니다.
    /// </summary>
    // ------------------------------------------------------------
    private Vector3 GetPlayerPosition(int index)
    {
        var startX = -6f;
        var spacing = 4f;

        return new Vector3(startX + spacing * index, 1f, 0f);
    }
    
    // ------------------------------------------------------------
    /// <summary>
    /// 플레이어 오브젝트를 생성합니다.
    /// </summary>
    // ------------------------------------------------------------
    private GameObject CreatePlayerObject(string name, int index)
    {
        var playerObject = new GameObject(name);      
        
        // 부모 오브젝트를 생성합니다.
        var parent = new GameObject($"{playerObject.name}_Parent");
        parent.transform.position = playerObject.transform.position;
        playerObject.transform.SetParent(parent.transform);
        
        // 플레이어 오브젝트의 위치를 설정합니다.
        playerObject.transform.position = GetPlayerPosition(index);

        // 리지드바디
        var playerRigidbody = playerObject.AddComponent<Rigidbody2D>();

        playerRigidbody.bodyType = RigidbodyType2D.Kinematic;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        return playerObject;
    }
    
    [UnityTest]
    public IEnumerator GroundChecker2D_01_기본_바닥_감지_테스트()
    {
        var groundLayer = 8;

        var monoForTEST = new GameObject("MonoForTEST").AddComponent<MonoForTEST>();

        // ------------------------------------------------------------
        // 바닥 설정
        // ------------------------------------------------------------
        var groundObject = CreateGroundObject(groundLayer);
        
        // ------------------------------------------------------------
        // 카메라 설정
        // ------------------------------------------------------------
        var cameraObject = CreateCameraObject();

        // ------------------------------------------------------------
        // 4개 플레이어 설정 (간격 4, 일렬 배치)
        // ------------------------------------------------------------
        var players = new List<GameObject>();
        var groundCheckers = new List<GroundChecker2D>();
        var gizmoDrawers = new List<GroundChecker2DGizmoDrawer>();
        
        // BoxCollider2D 플레이어
        var boxPlayer = CreatePlayerObject("BoxPlayer", 0);
        var circlePlayer = CreatePlayerObject("CirclePlayer", 1);
        var verticalCapsulePlayer = CreatePlayerObject("VerticalCapsulePlayer", 2);
        var horizontalCapsulePlayer = CreatePlayerObject("HorizontalCapsulePlayer", 3);

        boxPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
        var boxCollider = boxPlayer.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(1f, 1f);
        
        // CircleCollider2D 플레이어
        circlePlayer.transform.localScale = new Vector3(1f, 1f, 1f);
        var circleCollider = circlePlayer.AddComponent<CircleCollider2D>();
        circleCollider.radius = 0.5f;
        
        // CapsuleCollider2D(수직) 플레이어
        verticalCapsulePlayer.transform.localScale = new Vector3(1f, 2f, 1f);
        var verticalCapsuleCollider = verticalCapsulePlayer.AddComponent<CapsuleCollider2D>();
        verticalCapsuleCollider.direction = CapsuleDirection2D.Vertical;
        verticalCapsuleCollider.size = new Vector2(1f, 2f);

        // CapsuleCollider2D(수평) 플레이어
        horizontalCapsulePlayer.transform.localScale = new Vector3(2f, 1f, 1f);
        var horizontalCapsuleCollider = horizontalCapsulePlayer.AddComponent<CapsuleCollider2D>();
        horizontalCapsuleCollider.direction = CapsuleDirection2D.Horizontal;
        horizontalCapsuleCollider.size = new Vector2(2f, 1f);
        
        players.Add(boxPlayer);
        players.Add(circlePlayer);
        players.Add(verticalCapsulePlayer);
        players.Add(horizontalCapsulePlayer);
        
        // ------------------------------------------------------------
        // GroundChecker2D 및 테스터 설정
        // ------------------------------------------------------------
        foreach (var player in players)
        {
            var groundChecker = new GroundChecker2D(player);
            groundChecker.Config = new GroundCheckerConfig { Layer = 1 << groundLayer, Depth = 0.1f };
            groundCheckers.Add(groundChecker);
            
            var gizmoDrawer = player.AddComponent<GroundChecker2DGizmoDrawer>();
            gizmoDrawer.Initialize(groundChecker);
            gizmoDrawers.Add(gizmoDrawer);
        }

        // 이벤트 카운터 초기화
        var landEventCount = new int[groundCheckers.Count];
        var leaveEventCount = new int[groundCheckers.Count];
        
        // 이벤트 구독
        for (int i = 0; i < groundCheckers.Count; i++)
        {
            int index = i; // 클로저를 위한 캡처
            groundCheckers[i].OnLand += (groundChecker, gameObject) => 
            {
                landEventCount[index]++;
                Debug.Log($"Land 이벤트 발생: {players[index].name} (카운트: {landEventCount[index]})");
            };
            groundCheckers[i].OnLeave += (groundChecker, gameObject) => 
            {
                leaveEventCount[index]++;
                Debug.Log($"OnLeave 이벤트 발생: {players[index].name} (카운트: {leaveEventCount[index]})");
            };
        }
        
        IEnumerator Update()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
            
                // Act - 모든 플레이어의 바닥 감지 테스트
                foreach (var groundChecker in groundCheckers)
                {
                    groundChecker.Check(Time.fixedDeltaTime);
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    break;
                }
            }
        }

        // Update 시작
        monoForTEST.StartCoroutine(Update());

        // 1. 처음 3초 대기
        yield return new WaitForSeconds(3f);
        
        // 2. Dynamic으로 변경
        foreach (var player in players)
        {
            var rigidbody = player.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        // 3. Land 이벤트가 모든 오브젝트에서 딱 한번씩만 호출되는지 확인 (5초 유예)
        var landEventTriggered = new bool[groundCheckers.Count];
        
        IEnumerator WaitForLandEvents(List<GroundChecker2D> groundCheckers, bool[] landEventTriggered, float timeout)
        {
            float timer = 0f;
            var playerLandTimes = new float[groundCheckers.Count];
            var playerWaitTimes = new float[groundCheckers.Count];
            const float waitAfterLand = 3f;
            
            while (timer < timeout)
            {
                for (int i = 0; i < groundCheckers.Count; i++)
                {
                    if (!landEventTriggered[i] && groundCheckers[i].IsOnGround)
                    {
                        landEventTriggered[i] = true;
                        playerLandTimes[i] = timer;
                        playerWaitTimes[i] = 0f;
                        Debug.Log($"{players[i].name} 착지! 3초 대기 시작...");
                    }
                }
                
                // 각 플레이어의 대기 시간 업데이트 및 상태 체크
                for (int i = 0; i < groundCheckers.Count; i++)
                {
                    if (landEventTriggered[i])
                    {
                        playerWaitTimes[i] += Time.deltaTime;
                        
                        // Land 이벤트 대기 중에는 Leave 이벤트가 발생하면 안됨
                        if (leaveEventCount[i] > 0)
                        {
                            Debug.LogError($"{players[i].name} Land 대기 중에 OnLeave 이벤트가 {leaveEventCount[i]}번 발생했습니다!");
                            Assert.Fail($"Land 대기 중에 OnLeave 이벤트가 발생했습니다: {players[i].name}");
                        }
                        
                        // 3초 대기 완료된 플레이어 체크
                        if (playerWaitTimes[i] >= waitAfterLand)
                        {
                            if (landEventCount[i] != 1)
                            {
                                Debug.LogError($"{players[i].name} Land 이벤트가 {landEventCount[i]}번 발생했습니다! (예상: 1번)");
                                Assert.Fail($"Land 이벤트가 올바르게 호출되지 않았습니다: {players[i].name}");
                            }
                        }
                    }
                }
                
                // 모든 플레이어가 착지하고 3초씩 대기 완료했는지 확인
                bool allCompleted = true;
                for (int i = 0; i < groundCheckers.Count; i++)
                {
                    if (!landEventTriggered[i] || playerWaitTimes[i] < waitAfterLand)
                    {
                        allCompleted = false;
                        break;
                    }
                }
                
                if (allCompleted)
                {
                    Debug.Log("모든 플레이어의 Land 이벤트 체크 완료!");
                    yield break;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            Debug.LogError($"Land 이벤트 타임아웃! {timeout}초 초과");
        }

        yield return monoForTEST.StartCoroutine(WaitForLandEvents(groundCheckers, landEventTriggered, 5f));
        
        // Land 이벤트 검증
        bool landTestPassed = true;
        for (int i = 0; i < groundCheckers.Count; i++)
        {
            if (!landEventTriggered[i] || landEventCount[i] != 1)
            {
                landTestPassed = false;
                Debug.LogError($"Land 이벤트 실패: {players[i].name} - 호출됨: {landEventTriggered[i]}, 횟수: {landEventCount[i]}");
            }
        }
        
        if (!landTestPassed)
        {
            Debug.LogError("Land 이벤트 테스트 실패!");
            Assert.Fail("Land 이벤트가 올바르게 호출되지 않았습니다.");
        }

        // 4. 모든 오브젝트가 바닥에 닿고 점프
        foreach (var player in players)
        {
            var rigidbody = player.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(Vector2.up * 15f, ForceMode2D.Impulse);
            }
        }

        // 5. OnLeave 이벤트가 한번씩만 호출되는지 확인 (5초 유예)
        var leaveEventTriggered = new bool[groundCheckers.Count];
        
        IEnumerator WaitForLeaveEvents(List<GroundChecker2D> groundCheckers, bool[] leaveEventTriggered, float timeout)
        {
            float timer = 0f;
            var playerLeaveTimes = new float[groundCheckers.Count];
            var playerWaitTimes = new float[groundCheckers.Count];
            const float waitAfterLeave = 3f;
            
            while (timer < timeout)
            {
                for (int i = 0; i < groundCheckers.Count; i++)
                {
                    if (!leaveEventTriggered[i] && !groundCheckers[i].IsOnGround)
                    {
                        leaveEventTriggered[i] = true;
                        playerLeaveTimes[i] = timer;
                        playerWaitTimes[i] = 0f;
                        Debug.Log($"{players[i].name} 바닥 이탈! 3초 대기 시작...");
                    }
                }
                
                // 각 플레이어의 대기 시간 업데이트 및 상태 체크
                for (int i = 0; i < groundCheckers.Count; i++)
                {
                    if (leaveEventTriggered[i])
                    {
                        playerWaitTimes[i] += Time.deltaTime;
                        
                        // Leave 이벤트 대기 중에는 Land 이벤트가 추가로 발생하면 안됨 (이미 1번은 발생했으므로)
                        if (landEventCount[i] > 1)
                        {
                            Debug.LogError($"{players[i].name} OnLeave 대기 중에 Land 이벤트가 추가로 {landEventCount[i]}번 발생했습니다!");
                            Assert.Fail($"OnLeave 대기 중에 Land 이벤트가 추가로 발생했습니다: {players[i].name}");
                        }
                        
                        // 3초 대기 완료된 플레이어 체크
                        if (playerWaitTimes[i] >= waitAfterLeave)
                        {
                            if (leaveEventCount[i] != 1)
                            {
                                Debug.LogError($"{players[i].name} OnLeave 이벤트가 {leaveEventCount[i]}번 발생했습니다! (예상: 1번)");
                                Assert.Fail($"OnLeave 이벤트가 올바르게 호출되지 않았습니다: {players[i].name}");
                            }
                        }
                    }
                }
                
                // 모든 플레이어가 바닥을 떠나고 3초씩 대기 완료했는지 확인
                bool allCompleted = true;
                for (int i = 0; i < groundCheckers.Count; i++)
                {
                    if (!leaveEventTriggered[i] || playerWaitTimes[i] < waitAfterLeave)
                    {
                        allCompleted = false;
                        break;
                    }
                }
                
                if (allCompleted)
                {
                    Debug.Log("모든 플레이어의 OnLeave 이벤트 체크 완료!");
                    yield break;
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            Debug.LogError($"OnLeave 이벤트 타임아웃! {timeout}초 초과");
        }

        yield return monoForTEST.StartCoroutine(WaitForLeaveEvents(groundCheckers, leaveEventTriggered, 5f));
        
        // OnLeave 이벤트 검증
        bool leaveTestPassed = true;
        for (int i = 0; i < groundCheckers.Count; i++)
        {
            if (!leaveEventTriggered[i] || leaveEventCount[i] != 1)
            {
                leaveTestPassed = false;
                Debug.LogError($"OnLeave 이벤트 실패: {players[i].name} - 호출됨: {leaveEventTriggered[i]}, 횟수: {leaveEventCount[i]}");
            }
        }
        
        if (!leaveTestPassed)
        {
            Debug.LogError("OnLeave 이벤트 테스트 실패!");
            Assert.Fail("OnLeave 이벤트가 올바르게 호출되지 않았습니다.");
        }

        // 6. 원래 위치로 돌려놓고 kinematic으로 설정
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            player.transform.position = GetPlayerPosition(i);
            var rigidbody = player.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
                rigidbody.linearVelocity = Vector2.zero;
                rigidbody.angularVelocity = 0f;
            }
        }

        // 7. Space바를 눌러서 종료 및 테스트 성공
        Debug.Log("테스트 성공! Space바를 눌러서 종료하세요.");
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("테스트 완료!");
                break;
            }
            yield return null;
        }
    }
}
