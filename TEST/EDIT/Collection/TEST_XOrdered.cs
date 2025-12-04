using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using UnityEngine;

using inonego;
using inonego.Serializable;

// ============================================================================
/// <summary>
/// XOrdered 컬렉션의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_XOrdered
{

#region XOrdered<TElement, TOrder> 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TElement, TOrder>의 기본 생성 및 초기 상태를 테스트합니다.
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
    /// XOrdered<TElement, TOrder>의 모든 기능을 통합 테스트합니다.
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
    /// XOrdered<TElement, TOrder>를 대량 데이터로 스트레스 테스트합니다.
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
    /// XOrdered<TElement, TOrder>의 IReadOnlyList 인터페이스 구현을 테스트합니다.
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
    /// XOrdered<TElement, TOrder>의 IEnumerable 구현을 테스트합니다.
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
        IReadOnlyList<(TestElement Element, int Order)> readOnlyList = ordered;
        var count = readOnlyList.Count();
        var firstItem = readOnlyList.First();
        
        Assert.AreEqual(3, count);
        Assert.AreEqual("First", firstItem.Element.Name);
        Assert.AreEqual(10, firstItem.Order);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TElement, TOrder> 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
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

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TElement, TOrder> 클래스의 복제 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_07_복제_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var original = new XOrdered<TestElement, int>();
        original.Add(new TestElement("First"), 10);
        original.Add(new TestElement("Second"), 20);
        original.Add(new TestElement("Third"), 30);

        // ------------------------------------------------------------
        // Clone 테스트
        // ------------------------------------------------------------
        var cloned = original.Clone();
        
        Assert.AreEqual(original.Count, cloned.Count);
        Assert.AreEqual(original[0].Element.Name, cloned[0].Element.Name);
        Assert.AreEqual(original[0].Order, cloned[0].Order);
        Assert.AreEqual(original[1].Element.Name, cloned[1].Element.Name);
        Assert.AreEqual(original[1].Order, cloned[1].Order);
        Assert.AreEqual(original[2].Element.Name, cloned[2].Element.Name);
        Assert.AreEqual(original[2].Order, cloned[2].Order);

        // ------------------------------------------------------------
        // 복제본 수정이 원본에 영향을 주지 않는지 확인
        // ------------------------------------------------------------
        cloned.Add(new TestElement("Fourth"), 40);
        
        Assert.AreEqual(4, cloned.Count);
        Assert.AreEqual(3, original.Count, "복제본 수정이 원본에 영향을 주지 않아야 합니다");
    }

#endregion

