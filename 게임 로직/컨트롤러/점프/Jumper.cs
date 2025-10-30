using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public struct JumpEventArgs
    {
        public int MaxCount;
        public int Count;
    }

    // ============================================================
    /// <summary>
    /// 점프 기능을 담당하는 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Jumper : IJumper
    {

    #region 필드

        // ------------------------------------------------------------
        /// <summary>
        /// 점프 중인지 여부를 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField, ReadOnly]
        private bool isJumping = false;
        public bool IsJumping => isJumping;

        // ------------------------------------------------------------
        /// <summary>
        /// 최대 점프 횟수입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private int maxCount = 1;
        public int MaxCount
        {
            get => maxCount;
            set 
            {
                maxCount = Mathf.Max(0, value);

                // 현재 남은 점프 횟수를 다시 계산합니다.
                Count = Count;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 남은 점프 횟수입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private int count = 0;
        public int Count
        {
            get => count;
            set
            {
                count = Mathf.Clamp(value, 0, maxCount);
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 코요테 점프 지속 시간입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private float coyoteJumpDuration = 0.1f;
        public float CoyoteJumpDuration
        {
            get => coyoteJumpDuration;
            set => coyoteJumpDuration = Mathf.Max(0f, value);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 점프 버퍼 지속 시간입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private float jumpBufferDuration = 0.1f;
        public float JumpBufferDuration
        {
            get => jumpBufferDuration;
            set => jumpBufferDuration = Mathf.Max(0f, value);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 코요테 점프 타이머입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField] 
        private Timer coyoteJumpTimer = new Timer();
        public IReadOnlyTimer CoyoteJumpTimer => coyoteJumpTimer;

        // ------------------------------------------------------------
        /// <summary>
        /// 점프 버퍼 타이머입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private Timer jumpBufferTimer = new Timer();
        public IReadOnlyTimer JumpBufferTimer => jumpBufferTimer;

        // ------------------------------------------------------------
        /// <summary>
        /// 바닥 체커입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeReference]
        private IGroundChecker groundChecker = null;

    #endregion

    #region 이벤트

        // ------------------------------------------------------------
        /// <summary>
        /// 점프가 시작될 때 호출되는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event Action<Jumper, JumpEventArgs> OnJump = null;

        event Action<IJumper, JumpEventArgs> IJumper.OnJump
        { add => OnJump += value; remove => OnJump -= value; }

    #endregion

    #region 생성자 및 초기화

        public void Init(IGroundChecker groundChecker)
        {
            if (groundChecker == null)
            {
                throw new ArgumentNullException("바닥 체커가 null입니다.");
            }

            this.groundChecker = groundChecker;

            Reset();
        }

    #endregion

    #region 메서드

        private void StartCoyoteJumpTimer()
        {
            coyoteJumpTimer.Stop();
            coyoteJumpTimer.Start(CoyoteJumpDuration);
        }

        private void StartJumpBufferTimer()
        {
            jumpBufferTimer.Stop();
            jumpBufferTimer.Start(JumpBufferDuration);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 점프를 실행하도록 트리거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Trigger()
        {
            StartJumpBufferTimer();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 점프를 실행합니다.
        /// </summary>
        // ------------------------------------------------------------
        private void Jump()
        {
            if (Count <= 0)
            {
                return;
            }

            jumpBufferTimer.Stop();
            coyoteJumpTimer.Stop();

            // 점프 카운트 감소
            Count--;

            // 점프 여부 설정
            isJumping = true;

            OnJump?.Invoke(this, new JumpEventArgs { MaxCount = MaxCount, Count = Count });
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 업데이트를 진행합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void FixedUpdate()
        {
            FixedUpdate(Time.fixedDeltaTime);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 업데이트를 진행합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (groundChecker == null)
            {
                throw new NullReferenceException("바닥 체커가 null입니다. Init 메서드를 통해 초기화해주세요.");
            }

            // 타이머 업데이트
            coyoteJumpTimer.Update(fixedDeltaTime);

            bool isTriggered = jumpBufferTimer.IsRunning;
            bool isGrounded = groundChecker.IsOnGround;

            // 점프를 했음에도 바닥에서 벗어나지 못하는 경우에 대비해
            // 업데이트에서 횟수를 리셋합니다.
            if (isGrounded)
            {
                Reset();

                StartCoyoteJumpTimer();
            }

            bool canJump = coyoteJumpTimer.IsRunning || isJumping;

            if (isTriggered && canJump)
            {
                Jump();
            }

            // 타이머 업데이트
            jumpBufferTimer.Update(fixedDeltaTime);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 점프 상태와 카운트를 초기화합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Reset()
        {
            isJumping = false;

            Count = MaxCount;
        }

    #endregion

    }
}