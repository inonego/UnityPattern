using System;

using NUnit.Framework;

using UnityEngine;

using inonego;

// ============================================================================
/// <summary>
/// Value 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_Value
{

#region Value 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Value의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Value_01_기본_생성_테스트()
    {
        // Arrange & Act
        var value = new Value<int>();

        // Assert
        Assert.AreEqual(0, value.Current);
        Assert.IsTrue(value.InvokeEvent);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Value의 값 변경 및 이벤트 발생을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Value_02_값_변경_통합_테스트()
    {
        // Arrange
        var value = new Value<int>();
        bool valueChangeEventFired = false;
        Value<int> valueChangeSender = null;
        ValueChangeEventArgs<int> valueChangeEventArgs = default;

        value.OnValueChange += (sender, e) => 
        {
            valueChangeEventFired = true;
            valueChangeSender = sender;
            valueChangeEventArgs = e;
        };

        // Act & Assert - 값 변경
        value.Current = 10;
        Assert.AreEqual(10, value.Current);
        Assert.IsTrue(valueChangeEventFired);
        Assert.AreEqual(value, valueChangeSender);
        Assert.AreEqual(0, valueChangeEventArgs.Previous);
        Assert.AreEqual(10, valueChangeEventArgs.Current);

        // Reset
        valueChangeEventFired = false;

        // Act & Assert - 동일한 값 설정 (이벤트 발생하지 않음)
        value.Current = 10;
        Assert.AreEqual(10, value.Current);
        Assert.IsFalse(valueChangeEventFired, "동일한 값 설정 시 이벤트가 발생하지 않아야 합니다");

        // Act & Assert - 다른 값 설정
        value.Current = 20;
        Assert.AreEqual(20, value.Current);
        Assert.IsTrue(valueChangeEventFired);
        Assert.AreEqual(10, valueChangeEventArgs.Previous);
        Assert.AreEqual(20, valueChangeEventArgs.Current);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Value의 InvokeEvent 설정에 따른 이벤트 발생을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Value_03_InvokeEvent_설정_테스트()
    {
        // Arrange
        var value = new Value<int>();
        bool valueChangeEventFired = false;

        value.OnValueChange += (sender, e) => valueChangeEventFired = true;

        // Act & Assert - InvokeEvent = false일 때
        value.InvokeEvent = false;
        value.Current = 10;
        Assert.IsFalse(valueChangeEventFired, "InvokeEvent가 false일 때 이벤트가 발생하지 않아야 합니다");

        // Reset
        valueChangeEventFired = false;

        // Act & Assert - InvokeEvent = true일 때
        value.InvokeEvent = true;
        value.Current = 20;
        Assert.IsTrue(valueChangeEventFired, "InvokeEvent가 true일 때 이벤트가 발생해야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Value의 암시적 변환 및 비교 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Value_04_암시적_변환_및_비교_테스트()
    {
        // Arrange
        var value = new Value<int>(42);

        // Act & Assert - 암시적 변환
        int intValue = value; // 암시적 변환
        Assert.AreEqual(42, intValue);

        // Act & Assert - 직접 값과 비교
        Assert.IsTrue(value.Equals(42));
        Assert.IsTrue(value.Equals(new Value<int>(42)));
        Assert.IsFalse(value.Equals(new Value<int>(10)));

        // Act & Assert - ToString
        Assert.AreEqual("42", value.ToString());
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Value 상속 클래스의 ProcessValue 오버라이드를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Value_05_상속_ProcessValue_테스트()
    {
        // Arrange
        var customValue = new CustomValue();

        // Act & Assert - ProcessValue 오버라이드 동작
        customValue.Current = 5;
        Assert.AreEqual(10, customValue.Current, "ProcessValue에서 값이 2배로 처리되어야 합니다");

        customValue.Current = -3;
        Assert.AreEqual(0, customValue.Current, "ProcessValue에서 음수는 0으로 처리되어야 합니다");
    }

#endregion

#region Value 상속 클래스

    // ------------------------------------------------------------
    /// <summary>
    /// ProcessValue를 오버라이드하는 커스텀 Value 클래스
    /// </summary>
    // ------------------------------------------------------------
    private class CustomValue : Value<int>
    {
        protected override void ProcessValue(in int prev, ref int next)
        {
            // 값에 2를 곱하고, 음수는 0으로 처리
            next = Math.Max(0, next * 2);
        }
    }

#endregion

#region Value 직렬화 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Value 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Value_06_JSON_직렬화_테스트()
    {
        // Arrange - 커스텀 Value 클래스 (ProcessValue 오버라이드)
        var originalValue = new CustomValue();
        originalValue.Current = 5; // ProcessValue에서 2배로 처리되어 10이 됨
        originalValue.InvokeEvent = false; // 직렬화 시 이벤트는 무시

        // Act - 직렬화/역직렬화
        string json = JsonUtility.ToJson(originalValue);
        var deserializedValue = JsonUtility.FromJson<CustomValue>(json);

        // Assert - 상태 복원 확인
        Assert.AreEqual(originalValue.Current, deserializedValue.Current, "현재 값이 올바르게 복원되어야 합니다");
        Assert.AreEqual(originalValue.InvokeEvent, deserializedValue.InvokeEvent, "InvokeEvent 설정이 올바르게 복원되어야 합니다");
        
        // ProcessValue 동작 확인
        deserializedValue.Current = 3; // 3 * 2 = 6
        Assert.AreEqual(6, deserializedValue.Current, "ProcessValue 오버라이드가 올바르게 동작해야 합니다");
    }

#endregion

}
