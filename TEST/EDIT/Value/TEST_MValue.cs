using System;

using NUnit.Framework;

using UnityEngine;

using inonego;
using inonego.Modifier;

// ============================================================================
/// <summary>
/// MValue 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_MValue
{

#region MValue 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_01_기본_생성_테스트()
    {
        // Arrange & Act
        var mValue = new MValue<int>();

        // Assert
        Assert.AreEqual(0, mValue.Base);
        Assert.AreEqual(0, mValue.Modified);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 Modifier 추가 및 적용을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_02_Modifier_추가_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var mValue = new MValue<int>(100);
        var addModifier = new NumericIModifier(NumericIOperation.ADD, 10); // +10

        // ------------------------------------------------------------
        // Modifier 추가 - Modified 값 변경 확인
        // ------------------------------------------------------------
        mValue.AddModifier("add", addModifier, order: 1);
        
        Assert.AreEqual(100, mValue.Base);
        Assert.AreEqual(110, mValue.Modified, "Base(100) + Modifier(+10) = 110이어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 Modifier Order 순서대로 적용되는지 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_03_Modifier_Order_순서_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var mValue = new MValue<int>(100);
        
        // Order가 낮을수록 먼저 적용됨
        var addModifier = new NumericIModifier(NumericIOperation.ADD, 50);   // +50
        var multiModifier = new NumericIModifier(NumericIOperation.MUL, 2); // *2

        // ------------------------------------------------------------
        // Modifier 추가 - 역순으로 추가해도 Order에 따라 적용
        // ------------------------------------------------------------
        mValue.AddModifier("add", addModifier, order: 2);
        mValue.AddModifier("multi", multiModifier, order: 1);
        
        // 순서: Base(100) -> Multi(*2, order:1) = 200 -> Add(+50, order:2) = 250
        Assert.AreEqual(250, mValue.Modified, "Order 1(Multi) -> Order 2(Add) 순서로 적용되어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 Modifier 제거를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_04_Modifier_제거_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var mValue = new MValue<int>(100);
        mValue.AddModifier("add", new NumericIModifier(NumericIOperation.ADD, 20), order: 1);
        mValue.AddModifier("multi", new NumericIModifier(NumericIOperation.MUL, 2), order: 2);
        
        Assert.AreEqual(240, mValue.Modified); // (100 + 20) * 2 = 240

        // ------------------------------------------------------------
        // Modifier 제거 - Modified 값 재계산 확인
        // ------------------------------------------------------------
        bool removed = mValue.RemoveModifier("add");
        
        Assert.IsTrue(removed, "Modifier 제거에 성공해야 합니다");
        Assert.AreEqual(200, mValue.Modified, "add 제거 후 100 * 2 = 200이어야 합니다");

        // ------------------------------------------------------------
        // 없는 Modifier 제거 시도
        // ------------------------------------------------------------
        removed = mValue.RemoveModifier("nonexistent");
        
        Assert.IsFalse(removed, "존재하지 않는 Modifier 제거는 false를 반환해야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 모든 Modifier 제거를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_05_Modifier_전체_제거_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var mValue = new MValue<int>(100);
        mValue.AddModifier("add1", new NumericIModifier(NumericIOperation.ADD, 10), order: 1);
        mValue.AddModifier("add2", new NumericIModifier(NumericIOperation.ADD, 20), order: 2);
        mValue.AddModifier("multi", new NumericIModifier(NumericIOperation.MUL, 2), order: 3);
        
        Assert.AreEqual(260, mValue.Modified); // ((100 + 10) + 20) * 2 = 260

        // ------------------------------------------------------------
        // 모든 Modifier 제거
        // ------------------------------------------------------------
        mValue.ClearModifiers();
        
        Assert.AreEqual(100, mValue.Modified, "모든 Modifier 제거 후 Base 값만 남아야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 이벤트 발생을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_06_이벤트_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var mValue = new MValue<int>(100);
        bool modifiedChangeFired = false;
        ValueChangeEventArgs<int> modifiedChangeArgs = default;

        void Reset()
        {
            modifiedChangeFired = false;
            modifiedChangeArgs = default;
        }

        mValue.OnModifiedChange += (sender, e) => 
        {
            modifiedChangeFired = true;
            modifiedChangeArgs = e;
        };

        // ------------------------------------------------------------
        // Base 값 변경 - Modified 이벤트 발생
        // ------------------------------------------------------------
        mValue.Base = 200;
        
        Assert.IsTrue(modifiedChangeFired, "Base 변경 시 Modified 이벤트가 발생해야 합니다");
        Assert.AreEqual(100, modifiedChangeArgs.Previous);
        Assert.AreEqual(200, modifiedChangeArgs.Current);

        Reset();

        // ------------------------------------------------------------
        // Modifier 추가 - Modified 이벤트 발생
        // ------------------------------------------------------------
        mValue.AddModifier("add", new NumericIModifier(NumericIOperation.ADD, 50), order: 1);
        
        Assert.IsTrue(modifiedChangeFired, "Modifier 추가 시 Modified 이벤트가 발생해야 합니다");
        Assert.AreEqual(200, modifiedChangeArgs.Previous);
        Assert.AreEqual(250, modifiedChangeArgs.Current);

        Reset();

        // ------------------------------------------------------------
        // Modifier 제거 - Modified 이벤트 발생
        // ------------------------------------------------------------
        mValue.RemoveModifier("add");
        
        Assert.IsTrue(modifiedChangeFired, "Modifier 제거 시 Modified 이벤트가 발생해야 합니다");
        Assert.AreEqual(250, modifiedChangeArgs.Previous);
        Assert.AreEqual(200, modifiedChangeArgs.Current);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 암시적 변환을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_07_암시적_변환_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var mValue = new MValue<int>(100);
        mValue.AddModifier("add", new NumericIModifier(NumericIOperation.ADD, 50), order: 1);

        // ------------------------------------------------------------
        // 암시적 변환 - MValue<T>를 T로 변환 (Modified 값 반환)
        // ------------------------------------------------------------
        int intValue = mValue;
        
        Assert.AreEqual(150, intValue, "암시적 변환 시 Modified 값을 반환해야 합니다");
    }

#endregion

#region MValue 복제 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// MValue의 복제 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_08_복제_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var original = new MValue<int>(100);
        original.AddModifier("add", new NumericIModifier(NumericIOperation.ADD, 20), order: 1);
        original.AddModifier("multi", new NumericIModifier(NumericIOperation.MUL, 2), order: 2);
        
        Assert.AreEqual(240, original.Modified); // (100 + 20) * 2

        // ------------------------------------------------------------
        // Clone - 깊은 복사 확인
        // ------------------------------------------------------------
        var cloned = original.Clone();
        
        Assert.AreEqual(100, cloned.Base);
        Assert.AreEqual(240, cloned.Modified);
        Assert.AreNotSame(original, cloned, "복제본은 원본과 다른 인스턴스여야 합니다");

        // ------------------------------------------------------------
        // 복제본 수정 - 원본 영향 없음 확인
        // ------------------------------------------------------------
        cloned.Base = 200;
        cloned.RemoveModifier("add");
        
        Assert.AreEqual(100, original.Base, "복제본의 Base 변경이 원본에 영향을 주지 않아야 합니다");
        Assert.AreEqual(240, original.Modified, "복제본의 Modifier 제거가 원본에 영향을 주지 않아야 합니다");
        Assert.AreEqual(400, cloned.Modified); // 200 * 2
    }

#endregion

#region MValue 직렬화 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// MValue 클래스의 JSON 직렬화/역직렬화를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void MValue_09_JSON_직렬화_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var original = new MValue<int>(100);
        original.AddModifier("add", new NumericIModifier(NumericIOperation.ADD, 30), order: 1);
        original.AddModifier("multi", new NumericIModifier(NumericIOperation.MUL, 2), order: 2);
        
        Assert.AreEqual(260, original.Modified); // (100 + 30) * 2

        // ------------------------------------------------------------
        // JSON 직렬화/역직렬화 - 상태 복원 확인
        // ------------------------------------------------------------
        string json = JsonUtility.ToJson(original);
        var deserialized = JsonUtility.FromJson<MValue<int>>(json);
        
        Assert.AreEqual(original.Base, deserialized.Base, "Base 값이 올바르게 복원되어야 합니다");
        Assert.AreEqual(original.Modified, deserialized.Modified, "Modified 값이 올바르게 복원되어야 합니다");

        // ------------------------------------------------------------
        // 역직렬화 후 Modifier 동작 확인
        // ------------------------------------------------------------
        deserialized.AddModifier("add2", new NumericIModifier(NumericIOperation.ADD, 10), order: 3);
        
        Assert.AreEqual(270, deserialized.Modified); // ((100 + 30) * 2) + 10
    }

#endregion

}

