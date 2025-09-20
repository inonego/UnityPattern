using System;

using NUnit.Framework;

using inonego;

// ============================================================================
/// <summary>
/// Timer 시스템의 종합적인 테스트 클래스입니다.
/// </summary>
// ============================================================================
public class TEST_Timer
{

#region Timer 기본 기능 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Timer 클래스의 기본 생성 및 초기값을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_01_기본_생성_테스트()
    {
        // Arrange & Act
        var timer = new Timer();

        // Assert
        Assert.AreEqual(0.0f, timer.Duration, "초기 지속 시간은 0이어야 합니다");
        Assert.AreEqual(0.0f, timer.ElapsedTime, "초기 경과 시간은 0이어야 합니다");
        Assert.AreEqual(0.0f, timer.RemainingTime, "초기 남은 시간은 0이어야 합니다");
        Assert.AreEqual(float.NaN, timer.ElapsedTime01, "초기 경과 비율은 NaN이어야 합니다");
        Assert.AreEqual(float.NaN, timer.RemainingTime01, "초기 남은 비율은 NaN이어야 합니다");
        Assert.AreEqual(TimerState.Ready, timer.Current, "초기 상태는 Ready이어야 합니다");
        Assert.IsFalse(timer.IsWorking, "초기에는 작동하지 않아야 합니다");
        Assert.IsFalse(timer.IsPaused, "초기에는 일시정지되지 않아야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 시작 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_02_시작_테스트()
    {
        // Arrange
        var timer = new Timer();
        float duration = 5.0f;

        // Act
        timer.Start(duration);

        // Assert
        Assert.AreEqual(duration, timer.Duration, "지속 시간이 올바르게 설정되어야 합니다");
        Assert.AreEqual(0.0f, timer.ElapsedTime, "시작 시 경과 시간은 0이어야 합니다");
        Assert.AreEqual(duration, timer.RemainingTime, "시작 시 남은 시간은 지속 시간과 같아야 합니다");
        Assert.AreEqual(TimerState.Work, timer.Current, "상태가 Work이어야 합니다");
        Assert.IsTrue(timer.IsWorking, "작동 중이어야 합니다");
        Assert.IsFalse(timer.IsPaused, "일시정지되지 않아야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 중지 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_03_중지_테스트()
    {
        // Arrange
        var timer = new Timer();
        float duration = 5.0f;
        
        // Act
        timer.Start(5.0f);
        timer.Stop();

        // Assert
        Assert.AreEqual(duration, timer.Duration, "중지 시에도 Duration은 유지되어야 합니다");
        Assert.AreEqual(0.0f, timer.ElapsedTime, "중지 시 경과 시간은 0이어야 합니다");
        Assert.AreEqual(duration, timer.RemainingTime, "중지 시 남은 시간은 Duration과 같아야 합니다");
        Assert.AreEqual(TimerState.Ready, timer.Current, "상태가 Ready여야 합니다");
        Assert.IsFalse(timer.IsWorking, "작동하지 않아야 합니다");
        Assert.IsFalse(timer.IsPaused, "일시정지되지 않아야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 일시정지 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_04_일시정지_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);

        // Act
        timer.Pause();

        // Assert
        Assert.AreEqual(TimerState.Pause, timer.Current, "상태가 Pause여야 합니다");
        Assert.IsFalse(timer.IsWorking, "작동하지 않아야 합니다");
        Assert.IsTrue(timer.IsPaused, "일시정지되어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 재개 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_05_재개_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        timer.Pause();

        // Act
        timer.Resume();

        // Assert
        Assert.AreEqual(TimerState.Work, timer.Current, "상태가 Work이어야 합니다");
        Assert.IsTrue(timer.IsWorking, "작동해야 합니다");
        Assert.IsFalse(timer.IsPaused, "일시정지되지 않아야 합니다");
    }

#endregion

#region Timer 업데이트 및 시간 계산 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 업데이트 기능을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_06_업데이트_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);

        // Act
        timer.Update(2.0f); // 2초 경과

        // Assert
        Assert.AreEqual(2.0f, timer.ElapsedTime, "경과 시간이 올바르게 업데이트되어야 합니다");
        Assert.AreEqual(3.0f, timer.RemainingTime, "남은 시간이 올바르게 계산되어야 합니다");
        Assert.AreEqual(0.4f, timer.ElapsedTime01, "경과 비율이 올바르게 계산되어야 합니다");
        Assert.AreEqual(0.6f, timer.RemainingTime01, "남은 비율이 올바르게 계산되어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 완료 처리를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_07_완료_처리_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        bool eventFired = false;

        timer.OnEnd += (sender, e) => eventFired = true;

        // Act
        timer.Update(5.0f); // 정확히 지속 시간만큼 경과

        // Assert
        Assert.AreEqual(5.0f, timer.Duration, "Stop 후에도 Duration은 유지되어야 합니다");
        Assert.AreEqual(5.0f, timer.ElapsedTime, "경과 시간이 지속 시간과 같아야 합니다");
        Assert.AreEqual(0.0f, timer.RemainingTime, "남은 시간은 0이어야 합니다");
        Assert.AreEqual(1.0f, timer.ElapsedTime01, "경과 비율은 1이어야 합니다");
        Assert.AreEqual(0.0f, timer.RemainingTime01, "남은 비율은 0이어야 합니다");
        Assert.AreEqual(TimerState.Ready, timer.Current, "상태가 Ready여야 합니다");
        Assert.IsTrue(eventFired, "OnEnd 이벤트가 발생해야 합니다");
        Assert.IsFalse(timer.IsWorking, "작동하지 않아야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 오버플로우 처리를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_08_오버플로우_처리_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);

        // Act
        timer.Update(10.0f); // 지속 시간을 초과

        // Assert
        Assert.AreEqual(5.0f, timer.ElapsedTime, "경과 시간은 지속 시간을 초과하지 않아야 합니다");
        Assert.AreEqual(0.0f, timer.RemainingTime, "남은 시간은 0이어야 합니다");
        Assert.AreEqual(1.0f, timer.ElapsedTime01, "경과 비율은 1이어야 합니다");
        Assert.AreEqual(0.0f, timer.RemainingTime01, "남은 비율은 0이어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 일시정지 중 업데이트를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_09_일시정지_중_업데이트_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        timer.Update(2.0f); // 2초 경과
        timer.Pause(); // 일시정지

        // Act
        timer.Update(1.0f); // 일시정지 중 1초 경과

        // Assert
        Assert.AreEqual(2.0f, timer.ElapsedTime, "일시정지 중에는 경과 시간이 증가하지 않아야 합니다");
        Assert.AreEqual(3.0f, timer.RemainingTime, "일시정지 중에는 남은 시간이 감소하지 않아야 합니다");
    }

#endregion

#region Timer 이벤트 핸들링 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 OnStateChange 이벤트를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_10_OnStateChange_이벤트_테스트()
    {
        // Arrange
        var timer = new Timer();
        bool eventFired = false;
        TimerState eventState = TimerState.Ready;

        timer.OnStateChange += (sender, e) => {
            eventFired = true;
            eventState = e.Current;
        };

        // Act
        timer.Start(5.0f);

        // Assert
        Assert.IsTrue(eventFired, "OnStateChange 이벤트가 발생해야 합니다");
        Assert.AreEqual(TimerState.Work, eventState, "이벤트에서 올바른 상태를 전달해야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 OnEnd 이벤트를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_11_OnEnd_이벤트_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        bool eventFired = false;

        timer.OnEnd += (sender, e) => eventFired = true;

        // Act
        timer.Update(5.0f); // 타이머 완료

        // Assert
        Assert.IsTrue(eventFired, "OnEnd 이벤트가 발생해야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 InvokeEvent 설정을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_12_InvokeEvent_설정_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.InvokeEvent = false; // 이벤트 비활성화
        bool stateEventFired = false;
        bool endEventFired = false;

        timer.OnStateChange += (sender, e) => stateEventFired = true;
        timer.OnEnd += (sender, e) => endEventFired = true;

        // Act
        timer.Start(5.0f);
        timer.Update(5.0f);

        // Assert
        Assert.IsFalse(stateEventFired, "InvokeEvent가 false이면 OnStateChange 이벤트가 발생하지 않아야 합니다");
        Assert.IsFalse(endEventFired, "InvokeEvent가 false이면 OnEnd 이벤트가 발생하지 않아야 합니다");
    }

#endregion

#region Timer 예외 상황 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 중복 시작 예외를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_13_중복_시작_예외테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);

        // Act & Assert
        Assert.Throws<Timer.AlreadyRunningException>(() => timer.Start(3.0f));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 일시정지 상태에서 중복 시작 예외를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_14_일시정지_상태_중복_시작_예외테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        timer.Pause();

        // Act & Assert
        Assert.Throws<Timer.AlreadyRunningException>(() => timer.Start(3.0f));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 중지 상태에서 시작을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_15_중지_상태_시작_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        timer.Stop();

        // Act
        timer.Start(3.0f); // 중지 후 다시 시작

        // Assert
        Assert.AreEqual(3.0f, timer.Duration, "새로운 지속 시간이 설정되어야 합니다");
        Assert.AreEqual(0.0f, timer.ElapsedTime, "새로운 타이머 시작 시 경과 시간은 0이어야 합니다");
        Assert.AreEqual(3.0f, timer.RemainingTime, "새로운 타이머 시작 시 남은 시간은 새로운 Duration과 같아야 합니다");
        Assert.AreEqual(TimerState.Work, timer.Current, "상태가 Work이어야 합니다");
        Assert.IsTrue(timer.IsWorking, "작동해야 합니다");
    }

#endregion

#region Timer 경계값 및 특수 케이스 테스트

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 0 지속 시간을 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_16_0_지속시간_테스트()
    {
        // Arrange
        var timer = new Timer();
        bool eventFired = false;

        timer.OnEnd += (sender, e) => eventFired = true;

        // Act
        timer.Start(0.0f);
        timer.Update(0.0f);

        // Assert
        Assert.AreEqual(0.0f, timer.Duration, "지속 시간은 0이어야 합니다");
        Assert.AreEqual(0.0f, timer.ElapsedTime, "경과 시간은 0이어야 합니다");
        Assert.AreEqual(0.0f, timer.RemainingTime, "남은 시간은 0이어야 합니다");
        Assert.AreEqual(float.NaN, timer.ElapsedTime01, "0 지속 시간에서는 경과 비율이 NaN이어야 합니다");
        Assert.AreEqual(float.NaN, timer.RemainingTime01, "0 지속 시간에서는 남은 비율이 NaN이어야 합니다");
        Assert.IsTrue(eventFired, "0 지속 시간에서는 즉시 OnEnd 이벤트가 발생해야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 일시정지 후 재개 시 경과 시간 유지를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_19_일시정지_재개_경과시간_유지_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(10.0f);
        timer.Update(3.0f); // 3초 경과
        timer.Pause(); // 일시정지

        // Act
        timer.Resume(); // 재개
        timer.Update(2.0f); // 추가 2초 경과

        // Assert
        Assert.AreEqual(5.0f, timer.ElapsedTime, "일시정지 전 경과 시간이 유지되어야 합니다");
        Assert.AreEqual(5.0f, timer.RemainingTime, "남은 시간이 올바르게 계산되어야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Reset 메서드의 기본 기능과 예외 상황을 통합 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_17_Reset_통합_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        timer.Update(3.0f); // 일부 경과

        // Act & Assert - 작동 중일 때 Reset 시도 (예외 발생)
        Assert.Throws<Timer.FailedToResetException>(() => timer.Reset(), "작동 중일 때 Reset하면 예외가 발생해야 합니다");

        // Act & Assert - 일시정지 중일 때 Reset 시도 (예외 발생)
        timer.Pause();
        Assert.Throws<Timer.FailedToResetException>(() => timer.Reset(), "일시정지 중일 때 Reset하면 예외가 발생해야 합니다");

        // Act - 정상적인 Reset
        timer.Stop(); // 먼저 중지
        timer.Reset();

        // Assert - Reset 후 상태 확인
        Assert.AreEqual(0.0f, timer.Duration, "Reset 후 Duration은 0이어야 합니다");
        Assert.AreEqual(0.0f, timer.ElapsedTime, "Reset 후 ElapsedTime은 0이어야 합니다");
        Assert.AreEqual(0.0f, timer.RemainingTime, "Reset 후 RemainingTime은 0이어야 합니다");
        Assert.AreEqual(float.NaN, timer.ElapsedTime01, "Reset 후 ElapsedTime01은 NaN이어야 합니다");
        Assert.AreEqual(float.NaN, timer.RemainingTime01, "Reset 후 RemainingTime01은 NaN이어야 합니다");
        Assert.AreEqual(TimerState.Ready, timer.Current, "Reset 후 상태는 Ready여야 합니다");
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 음수 지속 시간으로 Start를 호출할 때 예외를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_18_음수_지속시간_예외테스트()
    {
        // Arrange
        var timer = new Timer();

        // Act & Assert
        Assert.Throws<Timer.InvalidDurationException>(() => timer.Start(-5.0f));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 음수 델타 시간으로 Update를 호출할 때 예외를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_19_음수_델타시간_예외테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);

        // Act & Assert
        Assert.Throws<Timer.InvalidDeltaTimeException>(() => timer.Update(-1.0f));
    }

    // ------------------------------------------------------------
    /// <summary>
    /// Timer의 중지 상태에서 일시정지/재개를 테스트합니다.
    /// </summary>
    // ------------------------------------------------------------
    [Test]
    public void Timer_20_중지_상태_일시정지_재개_테스트()
    {
        // Arrange
        var timer = new Timer();
        timer.Start(5.0f);
        timer.Stop();

        // Act
        timer.Pause(); // 중지 상태에서 일시정지
        timer.Resume(); // 중지 상태에서 재개

        // Assert
        Assert.AreEqual(TimerState.Ready, timer.Current, "중지 상태에서는 상태가 변경되지 않아야 합니다");
        Assert.IsFalse(timer.IsWorking, "중지 상태에서는 작동하지 않아야 합니다");
        Assert.IsFalse(timer.IsPaused, "중지 상태에서는 일시정지되지 않아야 합니다");
    }

#endregion

}
