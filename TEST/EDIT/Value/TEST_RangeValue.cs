using System;

using NUnit.Framework;

using UnityEngine;

using inonego;
using inonego.Serializable;

// ============================================================================
/// <summary>
/// RangeValue 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_RangeValue
{

#region RangeValue 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// RangeValue의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void RangeValue_01_기본_생성_테스트()
    {
        // Arrange & Act
        var rangeValue = new RangeValue<int>();

        // Assert
        Assert.AreEqual(0, rangeValue.Current);
        Assert.AreEqual(0, rangeValue.Min);
        Assert.AreEqual(0, rangeValue.Max);
    }

    // ----------------------------------------------------------------------
    /// <summary>
    /// RangeValue의 범위 설정 및 값 제한, 개별 설정을 통합 테스트합니다.
    /// </summary>
    // ----------------------------------------------------------------------
    [Test]
    public void RangeValue_02_범위_설정_및_값_제한_통합_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var rangeValue = new RangeValue<int>();

        // ------------------------------------------------------------
        // Range.Current로 범위 설정 - 현재값 최소값으로 조정
        // ------------------------------------------------------------
        rangeValue.Range.Current = (10, 50);
        
        Assert.AreEqual(10, rangeValue.Min);
        Assert.AreEqual(50, rangeValue.Max);
        Assert.AreEqual(10, rangeValue.Current, "범위 설정 시 현재값이 최소값으로 조정되어야 합니다");

        // ------------------------------------------------------------
        // 범위 내 값 설정
        // ------------------------------------------------------------
        rangeValue.Current = 30;
        
        Assert.AreEqual(30, rangeValue.Current);

        // ------------------------------------------------------------
        // 범위 초과 값 설정 - 최대값으로 제한
        // ------------------------------------------------------------
        rangeValue.Current = 100;
        
        Assert.AreEqual(50, rangeValue.Current, "범위를 초과하는 값은 최대값으로 제한되어야 합니다");

        // ------------------------------------------------------------
        // 범위 미만 값 설정 - 최소값으로 제한
        // ------------------------------------------------------------
        rangeValue.Current = 5;
        
        Assert.AreEqual(10, rangeValue.Current, "범위 미만 값은 최소값으로 제한되어야 합니다");

        // ------------------------------------------------------------
        // Min 개별 변경 - 현재값 유지
        // ------------------------------------------------------------
        rangeValue.Current = 30;
        rangeValue.Range.Current = (20, 50);
        
        Assert.AreEqual(20, rangeValue.Min);
        Assert.AreEqual(50, rangeValue.Max);
        Assert.AreEqual(30, rangeValue.Current, "Min 변경 시 현재값은 유지되어야 합니다");

        // ------------------------------------------------------------
        // Max 개별 변경 - 현재값 유지
        // ------------------------------------------------------------
        rangeValue.Range.Current = (20, 40);
        
        Assert.AreEqual(20, rangeValue.Min);
        Assert.AreEqual(40, rangeValue.Max);
        Assert.AreEqual(30, rangeValue.Current, "Max 변경 시 현재값은 유지되어야 합니다");

        // ------------------------------------------------------------
        // Min이 현재값보다 클 때 - 현재값 Min으로 조정
        // ------------------------------------------------------------
        rangeValue.Range.Current = (35, 40);
        
        Assert.AreEqual(35, rangeValue.Current, "Min이 현재값보다 클 때 현재값이 Min으로 조정되어야 합니다");

        // ------------------------------------------------------------
        // Max가 현재값보다 작을 때 - 현재값 Max로 조정
        // ------------------------------------------------------------
        rangeValue.Range.Current = (0, 25);
        
        Assert.AreEqual(25, rangeValue.Current, "Max가 현재값보다 작을 때 현재값이 Max로 조정되어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Min/Max 및 Current 이벤트 발생을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void RangeValue_03_이벤트_통합_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var rangeValue = new RangeValue<int>();
        bool valueChangeEventFired = false;
        Value<int> valueChangeSender = null;
        ValueChangeEventArgs<int> valueChangeEventArgs = default;

        bool rangeChangeFired = false;
        Value<MinMax<int>> rangeChangeSender = null;
        ValueChangeEventArgs<MinMax<int>> rangeChangeArgs = default;

        void Reset()
        {
            valueChangeEventFired = false;
            rangeChangeFired = false;
            valueChangeSender = null;
            rangeChangeSender = null;
            valueChangeEventArgs = default;
            rangeChangeArgs = default;
        }

        rangeValue.OnCurrentChange += (sender, e) => 
        {
            valueChangeEventFired = true;
            valueChangeSender = sender as Value<int>;
            valueChangeEventArgs = e;
        };

        rangeValue.Range.OnCurrentChange += (sender, e) =>
        {
            rangeChangeFired = true;
            rangeChangeSender = sender as Value<MinMax<int>>;
            rangeChangeArgs = e;
        };

        // ------------------------------------------------------------
        // Range.Current로 범위 설정 - 이벤트 발생 확인
        // ------------------------------------------------------------
        rangeValue.Range.Current = (10, 50);
        
        Assert.IsTrue(rangeChangeFired);
        
        Reset();
        
        // ------------------------------------------------------------
        // Range 범위 변경 이벤트 확인
        // ------------------------------------------------------------
        rangeValue.Range.Current = (15, 50);
        
        Assert.IsTrue(rangeChangeFired);
        Assert.AreEqual(15, rangeValue.Min);
        Assert.AreEqual(50, rangeValue.Max);

        Reset();

        // ------------------------------------------------------------
        // Current 값 변경 이벤트 확인
        // ------------------------------------------------------------
        rangeValue.Current = 30;
        
        Assert.IsTrue(valueChangeEventFired);
        Assert.AreEqual(rangeValue, valueChangeSender);
        Assert.AreEqual(15, valueChangeEventArgs.Previous);
        Assert.AreEqual(30, valueChangeEventArgs.Current);

        Reset();

        // ------------------------------------------------------------
        // Range 범위 변경 이벤트 확인
        // ------------------------------------------------------------
        rangeValue.Range.Current = (15, 40);
        
        Assert.IsTrue(rangeChangeFired);
        Assert.AreEqual(15, rangeValue.Min);
        Assert.AreEqual(40, rangeValue.Max);
        Assert.IsFalse(valueChangeEventFired);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// RangeValue의 비교 및 문자열 표현을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void RangeValue_04_비교_및_문자열_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var rangeValue = new RangeValue<int>(30, (10, 50));

        // ------------------------------------------------------------
        // CompareTo 비교 테스트
        // ------------------------------------------------------------
        Assert.AreEqual(0, rangeValue.CompareTo(30));
        Assert.AreEqual(1, rangeValue.CompareTo(20));
        Assert.AreEqual(-1, rangeValue.CompareTo(40));

        // ------------------------------------------------------------
        // ToString 문자열 표현 테스트
        // ------------------------------------------------------------
        Assert.AreEqual("30 (10 - 50)", rangeValue.ToString());
    }

#endregion

#region RangeValue 직렬화 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// RangeValue 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void RangeValue_05_JSON_직렬화_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var originalRangeValue = new RangeValue<int>(50, (10, 100));

        // ------------------------------------------------------------
        // JSON 직렬화/역직렬화 - 상태 복원 확인
        // ------------------------------------------------------------
        string json = JsonUtility.ToJson(originalRangeValue);
        var deserializedRangeValue = JsonUtility.FromJson<RangeValue<int>>(json);
        
        Assert.AreEqual(originalRangeValue.Current, deserializedRangeValue.Current, "현재 값이 올바르게 복원되어야 합니다");
        Assert.AreEqual(originalRangeValue.Min, deserializedRangeValue.Min, "최소값이 올바르게 복원되어야 합니다");
        Assert.AreEqual(originalRangeValue.Max, deserializedRangeValue.Max, "최대값이 올바르게 복원되어야 합니다");

        // ------------------------------------------------------------
        // 역직렬화 후 생성자 호출 여부 확인 - OnRangeChange 핸들러 등록 확인
        // ------------------------------------------------------------
        // Range를 Current 범위 밖으로 변경하여 Current가 재적용되도록 함
        // 역직렬화 시 생성자가 호출되었다면 OnRangeChange 핸들러가 등록되어 Current가 조정될 것
        deserializedRangeValue.Range.Current = (60, 120);
        
        Assert.AreEqual(60, deserializedRangeValue.Current, 
            "역직렬화 후 생성자가 호출되었다면 OnRangeChange 핸들러가 등록되어 Min(60)이 현재값(50)보다 크면 Current가 Min으로 조정되어야 합니다. " +
            "이렇지 않다면 생성자가 호출되지 않아 OnRangeChange가 등록되지 않았을 수 있습니다.");
    }

#endregion

}
