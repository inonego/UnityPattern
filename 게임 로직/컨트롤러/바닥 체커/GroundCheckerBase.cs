using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public abstract class GroundCheckerBase : IGroundChecker
    {

    #region 필드

        [SerializeField, ReadOnly]
        protected GameObject ground = null;
        public GameObject Ground => ground;

        public abstract Vector3 Velocity { get; }
        public abstract Vector3 GroundVelocity { get; }
        public abstract Vector3 Gravity { get; }

        public abstract GameObject GameObject { get; }

        public bool IsOnGround => ground != null;

    #endregion

    #region 이벤트

        public event ValueChangeEvent<IGroundChecker, GameObject> OnLand = null;
        public event ValueChangeEvent<IGroundChecker, GameObject> OnLeave = null;

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 바닥을 감지하고 변경 및 이벤트를 발생시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Check(float deltaTime)
        {
            if (GameObject == null) return;

            var detected = Detect(deltaTime);

            Process(detected);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 레이캐스트 등의 방법을 통해 바닥을 감지합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract GameObject Detect(float deltaTime);

        // ------------------------------------------------------------
        /// <summary>
        /// 주어진 벡터에 따라 게임 오브젝트가 바닥으로 향하고 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected static bool IsHeadingToGround(Vector3 velocity, Vector3 groundVelocity, Vector3 gravity)
        {
            var delta = velocity - groundVelocity;

            return Vector3.Dot(delta.normalized, gravity.normalized) > -0.001f;
        }

        protected abstract void ProcessGround(GameObject prev, ref GameObject next);

        // ------------------------------------------------------------
        /// <summary>
        /// 바닥에 대한 조건을 확인하고 변경 및 이벤트를 발생시킵니다.
        /// </summary>
        // ------------------------------------------------------------
        protected void Process(GameObject ground)
        {
            var (prev, next) = (this.ground, ground);

            ProcessGround(prev, ref next);

            if (prev == next) return;

            if (prev != null)
            {
                OnLeave?.Invoke(this, new() { Previous = prev, Current = next });
            }

            this.ground = next;

            if (next != null)
            {
                OnLand?.Invoke(this, new() { Previous = prev, Current = next });
            }
        }

    #endregion

    }
}