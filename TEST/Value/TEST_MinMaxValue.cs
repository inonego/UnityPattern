using System;

using NUnit.Framework;

using UnityEngine;

using inonego;

// ============================================================================
/// <summary>
/// MinMaxValue 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_MinMaxValue
{

#region MinMaxValue 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_01_기본_생성_테스트()
    {
        // Arrange & Act
        var minMaxValue = new MinMaxValue<int>();

        // Assert
        Assert.AreEqual(0, minMaxValue.Current);
        Assert.AreEqual(0, minMaxValue.Min);
        Assert.AreEqual(0, minMaxValue.Max);
        Assert.IsTrue(minMaxValue.InvokeEvent);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue의 범위 설정 및 값 제한을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_02_범위_설정_통합_테스트()
    {
        // Arrange
        var minMaxValue = new MinMaxValue<int>();

        // Act & Assert - 범위 설정
        minMaxValue.Range = new MinMax<int>(10, 50);
        Assert.AreEqual(10, minMaxValue.Min);
        Assert.AreEqual(50, minMaxValue.Max);
        Assert.AreEqual(10, minMaxValue.Current, "범위 설정 시 현재값이 최소값으로 조정되어야 합니다");

        // Act & Assert - 범위 내 값 설정
        minMaxValue.Current = 30;
        Assert.AreEqual(30, minMaxValue.Current);

        // Act & Assert - 범위 초과 값 설정 (최대값으로 제한)
        minMaxValue.Current = 100;
        Assert.AreEqual(50, minMaxValue.Current, "범위를 초과하는 값은 최대값으로 제한되어야 합니다");

        // Act & Assert - 범위 미만 값 설정 (최소값으로 제한)
        minMaxValue.Current = 5;
        Assert.AreEqual(10, minMaxValue.Current, "범위 미만 값은 최소값으로 제한되어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue의 Min, Max 개별 설정을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_03_Min_Max_개별_설정_테스트()
    {
        // Arrange
        var minMaxValue = new MinMaxValue<int>();
        minMaxValue.Range = new MinMax<int>(10, 50);
        minMaxValue.Current = 30;

        // Act & Assert - Min 변경
        minMaxValue.Min = 20;
        Assert.AreEqual(20, minMaxValue.Min);
        Assert.AreEqual(50, minMaxValue.Max);
        Assert.AreEqual(30, minMaxValue.Current, "Min 변경 시 현재값은 유지되어야 합니다");

        // Act & Assert - Max 변경
        minMaxValue.Max = 40;
        Assert.AreEqual(20, minMaxValue.Min);
        Assert.AreEqual(40, minMaxValue.Max);
        Assert.AreEqual(30, minMaxValue.Current, "Max 변경 시 현재값은 유지되어야 합니다");

        // Act & Assert - Min이 현재값보다 클 때
        minMaxValue.Min = 35;
        Assert.AreEqual(35, minMaxValue.Current, "Min이 현재값보다 클 때 현재값이 Min으로 조정되어야 합니다");

        // Act & Assert - Max가 현재값보다 작을 때
        minMaxValue.Min = 0;
        minMaxValue.Max = 25;
        Assert.AreEqual(25, minMaxValue.Current, "Max가 현재값보다 작을 때 현재값이 Max로 조정되어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue의 이벤트 발생을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_04_이벤트_통합_테스트()
    {
        // Arrange
        var minMaxValue = new MinMaxValue<int>();
        bool valueChangeEventFired = false;
        bool rangeChangeEventFired = false;

        Value<int> valueChangeSender = null;
        MinMaxValue<int> rangeChangeSender = null;
        ValueChangeEventArgs<int> valueChangeEventArgs = default;
        ValueChangeEventArgs<MinMax<int>> rangeChangeEventArgs = default;

        minMaxValue.OnValueChange += (sender, e) => 
        {
            valueChangeEventFired = true;
            valueChangeSender = sender;
            valueChangeEventArgs = e;
        };

        minMaxValue.OnRangeChange += (sender, e) => 
        {
            rangeChangeEventFired = true;
            rangeChangeSender = sender;
            rangeChangeEventArgs = e;
        };

        // Act & Assert - 범위 변경 이벤트
        minMaxValue.Range = new MinMax<int>(10, 50);
        Assert.IsTrue(rangeChangeEventFired);
        Assert.AreEqual(minMaxValue, rangeChangeSender);
        Assert.AreEqual(new MinMax<int>(0, 0), rangeChangeEventArgs.Previous);
        Assert.AreEqual(new MinMax<int>(10, 50), rangeChangeEventArgs.Current);

        // Reset
        rangeChangeEventFired = false;

        // Act & Assert - 값 변경 이벤트
        minMaxValue.Current = 30;
        Assert.IsTrue(valueChangeEventFired);
        Assert.AreEqual(minMaxValue, valueChangeSender);
        Assert.AreEqual(10, valueChangeEventArgs.Previous);
        Assert.AreEqual(30, valueChangeEventArgs.Current);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue의 예외 상황을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_05_예외_처리_테스트()
    {
        // Arrange
        var minMaxValue = new MinMaxValue<int>();

        // Act & Assert - 최소값이 최대값보다 클 때 예외
        Assert.Throws<ArgumentException>(() => minMaxValue.Range = new MinMax<int>(50, 10));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue의 비교 및 문자열 표현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_06_비교_및_문자열_테스트()
    {
        // Arrange
        var minMaxValue = new MinMaxValue<int>();
        minMaxValue.Range = new MinMax<int>(10, 50);
        minMaxValue.Current = 30;

        // Act & Assert - 비교
        Assert.AreEqual(0, minMaxValue.CompareTo(30));
        Assert.AreEqual(1, minMaxValue.CompareTo(20));
        Assert.AreEqual(-1, minMaxValue.CompareTo(40));

        // Act & Assert - ToString
        Assert.AreEqual("30 (10 - 50)", minMaxValue.ToString());
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue의 InvokeEvent 설정에 따른 이벤트 발생을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_07_InvokeEvent_설정_테스트()
    {
        // Arrange
        var minMaxValue = new MinMaxValue<int>();
        bool valueChangeEventFired = false;
        bool rangeChangeEventFired = false;

        minMaxValue.OnValueChange += (sender, e) => valueChangeEventFired = true;
        minMaxValue.OnRangeChange += (sender, e) => rangeChangeEventFired = true;

        // Act & Assert - InvokeEvent = false일 때
        minMaxValue.InvokeEvent = false;
        minMaxValue.Range = new MinMax<int>(10, 50);
        minMaxValue.Current = 30;
        Assert.IsFalse(valueChangeEventFired, "InvokeEvent가 false일 때 값 변경 이벤트가 발생하지 않아야 합니다");
        Assert.IsFalse(rangeChangeEventFired, "InvokeEvent가 false일 때 범위 변경 이벤트가 발생하지 않아야 합니다");

        // Reset
        valueChangeEventFired = false;
        rangeChangeEventFired = false;

        // Act & Assert - InvokeEvent = true일 때
        minMaxValue.InvokeEvent = true;
        minMaxValue.Range = new MinMax<int>(20, 60);
        minMaxValue.Current = 40;
        Assert.IsTrue(rangeChangeEventFired, "InvokeEvent가 true일 때 범위 변경 이벤트가 발생해야 합니다");
        Assert.IsTrue(valueChangeEventFired, "InvokeEvent가 true일 때 값 변경 이벤트가 발생해야 합니다");
    }

#endregion

#region MinMaxValue 직렬화 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// MinMaxValue 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MinMaxValue_08_JSON_직렬화_테스트()
    {
        // Arrange - 범위와 값이 설정된 상태
        var originalMinMaxValue = new MinMaxValue<int>();
        originalMinMaxValue.Range = new MinMax<int>(10, 100);
        originalMinMaxValue.Current = 50;
        originalMinMaxValue.InvokeEvent = false; // 직렬화 시 이벤트는 무시

        // Act - 직렬화/역직렬화
        string json = JsonUtility.ToJson(originalMinMaxValue);
        var deserializedMinMaxValue = JsonUtility.FromJson<MinMaxValue<int>>(json);

        // Assert - 상태 복원 확인
        Assert.AreEqual(originalMinMaxValue.Current, deserializedMinMaxValue.Current, "현재 값이 올바르게 복원되어야 합니다");
        Assert.AreEqual(originalMinMaxValue.Min, deserializedMinMaxValue.Min, "최소값이 올바르게 복원되어야 합니다");
        Assert.AreEqual(originalMinMaxValue.Max, deserializedMinMaxValue.Max, "최대값이 올바르게 복원되어야 합니다");
        Assert.AreEqual(originalMinMaxValue.Range, deserializedMinMaxValue.Range, "범위가 올바르게 복원되어야 합니다");
        Assert.AreEqual(originalMinMaxValue.InvokeEvent, deserializedMinMaxValue.InvokeEvent, "InvokeEvent 설정이 올바르게 복원되어야 합니다");
    }

#endregion

}
