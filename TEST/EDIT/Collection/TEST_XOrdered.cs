using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEngine;

using inonego.Serializable;

// ============================================================================
/// <summary>
/// XOrdered 컬렉션의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_XOrdered
{

#region XOrdered 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered의 기본 생성 및 초기 상태를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_01_기본_생성_테스트()
    {
        // Arrange & Act
        var ordered = new XOrdered<TestElement, int>();

        // Assert
        Assert.AreEqual(0, ordered.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered의 모든 기능을 통합 테스트합니다.
    /// Add, Remove, Contains, Clear, 중복 Order, 인덱서 접근
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_02_통합_기능_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var ordered = new XOrdered<TestElement, int>();

        // ------------------------------------------------------------
        // 1. Add - 역순으로 추가하여 오름차순 정렬 확인
        // ------------------------------------------------------------
        ordered.Add(new TestElement("Third"), 30);
        ordered.Add(new TestElement("First"), 10);
        ordered.Add(new TestElement("Second"), 20);
        
        Assert.AreEqual(3, ordered.Count);
        Assert.AreEqual("First", ordered[0].Element.Name, "Order 10이 첫 번째여야 합니다");
        Assert.AreEqual(10, ordered[0].Order, "첫 번째 Order는 10이어야 합니다");
        Assert.AreEqual("Second", ordered[1].Element.Name, "Order 20이 두 번째여야 합니다");
        Assert.AreEqual(20, ordered[1].Order, "두 번째 Order는 20이어야 합니다");
        Assert.AreEqual("Third", ordered[2].Element.Name, "Order 30이 세 번째여야 합니다");
        Assert.AreEqual(30, ordered[2].Order, "세 번째 Order는 30이어야 합니다");

        // ------------------------------------------------------------
        // 2. Contains - 존재 확인
        // ------------------------------------------------------------
        var firstElement = ordered[0].Element;
        Assert.IsTrue(ordered.Contains(firstElement), "추가한 요소는 Contains로 찾을 수 있어야 합니다");
        
        var nonExistent = new TestElement("NonExistent");
        Assert.IsFalse(ordered.Contains(nonExistent), "존재하지 않는 요소는 false를 반환해야 합니다");

        // ------------------------------------------------------------
        // 3. 중복 Order 처리 테스트
        // ------------------------------------------------------------
        ordered.Add(new TestElement("First-Dup"), 10);
        
        Assert.AreEqual(4, ordered.Count);
        // Upper Bound: 같은 Order는 추가된 순서대로 유지
        // 나중에 추가된 "First-Dup"이 "First" 뒤에 위치
        Assert.AreEqual("First", ordered[0].Element.Name);
        Assert.AreEqual("First-Dup", ordered[1].Element.Name);
        Assert.AreEqual("Second", ordered[2].Element.Name);
        Assert.AreEqual("Third", ordered[3].Element.Name);

        // ------------------------------------------------------------
        // 4. Remove - 요소 제거
        // ------------------------------------------------------------
        var secondElement = ordered[2].Element; // "Second"
        bool removed = ordered.Remove(secondElement);
        
        Assert.IsTrue(removed, "존재하는 Element 제거는 true를 반환해야 합니다");
        Assert.AreEqual(3, ordered.Count);
        Assert.AreEqual("First", ordered[0].Element.Name);
        Assert.AreEqual("First-Dup", ordered[1].Element.Name);
        Assert.AreEqual("Third", ordered[2].Element.Name);

        // 존재하지 않는 요소 제거 시도
        removed = ordered.Remove(secondElement);
        Assert.IsFalse(removed, "이미 제거된 Element는 false를 반환해야 합니다");

        // ------------------------------------------------------------
        // 5. Clear - 모든 요소 제거
        // ------------------------------------------------------------
        ordered.Clear();
        Assert.AreEqual(0, ordered.Count, "Clear 후 Count는 0이어야 합니다");

        // ------------------------------------------------------------
        // 6. Clear 후 재사용
        // ------------------------------------------------------------
        ordered.Add(new TestElement("A"), 1);
        ordered.Add(new TestElement("B"), 2);
        
        Assert.AreEqual(2, ordered.Count);
        Assert.AreEqual("A", ordered[0].Element.Name);
        Assert.AreEqual("B", ordered[1].Element.Name);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered를 대량 데이터로 스트레스 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_03_대량_데이터_스트레스_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비 - 100개 요소
        // ------------------------------------------------------------
        var ordered = new XOrdered<TestElement, int>();
        const int lTestCount = 100;

        // ------------------------------------------------------------
        // 1. 역순으로 Add (높은 Order부터 낮은 순으로)
        // ------------------------------------------------------------
        for (int i = lTestCount; i > 0; i--)
        {
            ordered.Add(new TestElement($"Element-{i}"), i);
        }
        
        Assert.AreEqual(lTestCount, ordered.Count);

        // ------------------------------------------------------------
        // 2. 인덱서 접근 - 오름차순으로 정렬되어 있어야 함 (1 -> 2 -> ... -> 100)
        // ------------------------------------------------------------
        for (int i = 0; i < lTestCount; i++)
        {
            int expectedOrder = i + 1;
            Assert.AreEqual($"Element-{expectedOrder}", ordered[i].Element.Name, 
                $"인덱스 {i}에서 Element-{expectedOrder}가 있어야 합니다");
            Assert.AreEqual(expectedOrder, ordered[i].Order, 
                $"인덱스 {i}의 Order는 {expectedOrder}여야 합니다");
        }

        // ------------------------------------------------------------
        // 3. 랜덤 순서로 Add 후 정렬 확인
        // ------------------------------------------------------------
        ordered.Clear();
        
        var random = new System.Random(42); // 고정 시드로 재현 가능
        var addedOrders = new List<int>();

        for (int i = 0; i < lTestCount; i++)
        {
            int order = random.Next(1, 1000);
            addedOrders.Add(order);
            ordered.Add(new TestElement($"Random-{i}"), order);
        }

        // 예상 순서: 오름차순 정렬
        addedOrders.Sort();

        // 인덱서로 순서 확인 - 이제 Order를 직접 확인 가능
        for (int i = 0; i < lTestCount; i++)
        {
            Assert.IsNotNull(ordered[i].Element);
            Assert.AreEqual(addedOrders[i], ordered[i].Order, 
                $"인덱스 {i}의 Order가 오름차순으로 정렬되어야 합니다");
        }

        Assert.AreEqual(lTestCount, ordered.Count);

        // ------------------------------------------------------------
        // 4. Add/Remove 혼합 테스트
        // ------------------------------------------------------------
        ordered.Clear();
        
        var elements = new List<TestElement>();
        for (int i = 0; i < 50; i++)
        {
            var element = new TestElement($"Mix-{i}");
            elements.Add(element);
            ordered.Add(element, i * 2);
        }
        
        Assert.AreEqual(50, ordered.Count);

        // 중간 요소들 제거 (짝수 인덱스)
        for (int i = 0; i < 25; i++)
        {
            ordered.Remove(elements[i * 2]);
        }
        
        Assert.AreEqual(25, ordered.Count);

        // 추가 Add
        for (int i = 50; i < 100; i++)
        {
            ordered.Add(new TestElement($"Mix-{i}"), i * 2);
        }
        
        Assert.AreEqual(75, ordered.Count);

        // 전체 순회 - 오름차순 확인
        int count = 0;
        int prevOrder = -1;
        foreach (var (element, order) in ordered)
        {
            Assert.IsNotNull(element);
            Assert.GreaterOrEqual(order, prevOrder, "Order는 오름차순이어야 합니다");
            prevOrder = order;
            count++;
        }
        Assert.AreEqual(75, count);

        // ------------------------------------------------------------
        // 5. Contains 성능 테스트 (선형 탐색)
        // ------------------------------------------------------------
        ordered.Clear();
        
        for (int i = 0; i < 50; i++)
        {
            ordered.Add(new TestElement($"Perf-{i}"), i);
        }

        // 존재하는 요소 찾기
        var searchElement = new TestElement("Perf-25");
        Assert.IsTrue(ordered.Contains(searchElement));

        // 존재하지 않는 요소
        var notFound = new TestElement("NotExist");
        Assert.IsFalse(ordered.Contains(notFound));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered의 IReadOnlyList 인터페이스 구현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_04_IReadOnlyList_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var ordered = new XOrdered<TestElement, int>();
        ordered.Add(new TestElement("First"), 10);
        ordered.Add(new TestElement("Second"), 20);
        ordered.Add(new TestElement("Third"), 30);

        // ------------------------------------------------------------
        // IReadOnlyList로 캐스팅 - 튜플 반환
        // ------------------------------------------------------------
        IReadOnlyList<(TestElement Element, int Order)> readOnlyList = ordered;
        
        Assert.AreEqual(3, readOnlyList.Count);
        Assert.AreEqual("First", readOnlyList[0].Element.Name);
        Assert.AreEqual(10, readOnlyList[0].Order);
        Assert.AreEqual("Second", readOnlyList[1].Element.Name);
        Assert.AreEqual(20, readOnlyList[1].Order);
        Assert.AreEqual("Third", readOnlyList[2].Element.Name);
        Assert.AreEqual(30, readOnlyList[2].Order);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered의 IEnumerable 구현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_05_IEnumerable_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var ordered = new XOrdered<TestElement, int>();
        ordered.Add(new TestElement("First"), 10);
        ordered.Add(new TestElement("Second"), 20);
        ordered.Add(new TestElement("Third"), 30);

        // ------------------------------------------------------------
        // foreach 순회 - 튜플 분해
        // ------------------------------------------------------------
        var names = new List<string>();
        var orders = new List<int>();
        foreach (var (element, order) in ordered)
        {
            names.Add(element.Name);
            orders.Add(order);
        }
        
        Assert.AreEqual(3, names.Count);
        Assert.AreEqual("First", names[0]);
        Assert.AreEqual("Second", names[1]);
        Assert.AreEqual("Third", names[2]);
        Assert.AreEqual(10, orders[0]);
        Assert.AreEqual(20, orders[1]);
        Assert.AreEqual(30, orders[2]);

        // ------------------------------------------------------------
        // LINQ 사용
        // ------------------------------------------------------------
        var count = ordered.Count();
        var firstItem = ordered.First();
        
        Assert.AreEqual(3, count);
        Assert.AreEqual("First", firstItem.Element.Name);
        Assert.AreEqual(10, firstItem.Order);
    }

#endregion

#region XOrdered 직렬화 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_06_JSON_직렬화_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var original = new XOrdered<TestElement, int>();
        original.Add(new TestElement("Third"), 30);
        original.Add(new TestElement("First"), 10);
        original.Add(new TestElement("Second"), 20);

        // ------------------------------------------------------------
        // JSON 직렬화/역직렬화 - 상태 복원 확인
        // ------------------------------------------------------------
        string json = JsonUtility.ToJson(original);
        var deserialized = JsonUtility.FromJson<XOrdered<TestElement, int>>(json);
        
        Assert.AreEqual(original.Count, deserialized.Count);
        Assert.AreEqual(original[0].Element.Name, deserialized[0].Element.Name);
        Assert.AreEqual(original[0].Order, deserialized[0].Order);
        Assert.AreEqual(original[1].Element.Name, deserialized[1].Element.Name);
        Assert.AreEqual(original[1].Order, deserialized[1].Order);
        Assert.AreEqual(original[2].Element.Name, deserialized[2].Element.Name);
        Assert.AreEqual(original[2].Order, deserialized[2].Order);

        // ------------------------------------------------------------
        // 역직렬화 후 Add 동작 확인
        // ------------------------------------------------------------
        deserialized.Add(new TestElement("Zero"), 0);
        
        Assert.AreEqual(4, deserialized.Count);
        Assert.AreEqual("Zero", deserialized[0].Element.Name, "Order 0이 맨 앞에 추가되어야 합니다");
        Assert.AreEqual(0, deserialized[0].Order);
    }

#endregion

#region Helper Classes

    // ------------------------------------------------------------
    /// <summary>
    /// 테스트용 Element 클래스
    /// </summary>
    // ------------------------------------------------------------
    [Serializable]
    private class TestElement : IEquatable<TestElement>
    {
        [SerializeField]
        public string Name;

        public TestElement() {}

        public TestElement(string name)
        {
            Name = name;
        }

        public bool Equals(TestElement other)
        {
            if (other == null) return false;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is TestElement other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }

#endregion

}