#region XOrdered<TKey, TElement, TOrder> 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TKey, TElement, TOrder>의 기본 생성 및 초기 상태를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_01_기본_생성_테스트()
    {
        // Arrange & Act
        var ordered = new XOrdered<string, TestElement, int>();

        // Assert
        Assert.AreEqual(0, ordered.Count);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TKey, TElement, TOrder>의 모든 기능을 통합 테스트합니다.
    /// Add, Remove, ContainsKey, TryGetValue, 인덱서, Clear, 중복 Key 예외 처리
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_02_통합_기능_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var ordered = new XOrdered<string, TestElement, int>();

        // ------------------------------------------------------------
        // 1. Add - Key와 함께 추가, 역순으로 추가하여 오름차순 정렬 확인
        // ------------------------------------------------------------
        ordered.Add("key3", new TestElement("Third"), 30);
        ordered.Add("key1", new TestElement("First"), 10);
        ordered.Add("key2", new TestElement("Second"), 20);
        
        Assert.AreEqual(3, ordered.Count);
        Assert.AreEqual("First", ordered[0].Element.Name, "Order 10이 첫 번째여야 합니다");
        Assert.AreEqual(10, ordered[0].Order, "첫 번째 Order는 10이어야 합니다");
        Assert.AreEqual("Second", ordered[1].Element.Name, "Order 20이 두 번째여야 합니다");
        Assert.AreEqual(20, ordered[1].Order, "두 번째 Order는 20이어야 합니다");
        Assert.AreEqual("Third", ordered[2].Element.Name, "Order 30이 세 번째여야 합니다");
        Assert.AreEqual(30, ordered[2].Order, "세 번째 Order는 30이어야 합니다");

        // ------------------------------------------------------------
        // 2. Key로 Element 조회 - 인덱서
        // ------------------------------------------------------------
        Assert.AreEqual("First", ordered["key1"].Name, "key1로 First를 찾을 수 있어야 합니다");
        Assert.AreEqual("Second", ordered["key2"].Name, "key2로 Second를 찾을 수 있어야 합니다");
        Assert.AreEqual("Third", ordered["key3"].Name, "key3로 Third를 찾을 수 있어야 합니다");
        Assert.IsNull(ordered["nonexistent"], "존재하지 않는 Key는 null을 반환해야 합니다");

        // ------------------------------------------------------------
        // 3. TryGetValue - Key로 Element 조회
        // ------------------------------------------------------------
        Assert.IsTrue(ordered.TryGetValue("key1", out var element1), "존재하는 Key는 true를 반환해야 합니다");
        Assert.AreEqual("First", element1.Name);
        
        Assert.IsFalse(ordered.TryGetValue("nonexistent", out var element2), "존재하지 않는 Key는 false를 반환해야 합니다");
        Assert.IsNull(element2);

        // ------------------------------------------------------------
        // 4. ContainsKey - Key 존재 확인
        // ------------------------------------------------------------
        Assert.IsTrue(ordered.ContainsKey("key1"), "존재하는 Key는 true를 반환해야 합니다");
        Assert.IsTrue(ordered.ContainsKey("key2"), "존재하는 Key는 true를 반환해야 합니다");
        Assert.IsTrue(ordered.ContainsKey("key3"), "존재하는 Key는 true를 반환해야 합니다");
        Assert.IsFalse(ordered.ContainsKey("nonexistent"), "존재하지 않는 Key는 false를 반환해야 합니다");

        // ------------------------------------------------------------
        // 5. 중복 Key 추가 시 예외 처리
        // ------------------------------------------------------------
        Assert.Throws<ArgumentException>(() => 
        {
            ordered.Add("key1", new TestElement("Duplicate"), 50);
        }, "중복된 Key 추가 시 ArgumentException이 발생해야 합니다");

        Assert.AreEqual(3, ordered.Count, "중복 Key 추가 실패 후 Count는 변경되지 않아야 합니다");

        // ------------------------------------------------------------
        // 6. 중복 Order 처리 테스트
        // ------------------------------------------------------------
        ordered.Add("key4", new TestElement("First-Dup"), 10);
        
        Assert.AreEqual(4, ordered.Count);
        // Upper Bound: 같은 Order는 추가된 순서대로 유지
        // 나중에 추가된 "First-Dup"이 "First" 뒤에 위치
        Assert.AreEqual("First", ordered[0].Element.Name);
        Assert.AreEqual("First-Dup", ordered[1].Element.Name);
        Assert.AreEqual("Second", ordered[2].Element.Name);
        Assert.AreEqual("Third", ordered[3].Element.Name);

        // ------------------------------------------------------------
        // 7. Remove - Key로 요소 제거
        // ------------------------------------------------------------
        bool removed = ordered.Remove("key2");
        
        Assert.IsTrue(removed, "존재하는 Key 제거는 true를 반환해야 합니다");
        Assert.AreEqual(3, ordered.Count);
        Assert.AreEqual("First", ordered[0].Element.Name);
        Assert.AreEqual("First-Dup", ordered[1].Element.Name);
        Assert.AreEqual("Third", ordered[2].Element.Name);
        Assert.IsFalse(ordered.ContainsKey("key2"), "제거된 Key는 ContainsKey에서 false를 반환해야 합니다");

        // 존재하지 않는 Key 제거 시도
        removed = ordered.Remove("nonexistent");
        Assert.IsFalse(removed, "존재하지 않는 Key 제거는 false를 반환해야 합니다");

        // ------------------------------------------------------------
        // 8. Clear - 모든 요소 제거
        // ------------------------------------------------------------
        ordered.Clear();
        Assert.AreEqual(0, ordered.Count, "Clear 후 Count는 0이어야 합니다");
        Assert.IsFalse(ordered.ContainsKey("key1"), "Clear 후 모든 Key가 제거되어야 합니다");
        Assert.IsFalse(ordered.ContainsKey("key3"), "Clear 후 모든 Key가 제거되어야 합니다");

        // ------------------------------------------------------------
        // 9. Clear 후 재사용
        // ------------------------------------------------------------
        ordered.Add("keyA", new TestElement("A"), 1);
        ordered.Add("keyB", new TestElement("B"), 2);
        
        Assert.AreEqual(2, ordered.Count);
        Assert.AreEqual("A", ordered[0].Element.Name);
        Assert.AreEqual("B", ordered[1].Element.Name);
        Assert.IsTrue(ordered.ContainsKey("keyA"));
        Assert.IsTrue(ordered.ContainsKey("keyB"));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TKey, TElement, TOrder>를 대량 데이터로 스트레스 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_03_대량_데이터_스트레스_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비 - 100개 요소
        // ------------------------------------------------------------
        var ordered = new XOrdered<string, TestElement, int>();
        const int lTestCount = 100;

        // ------------------------------------------------------------
        // 1. 역순으로 Add (높은 Order부터 낮은 순으로)
        // ------------------------------------------------------------
        for (int i = lTestCount; i > 0; i--)
        {
            ordered.Add($"key{i}", new TestElement($"Element-{i}"), i);
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
        // 3. Key로 조회 성능 테스트
        // ------------------------------------------------------------
        for (int i = 1; i <= lTestCount; i++)
        {
            string key = $"key{i}";
            Assert.IsTrue(ordered.ContainsKey(key), $"Key {key}는 존재해야 합니다");
            Assert.IsTrue(ordered.TryGetValue(key, out var element), $"Key {key}로 조회가 가능해야 합니다");
            Assert.AreEqual($"Element-{i}", element.Name);
            Assert.AreEqual($"Element-{i}", ordered[key].Name);
        }

        // ------------------------------------------------------------
        // 4. 랜덤 순서로 Add 후 정렬 확인
        // ------------------------------------------------------------
        ordered.Clear();
        
        var random = new System.Random(42); // 고정 시드로 재현 가능
        var addedOrders = new List<int>();

        for (int i = 0; i < lTestCount; i++)
        {
            int order = random.Next(1, 1000);
            addedOrders.Add(order);
            ordered.Add($"randomKey{i}", new TestElement($"Random-{i}"), order);
        }

        // 예상 순서: 오름차순 정렬
        addedOrders.Sort();

        // 인덱서로 순서 확인
        for (int i = 0; i < lTestCount; i++)
        {
            Assert.IsNotNull(ordered[i].Element);
            Assert.AreEqual(addedOrders[i], ordered[i].Order, 
                $"인덱스 {i}의 Order가 오름차순으로 정렬되어야 합니다");
        }

        Assert.AreEqual(lTestCount, ordered.Count);

        // ------------------------------------------------------------
        // 5. Add/Remove 혼합 테스트
        // ------------------------------------------------------------
        ordered.Clear();
        
        var keys = new List<string>();
        for (int i = 0; i < 50; i++)
        {
            string key = $"Mix-{i}";
            keys.Add(key);
            ordered.Add(key, new TestElement($"Mix-{i}"), i * 2);
        }
        
        Assert.AreEqual(50, ordered.Count);

        // 중간 요소들 제거 (짝수 인덱스)
        for (int i = 0; i < 25; i++)
        {
            ordered.Remove(keys[i * 2]);
        }
        
        Assert.AreEqual(25, ordered.Count);

        // 추가 Add
        for (int i = 50; i < 100; i++)
        {
            ordered.Add($"Mix-{i}", new TestElement($"Mix-{i}"), i * 2);
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
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TKey, TElement, TOrder>의 IReadOnlyDictionary 인터페이스 구현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_04_IReadOnlyDictionary_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var ordered = new XOrdered<string, TestElement, int>();
        ordered.Add("key1", new TestElement("First"), 10);
        ordered.Add("key2", new TestElement("Second"), 20);
        ordered.Add("key3", new TestElement("Third"), 30);

        // ------------------------------------------------------------
        // IReadOnlyDictionary로 캐스팅
        // ------------------------------------------------------------
        IReadOnlyDictionary<string, TestElement> dictionary = ordered;
        
        Assert.AreEqual(3, dictionary.Count);
        Assert.AreEqual("First", dictionary["key1"].Name);
        Assert.AreEqual("Second", dictionary["key2"].Name);
        Assert.AreEqual("Third", dictionary["key3"].Name);

        // ------------------------------------------------------------
        // Keys 프로퍼티 테스트
        // ------------------------------------------------------------
        var keys = new List<string>(dictionary.Keys);
        Assert.AreEqual(3, keys.Count);
        Assert.Contains("key1", keys);
        Assert.Contains("key2", keys);
        Assert.Contains("key3", keys);

        // ------------------------------------------------------------
        // Values 프로퍼티 테스트
        // ------------------------------------------------------------
        var values = new List<TestElement>(dictionary.Values);
        Assert.AreEqual(3, values.Count);
        var names = values.Select(v => v.Name).ToList();
        Assert.Contains("First", names);
        Assert.Contains("Second", names);
        Assert.Contains("Third", names);

        // ------------------------------------------------------------
        // GetEnumerator 테스트
        // ------------------------------------------------------------
        var pairs = new List<KeyValuePair<string, TestElement>>();
        foreach (var pair in dictionary)
        {
            pairs.Add(pair);
        }
        
        Assert.AreEqual(3, pairs.Count);
        var pairKeys = pairs.Select(p => p.Key).ToList();
        Assert.Contains("key1", pairKeys);
        Assert.Contains("key2", pairKeys);
        Assert.Contains("key3", pairKeys);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TKey, TElement, TOrder>의 IReadOnlyList 인터페이스 구현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_05_IReadOnlyList_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var ordered = new XOrdered<string, TestElement, int>();
        ordered.Add("key1", new TestElement("First"), 10);
        ordered.Add("key2", new TestElement("Second"), 20);
        ordered.Add("key3", new TestElement("Third"), 30);

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
    /// XOrdered<TKey, TElement, TOrder>의 IEnumerable 구현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_06_IEnumerable_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var ordered = new XOrdered<string, TestElement, int>();
        ordered.Add("key1", new TestElement("First"), 10);
        ordered.Add("key2", new TestElement("Second"), 20);
        ordered.Add("key3", new TestElement("Third"), 30);

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
        // LINQ 사용 - IReadOnlyList로 명시적 캐스팅
        // ------------------------------------------------------------
        IReadOnlyList<(TestElement Element, int Order)> readOnlyList = ordered;
        var count = readOnlyList.Count();
        var firstItem = readOnlyList.First();
        
        Assert.AreEqual(3, count);
        Assert.AreEqual("First", firstItem.Element.Name);
        Assert.AreEqual(10, firstItem.Order);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TKey, TElement, TOrder> 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_07_JSON_직렬화_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var original = new XOrdered<string, TestElement, int>();
        original.Add("key3", new TestElement("Third"), 30);
        original.Add("key1", new TestElement("First"), 10);
        original.Add("key2", new TestElement("Second"), 20);

        // ------------------------------------------------------------
        // JSON 직렬화/역직렬화 - 상태 복원 확인
        // ------------------------------------------------------------
        string json = JsonUtility.ToJson(original);
        var deserialized = JsonUtility.FromJson<XOrdered<string, TestElement, int>>(json);
        
        Assert.AreEqual(original.Count, deserialized.Count);
        Assert.AreEqual(original[0].Element.Name, deserialized[0].Element.Name);
        Assert.AreEqual(original[0].Order, deserialized[0].Order);
        Assert.AreEqual(original[1].Element.Name, deserialized[1].Element.Name);
        Assert.AreEqual(original[1].Order, deserialized[1].Order);
        Assert.AreEqual(original[2].Element.Name, deserialized[2].Element.Name);
        Assert.AreEqual(original[2].Order, deserialized[2].Order);

        // ------------------------------------------------------------
        // 역직렬화 후 Key 조회 확인
        // ------------------------------------------------------------
        Assert.IsTrue(deserialized.ContainsKey("key1"), "역직렬화 후 Key 조회가 가능해야 합니다");
        Assert.IsTrue(deserialized.ContainsKey("key2"), "역직렬화 후 Key 조회가 가능해야 합니다");
        Assert.IsTrue(deserialized.ContainsKey("key3"), "역직렬화 후 Key 조회가 가능해야 합니다");
        Assert.AreEqual("First", deserialized["key1"].Name);
        Assert.AreEqual("Second", deserialized["key2"].Name);
        Assert.AreEqual("Third", deserialized["key3"].Name);

        // ------------------------------------------------------------
        // 역직렬화 후 Add 동작 확인
        // ------------------------------------------------------------
        deserialized.Add("key0", new TestElement("Zero"), 0);
        
        Assert.AreEqual(4, deserialized.Count);
        Assert.AreEqual("Zero", deserialized[0].Element.Name, "Order 0이 맨 앞에 추가되어야 합니다");
        Assert.AreEqual(0, deserialized[0].Order);
        Assert.IsTrue(deserialized.ContainsKey("key0"), "추가한 Key가 존재해야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// XOrdered<TKey, TElement, TOrder> 클래스의 복제 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void XOrdered_Key_08_복제_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var original = new XOrdered<string, TestElement, int>();
        original.Add("key1", new TestElement("First"), 10);
        original.Add("key2", new TestElement("Second"), 20);
        original.Add("key3", new TestElement("Third"), 30);

        // ------------------------------------------------------------
        // Clone 테스트
        // ------------------------------------------------------------
        var cloned = original.Clone();
        
        Assert.AreEqual(original.Count, cloned.Count);
        Assert.AreEqual(original[0].Element.Name, cloned[0].Element.Name);
        Assert.AreEqual(original[0].Order, cloned[0].Order);
        Assert.AreEqual(original[1].Element.Name, cloned[1].Element.Name);
        Assert.AreEqual(original[1].Order, cloned[1].Order);
        Assert.AreEqual(original[2].Element.Name, cloned[2].Element.Name);
        Assert.AreEqual(original[2].Order, cloned[2].Order);

        // ------------------------------------------------------------
        // 복제본의 Key 조회 확인
        // ------------------------------------------------------------
        Assert.IsTrue(cloned.ContainsKey("key1"), "복제본에서 Key 조회가 가능해야 합니다");
        Assert.IsTrue(cloned.ContainsKey("key2"), "복제본에서 Key 조회가 가능해야 합니다");
        Assert.IsTrue(cloned.ContainsKey("key3"), "복제본에서 Key 조회가 가능해야 합니다");
        Assert.AreEqual("First", cloned["key1"].Name);
        Assert.AreEqual("Second", cloned["key2"].Name);
        Assert.AreEqual("Third", cloned["key3"].Name);

        // ------------------------------------------------------------
        // 복제본 수정이 원본에 영향을 주지 않는지 확인
        // ------------------------------------------------------------
        cloned.Add("key4", new TestElement("Fourth"), 40);
        
        Assert.AreEqual(4, cloned.Count);
        Assert.AreEqual(3, original.Count, "복제본 수정이 원본에 영향을 주지 않아야 합니다");
        Assert.IsTrue(cloned.ContainsKey("key4"));
        Assert.IsFalse(original.ContainsKey("key4"));
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

