using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using inonego.Pool;

// ============================================================================
/// <summary>
/// PoolBase 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_Pool_PoolBase
{

#region PoolBase 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void PoolBase_01_기본_생성_테스트()
    {
        // Arrange & Act
        var pool = new TestPool();

        // Assert
        Assert.AreEqual(0, pool.Released.Count);
        Assert.AreEqual(0, pool.Acquired.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 Acquire 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void PoolBase_02_Acquire_테스트()
    {
        // Arrange
        var pool = new TestPool();

        // Act
        var item1 = pool.Acquire();
        var item2 = pool.Acquire();

        // Assert
        Assert.IsNotNull(item1);
        Assert.IsNotNull(item2);
        Assert.AreNotSame(item1, item2);
        Assert.AreEqual(2, pool.Acquired.Count);
        Assert.AreEqual(0, pool.Released.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 Release 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void PoolBase_03_Release_테스트()
    {
        // Arrange
        var pool = new TestPool();
        var item1 = pool.Acquire();
        var item2 = pool.Acquire();

        // Act
        pool.Release(item1);

        // Assert
        Assert.AreEqual(1, pool.Acquired.Count);
        Assert.AreEqual(1, pool.Released.Count);

        // Act
        pool.Release(item2);

        // Assert
        Assert.AreEqual(0, pool.Acquired.Count);
        Assert.AreEqual(2, pool.Released.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 재사용 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void PoolBase_04_재사용_테스트()
    {
        // Arrange
        var pool = new TestPool();
        var item1 = pool.Acquire();
        pool.Release(item1);

        // Act
        var item2 = pool.Acquire();

        // Assert
        Assert.AreSame(item1, item2, "Released된 오브젝트가 재사용되어야 합니다");
        Assert.AreEqual(1, pool.Acquired.Count);
        Assert.AreEqual(0, pool.Released.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 ReleaseAll 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void PoolBase_05_ReleaseAll_테스트()
    {
        // Arrange
        var pool = new TestPool();
        var item1 = pool.Acquire();
        var item2 = pool.Acquire();
        var item3 = pool.Acquire();

        // Act
        pool.ReleaseAll();

        // Assert
        Assert.AreEqual(0, pool.Acquired.Count);
        Assert.AreEqual(3, pool.Released.Count);
    }

#endregion

#region PoolBase 예외 처리 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 예외 상황들을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void PoolBase_06_예외_처리_통합_테스트()
    {
        var pool = new TestPool();

        // 풀에 없는 아이템 Release 시도
        var nonPooledItem = new TestPoolItem();
        Assert.Throws<Exception>(() => pool.Release(nonPooledItem));

        // 이미 Released된 아이템 다시 Release 시도
        var item = pool.Acquire();
        pool.Release(item);
        Assert.Throws<Exception>(() => pool.Release(item));
    }

#endregion

#region PoolBase 콜백 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 OnAcquire/OnRelease 콜백을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void PoolBase_07_콜백_테스트()
    {
        // Arrange
        var pool = new TestPoolWithCallbacks();
        var item = pool.Acquire();

        // Assert
        Assert.AreEqual(1, pool.AcquireCallCount);
        Assert.AreEqual(0, pool.ReleaseCallCount);

        // Act
        pool.Release(item);

        // Assert
        Assert.AreEqual(1, pool.AcquireCallCount);
        Assert.AreEqual(1, pool.ReleaseCallCount);

        // Act - 재사용
        var item2 = pool.Acquire();

        // Assert
        Assert.AreEqual(2, pool.AcquireCallCount);
        Assert.AreEqual(1, pool.ReleaseCallCount);
        Assert.AreSame(item, item2);
    }

#endregion

#region 테스트용 Pool 클래스

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 Pool 아이템 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestPoolItem
    {
        public int Value { get; set; }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 Pool 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestPool : PoolBase<TestPoolItem>
    {
        protected override TestPoolItem AcquireNew()
        {
            return new TestPoolItem();
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 콜백 테스트용 Pool 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestPoolWithCallbacks : PoolBase<TestPoolItem>
    {
        public int AcquireCallCount { get; private set; }
        public int ReleaseCallCount { get; private set; }

        protected override TestPoolItem AcquireNew()
        {
            return new TestPoolItem();
        }

        protected override void OnAcquire(TestPoolItem item)
        {
            AcquireCallCount++;
            item.Value = 100;
        }

        protected override void OnRelease(TestPoolItem item)
        {
            ReleaseCallCount++;
            item.Value = 0;
        }
    }

#endregion

}

