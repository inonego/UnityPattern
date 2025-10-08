using System;
using UnityEngine;

namespace inonego
{
    [Serializable]
    public struct JumpEventArgs
    {

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
        /// 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private bool invokeEvent = true;
        public bool InvokeEvent
        {
            get => invokeEvent;
            set => invokeEvent = value;
        }

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

        public Jumper(IGroundChecker groundChecker)
        {
            if (groundChecker == null)
            {
                throw new ArgumentNullException("바닥 체커가 null입니다. 생성자에서 초기화해주세요.");
            }

            this.groundChecker = groundChecker;

            groundChecker.OnLand += OnGroundLand;
            groundChecker.OnLeave += OnGroundLeave;

            Reset();
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 점프를 실행하도록 트리거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Trigger()
        {
            // 횟수 체크는 Jump() 메서드에서 진행합니다.

            // ------------------------------------------------------------
            // 코요테 점프 타이머가 작동 중이면 바로 점프 실행
            // ------------------------------------------------------------
            if (coyoteJumpTimer.IsRunning)
            {
                Jump();
            }
            else
            {
                // ------------------------------------------------------------
                // 바닥에 닿아있으면 바로 점프 실행
                // ------------------------------------------------------------
                if (groundChecker.IsOnGround)
                {
                    Jump();
                }
                else
                {
                    jumpBufferTimer.Start(JumpBufferDuration);
                }
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 점프를 실행합니다.
        /// </summary>
        // ------------------------------------------------------------
        private void Jump()
        {
            coyoteJumpTimer.Stop();
            jumpBufferTimer.Stop();

            if (count <= 0)
            {
                return;
            }

            count--;
            
            if (InvokeEvent)
            {
                OnJump?.Invoke(this, new JumpEventArgs());
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 업데이트를 진행합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update()
        {
            Update(Time.deltaTime);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 업데이트를 진행합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update(float deltaTime)
        {
            coyoteJumpTimer.Update(deltaTime);
            jumpBufferTimer.Update(deltaTime);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 점프 카운트를 초기화합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Reset()
        {
            Count = MaxCount;
        }

    #endregion

    #region 이벤트 핸들러

        // ------------------------------------------------------------
        /// <summary>
        /// 바닥에 착지했을 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void OnGroundLand(IGroundChecker sender, ValueChangeEventArgs<GameObject> args)
        {
            Reset();

            // 점프 버퍼 타이머가 작동 중이면 바로 점프 실행
            if (jumpBufferTimer.IsRunning)
            {
                Jump();
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 바닥을 벗어났을 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void OnGroundLeave(IGroundChecker sender, ValueChangeEventArgs<GameObject> args)
        {
            coyoteJumpTimer.Start(CoyoteJumpDuration);
        }

    #endregion
    
    }
}