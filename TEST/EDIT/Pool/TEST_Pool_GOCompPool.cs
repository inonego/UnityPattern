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
/// GOCompPool 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_Pool_GOCompPool
{

#region GOCompPool 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_01_기본_생성_테스트()
    {
        // Arrange & Act
        var pool = new GOCompPool<TestComponent>();

        // Assert
        Assert.IsNotNull(pool);
        Assert.IsNotNull(pool.GameObjectProvider);
        Assert.AreEqual(0, pool.Released.Count);
        Assert.AreEqual(0, pool.Acquired.Count);
        Assert.AreEqual(0, pool.ReleasedComp.Count);
        Assert.AreEqual(0, pool.AcquiredComp.Count);

        yield return null;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 Component 획득을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_02_AcquireComp_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        var testComp = prefab.AddComponent<TestComponent>();
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOCompPool<TestComponent>(provider);

        TestComponent comp1 = null;
        TestComponent comp2 = null;

        try
        {
            // Act
            comp1 = pool.AcquireComp();
            comp2 = pool.AcquireComp();

            yield return null;

            // Assert
            Assert.IsNotNull(comp1);
            Assert.IsNotNull(comp2);
            Assert.AreNotSame(comp1, comp2);
            Assert.AreEqual(2, pool.AcquiredComp.Count);
            Assert.AreEqual(0, pool.ReleasedComp.Count);
            Assert.IsTrue(comp1.gameObject.activeSelf, "획득한 Component의 GameObject는 활성화되어야 합니다");
            Assert.IsTrue(comp2.gameObject.activeSelf, "획득한 Component의 GameObject는 활성화되어야 합니다");
        }
        finally
        {
            // Cleanup - 획득한 GameObject들 정리
            if (comp1 != null) GameObject.DestroyImmediate(comp1.gameObject);
            if (comp2 != null) GameObject.DestroyImmediate(comp2.gameObject);
            GameObject.DestroyImmediate(prefab);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 Component 반환을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_03_ReleaseComp_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        prefab.AddComponent<TestComponent>();
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var poolParent = new GameObject("PoolParent");
        var pool = new GOCompPool<TestComponent>(provider) 
        { 
            Pool = poolParent.transform 
        };

        TestComponent comp = null;

        try
        {
            comp = pool.AcquireComp();

            yield return null;

            // Act
            pool.ReleaseComp(comp);

            yield return null;

            // Assert
            Assert.AreEqual(0, pool.AcquiredComp.Count);
            Assert.AreEqual(1, pool.ReleasedComp.Count);
            Assert.IsFalse(comp.gameObject.activeSelf, "반환된 Component의 GameObject는 비활성화되어야 합니다");
            Assert.AreEqual(poolParent.transform, comp.transform.parent, "반환된 Component는 Pool 부모로 이동해야 합니다");
        }
        finally
        {
            // Cleanup - 획득한 GameObject 정리
            if (comp != null) GameObject.DestroyImmediate(comp.gameObject);
            GameObject.DestroyImmediate(prefab);
            GameObject.DestroyImmediate(poolParent);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 Component 재사용을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_04_재사용_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        prefab.AddComponent<TestComponent>();
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOCompPool<TestComponent>(provider);

        TestComponent comp1 = null;
        TestComponent comp2 = null;

        try
        {
            comp1 = pool.AcquireComp();
            comp1.Value = 42;
            var instanceId = comp1.GetInstanceID();

            yield return null;

            pool.ReleaseComp(comp1);

            yield return null;

            // Act
            comp2 = pool.AcquireComp();

            yield return null;

            // Assert
            Assert.AreEqual(instanceId, comp2.GetInstanceID(), "같은 Component가 재사용되어야 합니다");
            Assert.AreEqual(42, comp2.Value, "재사용 시 Component의 데이터가 유지되어야 합니다");
            Assert.IsTrue(comp2.gameObject.activeSelf, "재사용된 Component의 GameObject는 활성화되어야 합니다");
            Assert.AreEqual(1, pool.AcquiredComp.Count);
            Assert.AreEqual(0, pool.ReleasedComp.Count);
        }
        finally
        {
            // Cleanup - 재사용된 GameObject 정리 (comp1과 comp2는 같은 인스턴스)
            if (comp2 != null) GameObject.DestroyImmediate(comp2.gameObject);
            GameObject.DestroyImmediate(prefab);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 IPool 인터페이스 구현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_05_IPool_인터페이스_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        prefab.AddComponent<TestComponent>();
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        IPool<TestComponent> pool = new GOCompPool<TestComponent>(provider);

        TestComponent comp1 = null;
        TestComponent comp2 = null;

        try
        {
            // Act
            comp1 = pool.Acquire();
            comp2 = pool.Acquire();

            yield return null;

            // Assert
            Assert.IsNotNull(comp1);
            Assert.IsNotNull(comp2);
            Assert.AreEqual(2, pool.Acquired.Count);
            Assert.AreEqual(0, pool.Released.Count);

            // Act
            pool.Release(comp1);

            yield return null;

            // Assert
            Assert.AreEqual(1, pool.Acquired.Count);
            Assert.AreEqual(1, pool.Released.Count);
        }
        finally
        {
            // Cleanup - 획득한 GameObject들 정리
            if (comp1 != null) GameObject.DestroyImmediate(comp1.gameObject);
            if (comp2 != null) GameObject.DestroyImmediate(comp2.gameObject);
            GameObject.DestroyImmediate(prefab);
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 다중 Component 관리를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_06_다중_Component_관리_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        prefab.AddComponent<TestComponent>();
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOCompPool<TestComponent>(provider);

        var components = new List<TestComponent>();

        try
        {
            // Act - 5개 획득
            for (int i = 0; i < 5; i++)
            {
                var comp = pool.AcquireComp();
                comp.Value = i * 10;
                components.Add(comp);
            }

            yield return null;

            // Assert
            Assert.AreEqual(5, pool.AcquiredComp.Count);
            Assert.AreEqual(0, pool.ReleasedComp.Count);

            // Act - 3개 반환
            for (int i = 0; i < 3; i++)
            {
                pool.ReleaseComp(components[i]);
            }

            yield return null;

            // Assert
            Assert.AreEqual(2, pool.AcquiredComp.Count);
            Assert.AreEqual(3, pool.ReleasedComp.Count);

            // Act - 다시 2개 획득 (재사용)
            var reused1 = pool.AcquireComp();
            var reused2 = pool.AcquireComp();

            yield return null;

            // Assert
            Assert.AreEqual(4, pool.AcquiredComp.Count);
            Assert.AreEqual(1, pool.ReleasedComp.Count);
            Assert.Contains(reused1, components, "재사용된 Component는 이전에 사용되던 Component여야 합니다");
            Assert.Contains(reused2, components, "재사용된 Component는 이전에 사용되던 Component여야 합니다");
        }
        finally
        {
            // Cleanup - 획득한 GameObject들 정리
            foreach (var comp in components)
            {
                if (comp != null) GameObject.DestroyImmediate(comp.gameObject);
            }
            GameObject.DestroyImmediate(prefab);
        }
    }

#endregion

#region GOCompPool 예외 처리 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 예외 상황들을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_07_예외_처리_통합_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        prefab.AddComponent<TestComponent>();
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOCompPool<TestComponent>(provider);

        try
        {
            // null Component 반환 시도
            Assert.Throws<Exception>(() => pool.ReleaseComp(null));

            yield return null;
        }
        finally
        {
            // Cleanup
            GameObject.DestroyImmediate(prefab);
        }
    }

#endregion

#region GOCompPool 비동기 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// GOCompPool의 비동기 Component 획득을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [UnityTest]
    public IEnumerator GOCompPool_08_AcquireCompAsync_테스트()
    {
        // Arrange
        var prefab = new GameObject("TestPrefab");
        prefab.AddComponent<TestComponent>();
        var provider = new PrefabGameObjectProvider { Prefab = prefab };
        var pool = new GOCompPool<TestComponent>(provider);

        TestComponent comp = null;
        bool completed = false;

        // Act
        var task = TestAcquireAsync(pool);
        
        async Awaitable TestAcquireAsync(GOCompPool<TestComponent> p)
        {
            comp = await p.AcquireCompAsync();
            completed = true;
        }

        // Wait for completion
        yield return new WaitUntil(() => completed);

        // Assert
        Assert.IsNotNull(comp);
        Assert.IsTrue(comp.gameObject.activeSelf);
        Assert.AreEqual(1, pool.AcquiredComp.Count);

        // Cleanup - 획득한 GameObject 정리
        if (comp != null) GameObject.DestroyImmediate(comp.gameObject);
        GameObject.DestroyImmediate(prefab);
    }

#endregion

#region 테스트용 Component 클래스

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 Component 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestComponent : MonoBehaviour
    {
        public int Value { get; set; }
    }

#endregion

}

