using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using inonego;
using inonego.Pool;

// ============================================================================
/// <summary>
/// GOPool 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_Pool_GOPool
{

#region GOPool 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_01_기본_생성_테스트()
    {
        // Arrange & Act
        var pool = new GOPool();

        // Assert
        Assert.IsNotNull(pool);
        Assert.IsNotNull(pool.GameObjectProvider);
        Assert.AreEqual(0, pool.Released.Count);
        Assert.AreEqual(0, pool.Acquired.Count);

        yield return null;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 GameObject 획득을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_02_Acquire_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOPool(provider);

        GameObject go1 = null;
        GameObject go2 = null;

        try
        {
            // Act
            go1 = pool.Acquire();
            go2 = pool.Acquire();

            yield return null;

            // Assert
            Assert.IsNotNull(go1);
            Assert.IsNotNull(go2);
            Assert.AreNotSame(go1, go2);
            Assert.AreEqual(2, pool.Acquired.Count);
            Assert.AreEqual(0, pool.Released.Count);
            Assert.IsTrue(go1.activeSelf, "획득한 GameObject는 활성화되어야 합니다");
            Assert.IsTrue(go2.activeSelf, "획득한 GameObject는 활성화되어야 합니다");
        }
        finally
        {
            // Cleanup - 획득한 GameObject들 정리
            if (go1 != null) GameObject.DestroyImmediate(go1);
            if (go2 != null) GameObject.DestroyImmediate(go2);
            GameObject.DestroyImmediate(prefab);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 GameObject 반환을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_03_Release_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var poolParent = new GameObject("PoolParent");
        var pool = new GOPool(provider) { Pool = poolParent.transform };

        GameObject go = null;

        try
        {
            go = pool.Acquire();

            yield return null;

            // Act
            pool.Release(go);

            yield return null;

            // Assert
            Assert.AreEqual(0, pool.Acquired.Count);
            Assert.AreEqual(1, pool.Released.Count);
            Assert.IsFalse(go.activeSelf, "반환된 GameObject는 비활성화되어야 합니다");
            Assert.AreEqual(poolParent.transform, go.transform.parent, "반환된 GameObject는 Pool 부모로 이동해야 합니다");
        }
        finally
        {
            // Cleanup - 획득한 GameObject 정리
            if (go != null) GameObject.DestroyImmediate(go);
            GameObject.DestroyImmediate(prefab);
            GameObject.DestroyImmediate(poolParent);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 GameObject 재사용을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_04_재사용_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOPool(provider);

        GameObject go1 = null;
        GameObject go2 = null;

        try
        {
            go1 = pool.Acquire();
            var instanceId = go1.GetInstanceID();

            yield return null;

            pool.Release(go1);

            yield return null;

            // Act
            go2 = pool.Acquire();

            yield return null;

            // Assert
            Assert.AreEqual(instanceId, go2.GetInstanceID(), "같은 GameObject가 재사용되어야 합니다");
            Assert.IsTrue(go2.activeSelf, "재사용된 GameObject는 활성화되어야 합니다");
            Assert.AreEqual(1, pool.Acquired.Count);
            Assert.AreEqual(0, pool.Released.Count);
        }
        finally
        {
            // Cleanup - 재사용된 GameObject 정리 (go1과 go2는 같은 인스턴스)
            if (go2 != null) GameObject.DestroyImmediate(go2);
            GameObject.DestroyImmediate(prefab);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 Parent 설정을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_05_Parent_설정_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var parentObject = new GameObject("Parent");
        var pool = new GOPool(provider) { Parent = parentObject.transform };

        GameObject go = null;

        try
        {
            // Act
            go = pool.Acquire();

            yield return null;

            // Assert
            Assert.AreEqual(parentObject.transform, go.transform.parent, "획득한 GameObject는 지정된 Parent의 자식이어야 합니다");
        }
        finally
        {
            // Cleanup - 획득한 GameObject 정리
            if (go != null) GameObject.DestroyImmediate(go);
            GameObject.DestroyImmediate(prefab);
            GameObject.DestroyImmediate(parentObject);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 ReleaseAll 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_06_ReleaseAll_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var poolParent = new GameObject("PoolParent");
        var pool = new GOPool(provider) { Pool = poolParent.transform };

        GameObject go1 = null;
        GameObject go2 = null;
        GameObject go3 = null;

        try
        {
            go1 = pool.Acquire();
            go2 = pool.Acquire();
            go3 = pool.Acquire();

            yield return null;

            // Act
            pool.ReleaseAll();

            yield return null;

            // Assert
            Assert.AreEqual(0, pool.Acquired.Count);
            Assert.AreEqual(3, pool.Released.Count);
            Assert.IsFalse(go1.activeSelf, "모든 GameObject가 비활성화되어야 합니다");
            Assert.IsFalse(go2.activeSelf, "모든 GameObject가 비활성화되어야 합니다");
            Assert.IsFalse(go3.activeSelf, "모든 GameObject가 비활성화되어야 합니다");
        }
        finally
        {
            // Cleanup - 획득한 GameObject들 정리
            if (go1 != null) GameObject.DestroyImmediate(go1);
            if (go2 != null) GameObject.DestroyImmediate(go2);
            if (go3 != null) GameObject.DestroyImmediate(go3);
            GameObject.DestroyImmediate(prefab);
            GameObject.DestroyImmediate(poolParent);
        }
    }

#endregion

#region GOPool Parent 관리 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 WorldPositionStays 설정을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_07_WorldPositionStays_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        prefab.transform.position = new Vector3(10, 20, 30);
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var parentObject = new GameObject("Parent");
        parentObject.transform.position = new Vector3(5, 5, 5);
        
        var pool = new GOPool(provider) 
        { 
            Parent = parentObject.transform,
            WorldPositionStays = true 
        };

        GameObject go = null;

        try
        {
            // Act
            go = pool.Acquire();

            yield return null;

            // Assert - WorldPositionStays = true일 때 월드 위치 유지
            Assert.AreEqual(parentObject.transform, go.transform.parent);
            // 월드 위치는 생성 시 설정한 위치가 유지되어야 함
        }
        finally
        {
            // Cleanup - 획득한 GameObject 정리
            if (go != null) GameObject.DestroyImmediate(go);
            GameObject.DestroyImmediate(prefab);
            GameObject.DestroyImmediate(parentObject);
        }
    }

#endregion

#region GOPool 비동기 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// GOPool의 비동기 획득을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOPool_08_AcquireAsync_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOPool(provider);

        GameObject go = null;
        bool completed = false;

        // Act
        var task = TestAcquireAsync(pool);
        
        async Awaitable TestAcquireAsync(GOPool p)
        {
            go = await p.AcquireAsync();
            completed = true;
        }

        // Wait for completion
        yield return new WaitUntil(() => completed);

        // Assert
        Assert.IsNotNull(go);
        Assert.IsTrue(go.activeSelf);
        Assert.AreEqual(1, pool.Acquired.Count);

        // Cleanup - 획득한 GameObject 정리
        if (go != null) GameObject.DestroyImmediate(go);
        GameObject.DestroyImmediate(prefab);
    }

#endregion

}

