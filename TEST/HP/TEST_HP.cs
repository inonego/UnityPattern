using System;

using NUnit.Framework;

using inonego;

// ============================================================================
/// <summary>
/// HP 시스템의 핵심 기능 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_HP
{

#region HP 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_01_기본_생성_테스트()
    {
        // Arrange & Act
        var hp = new HP();

        // Assert
        Assert.AreEqual(0, hp.Value);
        Assert.AreEqual(0, hp.MaxValue);
        Assert.AreEqual(0.0f, hp.Ratio);
        Assert.AreEqual(HP.State.Dead, hp.Current);
        Assert.IsFalse(hp.IsAlive);
        Assert.IsTrue(hp.IsDead);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 상태 변경 및 자동 상태 전환을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_02_상태_변경_통합_테스트()
    {
        // Arrange
        var hp = new HP();
        hp.MaxValue = 100;

        // Act & Assert - 살아있는 상태로 변경
        hp.MakeAlive();
        Assert.AreEqual(HP.State.Alive, hp.Current);
        Assert.AreEqual(100, hp.Value);
        Assert.AreEqual(100, hp.MaxValue);
        Assert.IsTrue(hp.IsAlive);
        Assert.IsFalse(hp.IsDead);

        // Act & Assert - 죽은 상태로 변경
        hp.MakeDead();
        Assert.AreEqual(HP.State.Dead, hp.Current);
        Assert.AreEqual(0, hp.Value);
        Assert.IsFalse(hp.IsAlive);
        Assert.IsTrue(hp.IsDead);

        // Act & Assert - 체력 설정으로 자동 상태 전환
        hp.Value = 50; // 0에서 50으로 변경 시 자동으로 살아있는 상태로 전환
        Assert.AreEqual(HP.State.Alive, hp.Current);
        Assert.AreEqual(50, hp.Value);
        Assert.IsTrue(hp.IsAlive);

        hp.Value = 0; // 50에서 0으로 변경 시 자동으로 죽은 상태로 전환
        Assert.AreEqual(HP.State.Dead, hp.Current);
        Assert.AreEqual(0, hp.Value);
        Assert.IsTrue(hp.IsDead);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 힐과 데미지 적용을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_03_힐_데미지_통합_테스트()
    {
        // Arrange
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();
        hp.Value = 50;

        // Act & Assert - 힐 적용
        hp.ApplyHeal(30);
        Assert.AreEqual(80, hp.Value);
        Assert.AreEqual(0.8f, hp.Ratio);

        // Act & Assert - 최대 체력 초과 힐
        hp.ApplyHeal(50); // 80 + 50 = 130, 최대 100으로 제한
        Assert.AreEqual(100, hp.Value);
        Assert.AreEqual(1.0f, hp.Ratio);

        // Act & Assert - 데미지 적용
        hp.ApplyDamage(40);
        Assert.AreEqual(60, hp.Value);
        Assert.AreEqual(0.6f, hp.Ratio);

        // Act & Assert - 치명적 데미지 (죽음)
        hp.ApplyDamage(100); // 60 - 100 = -40, 0으로 제한되고 죽음 상태로 전환
        Assert.AreEqual(0, hp.Value);
        Assert.AreEqual(HP.State.Dead, hp.Current);
        Assert.IsTrue(hp.IsDead);

        // Act & Assert - 죽은 상태에서 힐/데미지 무시
        hp.ApplyHeal(50); // 죽은 상태에서는 힐 무시
        Assert.AreEqual(0, hp.Value);
        Assert.IsTrue(hp.IsDead);

        hp.ApplyDamage(30); // 죽은 상태에서는 데미지 무시
        Assert.AreEqual(0, hp.Value);
        Assert.IsTrue(hp.IsDead);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 최대 체력 변경 및 자동 조정을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_04_최대체력_변경_테스트()
    {
        // Arrange
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();
        hp.Value = 80;

        // Act & Assert - 최대 체력 증가
        hp.MaxValue = 150;
        Assert.AreEqual(150, hp.MaxValue);
        Assert.AreEqual(80, hp.Value); // 현재 체력은 유지
        Assert.AreEqual(80.0f / 150.0f, hp.Ratio);

        // Act & Assert - 최대 체력 감소 (현재 체력 초과)
        hp.MaxValue = 50; // 80에서 50으로 감소
        Assert.AreEqual(50, hp.MaxValue);
        Assert.AreEqual(50, hp.Value); // 현재 체력이 최대 체력으로 조정됨
        Assert.AreEqual(1.0f, hp.Ratio);

        // Act & Assert - 최대 체력 0 설정
        hp.MaxValue = 0;
        Assert.AreEqual(0, hp.MaxValue);
        Assert.AreEqual(0, hp.Value);
        Assert.AreEqual(0.0f, hp.Ratio);
        Assert.IsTrue(hp.IsDead); // 최대 체력이 0이면 죽은 상태
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 비율 계산 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_05_비율_계산_테스트()
    {
        // Arrange
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();

        // Act & Assert - 다양한 체력 비율
        hp.Value = 0;
        Assert.AreEqual(0.0f, hp.Ratio);

        hp.Value = 25;
        Assert.AreEqual(0.25f, hp.Ratio);

        hp.Value = 50;
        Assert.AreEqual(0.5f, hp.Ratio);

        hp.Value = 75;
        Assert.AreEqual(0.75f, hp.Ratio);

        hp.Value = 100;
        Assert.AreEqual(1.0f, hp.Ratio);

        // Act & Assert - 최대 체력이 0일 때
        hp.MaxValue = 0;
        Assert.AreEqual(0.0f, hp.Ratio);
    }

#endregion

#region HP 이벤트 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 이벤트 발생을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_06_이벤트_통합_테스트()
    {
        // Arrange
        var hp = new HP();
        bool valueChangeEventFired = false;
        bool maxValueChangeEventFired = false;
        bool stateChangeEventFired = false;
        bool healEventFired = false;
        bool damageEventFired = false;

        HP valueChangeSender = null;
        HP maxValueChangeSender = null;
        HP stateChangeSender = null;
        HP healSender = null;
        HP damageSender = null;

        ValueChangeEventArgs<int> valueChangeEventArgs = default;
        ValueChangeEventArgs<int> maxValueChangeEventArgs = default;
        ValueChangeEventArgs<HP.State> stateChangeEventArgs = default;
        HP.ApplyEventArgs healEventArgs = default;
        HP.ApplyEventArgs damageEventArgs = default;

        hp.OnValueChange += (sender, e) => 
        {
            valueChangeEventFired = true;
            valueChangeSender = sender;
            valueChangeEventArgs = e;
        };

        hp.OnMaxValueChange += (sender, e) => 
        {
            maxValueChangeEventFired = true;
            maxValueChangeSender = sender;
            maxValueChangeEventArgs = e;
        };

        hp.OnStateChange += (sender, e) => 
        {
            stateChangeEventFired = true;
            stateChangeSender = sender;
            stateChangeEventArgs = e;
        };

        hp.OnHeal += (sender, e) => 
        {
            healEventFired = true;
            healSender = sender;
            healEventArgs = e;
        };

        hp.OnDamage += (sender, e) => 
        {
            damageEventFired = true;
            damageSender = sender;
            damageEventArgs = e;
        };

        // Act & Assert - 최대 체력 변경 이벤트
        hp.MaxValue = 100;
        Assert.IsTrue(maxValueChangeEventFired);
        Assert.AreEqual(hp, maxValueChangeSender);
        Assert.AreEqual(0, maxValueChangeEventArgs.Previous);
        Assert.AreEqual(100, maxValueChangeEventArgs.Current);

        // Reset
        maxValueChangeEventFired = false;

        // Act & Assert - 상태 변경 이벤트
        hp.MakeAlive();
        Assert.IsTrue(stateChangeEventFired);
        Assert.AreEqual(hp, stateChangeSender);
        Assert.AreEqual(HP.State.Dead, stateChangeEventArgs.Previous);
        Assert.AreEqual(HP.State.Alive, stateChangeEventArgs.Current);

        // Reset
        stateChangeEventFired = false;

        // Act & Assert - 체력 변경 이벤트
        hp.Value = 50;
        Assert.IsTrue(valueChangeEventFired);
        Assert.AreEqual(hp, valueChangeSender);
        Assert.AreEqual(100, valueChangeEventArgs.Previous);
        Assert.AreEqual(50, valueChangeEventArgs.Current);

        // Reset
        valueChangeEventFired = false;

        // Act & Assert - 힐 이벤트
        hp.ApplyHeal(30);
        Assert.IsTrue(healEventFired);
        Assert.AreEqual(hp, healSender);
        Assert.AreEqual(30, healEventArgs.Amount);

        // Reset
        healEventFired = false;

        // Act & Assert - 데미지 이벤트
        hp.ApplyDamage(20);
        Assert.IsTrue(damageEventFired);
        Assert.AreEqual(hp, damageSender);
        Assert.AreEqual(20, damageEventArgs.Amount);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 InvokeEvent 설정에 따른 이벤트 발생을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_07_InvokeEvent_설정_테스트()
    {
        // Arrange
        var hp = new HP();
        bool valueChangeEventFired = false;
        bool stateChangeEventFired = false;

        hp.OnValueChange += (sender, e) => valueChangeEventFired = true;
        hp.OnStateChange += (sender, e) => stateChangeEventFired = true;

        // Act & Assert - InvokeEvent = false일 때
        hp.InvokeEvent = false;
        hp.MaxValue = 100;
        hp.MakeAlive();
        Assert.IsFalse(valueChangeEventFired);
        Assert.IsFalse(stateChangeEventFired);

        // Reset
        valueChangeEventFired = false;
        stateChangeEventFired = false;

        // Act & Assert - InvokeEvent = true일 때
        hp.InvokeEvent = true;
        hp.Value = 50;
        Assert.IsTrue(valueChangeEventFired);
    }

#endregion

#region HP 유틸리티 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 Clone 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_08_Clone_테스트()
    {
        // Arrange
        var originalHp = new HP();
        originalHp.MaxValue = 100;
        originalHp.MakeAlive();
        originalHp.Value = 75;

        // Act
        var clonedHp = originalHp.Clone();

        // Assert
        Assert.AreEqual(originalHp.Current, clonedHp.Current);
        Assert.AreEqual(originalHp.Value, clonedHp.Value);
        Assert.AreEqual(originalHp.MaxValue, clonedHp.MaxValue);
        Assert.AreEqual(originalHp.Ratio, clonedHp.Ratio);
        Assert.AreNotSame(originalHp, clonedHp); // 다른 인스턴스여야 함
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 비율 기반 적용량 계산을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_09_비율_기반_계산_테스트()
    {
        // Arrange
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();
        hp.Value = 60; // 60% 체력

        // Act & Assert - ByValue (현재 체력 기준)
        int amount1 = hp.CalculateApplyAmount(0.5f, HP.ApplyRatioType.ByValue);
        Assert.AreEqual(30, amount1); // 60 * 0.5 = 30

        // Act & Assert - ByMaxValue (최대 체력 기준)
        int amount2 = hp.CalculateApplyAmount(0.3f, HP.ApplyRatioType.ByMaxValue);
        Assert.AreEqual(30, amount2); // 100 * 0.3 = 30

        // Act & Assert - ByMissingValue (부족한 체력 기준)
        int amount3 = hp.CalculateApplyAmount(0.5f, HP.ApplyRatioType.ByMissingValue);
        Assert.AreEqual(20, amount3); // (100 - 60) * 0.5 = 20
    }

#endregion

}
