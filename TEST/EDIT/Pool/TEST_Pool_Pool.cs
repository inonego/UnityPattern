using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using UnityEngine;

using inonego.Pool;

// ============================================================================
/// <summary>
/// Pool 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_Pool_Pool
{

#region Pool 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 기본 생성 및 new() 제약을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Pool_01_기본_생성_테스트()
    {
        // Arrange & Act
        var pool = new TestSimplePool();

        // Assert
        Assert.AreEqual(0, pool.Released.Count);
        Assert.AreEqual(0, pool.Acquired.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 자동 객체 생성을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Pool_02_자동_생성_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var pool = new TestSimplePool();

        // ------------------------------------------------------------
        // 자동 객체 생성
        // ------------------------------------------------------------
        var item1 = pool.Acquire();
        var item2 = pool.Acquire();

        Assert.IsNotNull(item1);
        Assert.IsNotNull(item2);
        Assert.AreNotSame(item1, item2);
        Assert.AreEqual(2, pool.Acquired.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 재사용 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Pool_03_재사용_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var pool = new TestSimplePool();
        var item1 = pool.Acquire();
        item1.Value = 42;
        pool.Release(item1);

        // ------------------------------------------------------------
        // 재사용
        // ------------------------------------------------------------
        var item2 = pool.Acquire();

        Assert.AreSame(item1, item2, "Released된 객체가 재사용되어야 합니다");
        Assert.AreEqual(42, item2.Value, "재사용된 객체의 데이터가 유지되어야 합니다");
        Assert.AreEqual(1, pool.Acquired.Count);
        Assert.AreEqual(0, pool.Released.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 다중 객체 관리를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Pool_04_다중_객체_관리_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var pool = new TestSimplePool();
        var items = new List<TestPoolItem>();

        // ------------------------------------------------------------
        // 10개 획득
        // ------------------------------------------------------------
        for (int i = 0; i < 10; i++)
        {
            var item = pool.Acquire();
            item.Value = i;
            items.Add(item);
        }

        Assert.AreEqual(10, pool.Acquired.Count);
        Assert.AreEqual(0, pool.Released.Count);

        // ------------------------------------------------------------
        // 5개 반환
        // ------------------------------------------------------------
        for (int i = 0; i < 5; i++)
        {
            pool.Release(items[i]);
        }

        Assert.AreEqual(5, pool.Acquired.Count);
        Assert.AreEqual(5, pool.Released.Count);

        // ------------------------------------------------------------
        // 다시 3개 획득 (재사용)
        // ------------------------------------------------------------
        var reused1 = pool.Acquire();
        var reused2 = pool.Acquire();
        var reused3 = pool.Acquire();

        Assert.AreEqual(8, pool.Acquired.Count);
        Assert.AreEqual(2, pool.Released.Count);
        Assert.Contains(reused1, items, "재사용된 객체는 이전에 사용되던 객체여야 합니다");
        Assert.Contains(reused2, items, "재사용된 객체는 이전에 사용되던 객체여야 합니다");
        Assert.Contains(reused3, items, "재사용된 객체는 이전에 사용되던 객체여야 합니다");
    }

#endregion

#region Pool 사용자 정의 타입 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Pool의 복잡한 타입 지원을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Pool_05_복잡한_타입_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var pool = new TestComplexPool();

        // ------------------------------------------------------------
        // 복잡한 타입 객체 생성
        // ------------------------------------------------------------
        var item1 = pool.Acquire();
        item1.Name = "Item1";
        item1.Data.Add("Key1", 100);

        var item2 = pool.Acquire();
        item2.Name = "Item2";
        item2.Data.Add("Key2", 200);

        Assert.AreEqual("Item1", item1.Name);
        Assert.AreEqual("Item2", item2.Name);
        Assert.AreEqual(100, item1.Data["Key1"]);
        Assert.AreEqual(200, item2.Data["Key2"]);

        // ------------------------------------------------------------
        // Release and Reuse
        // ------------------------------------------------------------
        pool.Release(item1);
        var item3 = pool.Acquire();

        Assert.AreSame(item1, item3, "복잡한 타입도 재사용되어야 합니다");
        Assert.AreEqual("Item1", item3.Name, "재사용 시 데이터가 유지되어야 합니다");
    }

#endregion

#region 테스트용 Pool 클래스

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 간단한 Pool 아이템 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestPoolItem
    {
        public int Value { get; set; }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 간단한 Pool 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestSimplePool : Pool<TestPoolItem>
    {
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 복잡한 Pool 아이템 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestComplexItem
    {
        public string Name { get; set; }
        public Dictionary<string, int> Data { get; set; }

        public TestComplexItem()
        {
            Data = new Dictionary<string, int>();
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 복잡한 Pool 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class TestComplexPool : Pool<TestComplexItem>
    {
    }

#endregion

}

