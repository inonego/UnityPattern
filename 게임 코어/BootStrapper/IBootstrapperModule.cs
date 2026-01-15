using UnityEngine;

namespace inonego.Core.Bootstrapper
{
    public interface IBootstrapperModule
    {
        // -------------------------------------------------------------
        /// <summary>
        /// 부트스트래퍼에서 컴포넌트 생성 후 호출되는 초기화 메서드입니다.
        /// </summary>
        // -------------------------------------------------------------
        public Awaitable Init();
    }
}
