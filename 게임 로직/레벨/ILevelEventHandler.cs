using System;

namespace inonego
{
    public delegate void LevelUpEvent<in TSender>(TSender sender, LevelUpEventArgs e);

    [Serializable]
    public struct LevelUpEventArgs
    {
        public int Level;
    }

    // =======================================================================================
    /// <summary>
    /// 레벨 이벤트 핸들러 인터페이스입니다.
    /// </summary>
    // =======================================================================================
    public interface ILevelEventHandler<out TSelf>
    {   
        public InvokeEventFlag InvokeEvent { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 레벨 업 이벤트입니다. 레벨이 1 오를때마다 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        public event LevelUpEvent<TSelf> OnLevelUp;
        public event ValueChangeEvent<TSelf, int> OnValueChange;
    }
}