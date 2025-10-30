using System;

using NUnit.Framework;

using UnityEngine;

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
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var hp = new HP();

        // ------------------------------------------------------------
        // 테스트 결과
        // ------------------------------------------------------------
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
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var hp = new HP();
        hp.MaxValue = 100;

        // ------------------------------------------------------------
        // MakeAlive - 살아있는 상태로 변경
        // ------------------------------------------------------------
        hp.MakeAlive();
        
        Assert.AreEqual(HP.State.Alive, hp.Current);
        Assert.AreEqual(100, hp.Value);
        Assert.AreEqual(100, hp.MaxValue);
        Assert.IsTrue(hp.IsAlive);
        Assert.IsFalse(hp.IsDead);

        // ------------------------------------------------------------
        // MakeDead - 죽은 상태로 변경
        // ------------------------------------------------------------
        hp.MakeDead();
        
        Assert.AreEqual(HP.State.Dead, hp.Current);
        Assert.AreEqual(0, hp.Value);
        Assert.IsFalse(hp.IsAlive);
        Assert.IsTrue(hp.IsDead);

        // ------------------------------------------------------------
        // Value 설정으로 자동 상태 전환 - Alive
        // ------------------------------------------------------------
        hp.Value = 50;
        
        Assert.AreEqual(HP.State.Alive, hp.Current);
        Assert.AreEqual(50, hp.Value);
        Assert.IsTrue(hp.IsAlive);

        // ------------------------------------------------------------
        // Value 설정으로 자동 상태 전환 - Dead
        // ------------------------------------------------------------
        hp.Value = 0;
        
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
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();
        hp.Value = 50;

        // ------------------------------------------------------------
        // ApplyHeal - 힐 적용
        // ------------------------------------------------------------
        hp.ApplyHeal(30);
        
        Assert.AreEqual(80, hp.Value);
        Assert.AreEqual(0.8f, hp.Ratio);

        // ------------------------------------------------------------
        // ApplyHeal - 최대 체력 초과 힐 제한
        // ------------------------------------------------------------
        hp.ApplyHeal(50);
        
        Assert.AreEqual(100, hp.Value);
        Assert.AreEqual(1.0f, hp.Ratio);

        // ------------------------------------------------------------
        // ApplyDamage - 데미지 적용
        // ------------------------------------------------------------
        hp.ApplyDamage(40);
        
        Assert.AreEqual(60, hp.Value);
        Assert.AreEqual(0.6f, hp.Ratio);

        // ------------------------------------------------------------
        // ApplyDamage - 치명적 데미지로 상태 전환
        // ------------------------------------------------------------
        hp.ApplyDamage(100);
        
        Assert.AreEqual(0, hp.Value);
        Assert.AreEqual(HP.State.Dead, hp.Current);
        Assert.IsTrue(hp.IsDead);

        // ------------------------------------------------------------
        // ApplyHeal - 죽은 상태에서 힐 무시
        // ------------------------------------------------------------
        hp.ApplyHeal(50);
        
        Assert.AreEqual(0, hp.Value);
        Assert.IsTrue(hp.IsDead);

        // ------------------------------------------------------------
        // ApplyDamage - 죽은 상태에서 데미지 무시
        // ------------------------------------------------------------
        hp.ApplyDamage(30);
        
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
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();
        hp.Value = 80;

        // ------------------------------------------------------------
        // MaxValue 증가 - 현재 체력 유지
        // ------------------------------------------------------------
        hp.MaxValue = 150;
        
        Assert.AreEqual(150, hp.MaxValue);
        Assert.AreEqual(80, hp.Value);
        Assert.AreEqual(80.0f / 150.0f, hp.Ratio);

        // ------------------------------------------------------------
        // MaxValue 감소 - 현재 체력 조정
        // ------------------------------------------------------------
        hp.MaxValue = 50;
        
        Assert.AreEqual(50, hp.MaxValue);
        Assert.AreEqual(50, hp.Value);
        Assert.AreEqual(1.0f, hp.Ratio);

        // ------------------------------------------------------------
        // MaxValue 0 설정 - 죽은 상태 전환
        // ------------------------------------------------------------
        hp.MaxValue = 0;
        
        Assert.AreEqual(0, hp.MaxValue);
        Assert.AreEqual(0, hp.Value);
        Assert.AreEqual(0.0f, hp.Ratio);
        Assert.IsTrue(hp.IsDead);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 비율 계산 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_05_비율_계산_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();

        // ------------------------------------------------------------
        // Ratio 계산 - 다양한 체력 값
        // ------------------------------------------------------------
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

        // ------------------------------------------------------------
        // Ratio 계산 - MaxValue 0일 때
        // ------------------------------------------------------------
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
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
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

        void Reset()
        {
            valueChangeEventFired = false;
            maxValueChangeEventFired = false;
            stateChangeEventFired = false;
            healEventFired = false;
            damageEventFired = false;
            valueChangeSender = null;
            maxValueChangeSender = null;
            stateChangeSender = null;
            healSender = null;
            damageSender = null;
            valueChangeEventArgs = default;
            maxValueChangeEventArgs = default;
            stateChangeEventArgs = default;
            healEventArgs = default;
            damageEventArgs = default;
        }

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

        // ------------------------------------------------------------
        // OnMaxValueChange 이벤트 확인
        // ------------------------------------------------------------
        hp.MaxValue = 100;
        
        Assert.IsTrue(maxValueChangeEventFired);
        Assert.AreEqual(hp, maxValueChangeSender);
        Assert.AreEqual(0, maxValueChangeEventArgs.Previous);
        Assert.AreEqual(100, maxValueChangeEventArgs.Current);

        Reset();

        // ------------------------------------------------------------
        // OnStateChange 이벤트 확인
        // ------------------------------------------------------------
        hp.MakeAlive();
        
        Assert.IsTrue(stateChangeEventFired);
        Assert.AreEqual(hp, stateChangeSender);
        Assert.AreEqual(HP.State.Dead, stateChangeEventArgs.Previous);
        Assert.AreEqual(HP.State.Alive, stateChangeEventArgs.Current);

        Reset();

        // ------------------------------------------------------------
        // OnValueChange 이벤트 확인
        // ------------------------------------------------------------
        hp.Value = 50;
        
        Assert.IsTrue(valueChangeEventFired);
        Assert.AreEqual(hp, valueChangeSender);
        Assert.AreEqual(100, valueChangeEventArgs.Previous);
        Assert.AreEqual(50, valueChangeEventArgs.Current);

        Reset();

        // ------------------------------------------------------------
        // OnHeal 이벤트 확인
        // ------------------------------------------------------------
        hp.ApplyHeal(30);
        
        Assert.IsTrue(healEventFired);
        Assert.AreEqual(hp, healSender);
        Assert.AreEqual(30, healEventArgs.Amount);

        Reset();

        // ------------------------------------------------------------
        // OnDamage 이벤트 확인
        // ------------------------------------------------------------
        hp.ApplyDamage(20);
        
        Assert.IsTrue(damageEventFired);
        Assert.AreEqual(hp, damageSender);
        Assert.AreEqual(20, damageEventArgs.Amount);
    }

#endregion

#region HP 유틸리티 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 Clone 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_07_Clone_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var originalHp = new HP();
        originalHp.MaxValue = 100;
        originalHp.MakeAlive();
        originalHp.Value = 75;

        // ------------------------------------------------------------
        // Clone - 상태 복사 확인
        // ------------------------------------------------------------
        var clonedHp = originalHp.Clone();
        
        Assert.AreEqual(originalHp.Current, clonedHp.Current);
        Assert.AreEqual(originalHp.Value, clonedHp.Value);
        Assert.AreEqual(originalHp.MaxValue, clonedHp.MaxValue);
        Assert.AreEqual(originalHp.Ratio, clonedHp.Ratio);
        Assert.AreNotSame(originalHp, clonedHp);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// HP의 비율 기반 적용량 계산을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void HP_08_비율_기반_계산_테스트()
    {
        // ------------------------------------------------------------
        // 테스트 준비
        // ------------------------------------------------------------
        var hp = new HP();
        hp.MaxValue = 100;
        hp.MakeAlive();
        hp.Value = 60;

        // ------------------------------------------------------------
        // CalculateApplyAmount - ByValue (현재 체력 기준)
        // ------------------------------------------------------------
        int amount1 = hp.CalculateApplyAmount(0.5f, HP.ApplyRatioType.ByValue);
        
        Assert.AreEqual(30, amount1);

        // ------------------------------------------------------------
        // CalculateApplyAmount - ByMaxValue (최대 체력 기준)
        // ------------------------------------------------------------
        int amount2 = hp.CalculateApplyAmount(0.3f, HP.ApplyRatioType.ByMaxValue);
        
        Assert.AreEqual(30, amount2);

        // ------------------------------------------------------------
        // CalculateApplyAmount - ByMissingValue (부족한 체력 기준)
        // ------------------------------------------------------------
        int amount3 = hp.CalculateApplyAmount(0.5f, HP.ApplyRatioType.ByMissingValue);
        
        Assert.AreEqual(20, amount3);
    }

#endregion

}
