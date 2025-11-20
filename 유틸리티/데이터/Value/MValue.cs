using System;

using UnityEngine;

namespace inonego
{
    using Modifier;
    using Serializable;

    // ============================================================
    /// <summary>
    /// 수정자를 적용할 수 있는 값을 관리하는 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class MValue<T> : Value<T>, IReadOnlyMValue<T>, IDeepCloneable<MValue<T>>
    where T : struct
    {

    #region 내부 구조체
    
        // ============================================================
        /// <summary>
        /// Key와 Modifier를 함께 저장하는 엔트리입니다.
        /// </summary>
        // ============================================================
        [Serializable]
        protected class ModifierEntry : IEquatable<ModifierEntry>
        {
            [SerializeField]
            private string key;
            public string Key => key;
            
            [SerializeReference]
            private IModifier<T> modifier;
            public IModifier<T> Modifier => modifier;
            
            private ModifierEntry() {}
            
            public ModifierEntry(string key, IModifier<T> modifier)
            {
                this.key = key;
                this.modifier = modifier;
            }
            
            public bool Equals(ModifierEntry other) => key == other.key;
            public override bool Equals(object obj) => obj is ModifierEntry other && Equals(other);
            public override int GetHashCode() => key?.GetHashCode() ?? 0;
        }

    #endregion

    #region 필드

        // ------------------------------------------------------------
        /// <summary>
        /// Order 순서로 정렬된 수정자 목록입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField, HideInInspector]
        private XOrdered<ModifierEntry, int> modifiers = new();
        
        // ------------------------------------------------------------
        /// <summary>
        /// 캐싱된 수정자 적용 값입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField, HideInInspector]
        private T cached;

        // ------------------------------------------------------------
        /// <summary>
        /// 수정자가 적용된 현재 값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Modified => cached;

    #endregion

    #region 이벤트

        // ------------------------------------------------------------
        /// <summary>
        /// 값이 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEventHandler<T> OnModifiedChange = null;

    #endregion

    #region 생성자

        public MValue() : this(default) {}

        public MValue(T value) : base(value)
        {
            Refresh();
        }

    #endregion

    #region 메서드

        private void Refresh(bool invokeEvent = true)
        {
            var (prev, next) = (cached, Modify(Base));

            if (Equals(prev, next)) return;

            cached = next;

            if (invokeEvent)
            {
                OnModifiedChange?.Invoke(this, new() { Previous = prev, Current = cached });
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 수정자가 적용된 값을 계산합니다.
        /// Order 순서대로 수정자를 적용합니다.
        /// </summary>
        // ------------------------------------------------------------
        private T Modify(T value)
        {
            foreach (var (entry, order) in modifiers)
            {
                value = entry.Modifier.Modify(value);
            }
            
            return value;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// Base 값을 설정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public override void Set(T value, bool invokeEvent = true)
        {
            base.Set(value, invokeEvent);

            Refresh(invokeEvent);
        }

    #endregion

    #region 수정자 관리

        // ------------------------------------------------------------
        /// <summary>
        /// 수정자를 추가합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void AddModifier(string key, IModifier<T> modifier, int order = 0, bool invokeEvent = true)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException(nameof(modifier), "추가하려는 수정자가 null입니다.");
            }

            var entry = new ModifierEntry(key, null);

            if (modifiers.Contains(entry))
            {
                throw new ArgumentException($"이미 존재하는 키({key})입니다.");
            }

            modifiers.Add(new ModifierEntry(key, modifier), order);

            Refresh(invokeEvent);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 키로 수정자를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool RemoveModifier(string key, bool invokeEvent = true)
        {
            var entry = new ModifierEntry(key, null);

            bool removed = modifiers.Remove(entry);
            
            if (removed)
            {
                Refresh(invokeEvent);

                return true;
            }
            
            return false;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 모든 수정자를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void ClearModifiers(bool invokeEvent = true)
        {
            if (modifiers.Count > 0)
            {
                modifiers.Clear();

                Refresh(invokeEvent);
            }
        }

    #endregion

    #region 복제

        public new MValue<T> @new() => new MValue<T>();

        public new MValue<T> Clone()
        {
            var result = @new();
            result.CloneFrom(this);
            return result;
        }

        public void CloneFrom(MValue<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"MValue<T>.CloneFrom()의 인자가 null입니다.");
            }

            base.CloneFrom(source);

            cached = source.cached;

            // 수정자 목록 복사
            modifiers.Clear();

            foreach (var (entry, order) in source.modifiers)
            {
                var clonedModifier = entry.Modifier.Clone();

                modifiers.Add(new ModifierEntry(entry.Key, clonedModifier), order);
            }
        }

    #endregion

    #region 암시적 변환

        // ------------------------------------------------------------
        /// <summary>
        /// MValue<T>에서 T로의 암시적 변환입니다.
        /// </summary>
        // ------------------------------------------------------------
        public static implicit operator T(MValue<T> wrapper)
        {
            return wrapper != null ? wrapper.Modified : default;
        }

    #endregion

    #region Object 오버라이드

        public override bool Equals(object obj)
        {
            if (obj is MValue<T> other)
                return Equals(Modified, other.Modified);
            if (obj is T directValue)
                return Equals(Modified, directValue);
            return false;
        }

        public override int GetHashCode() => Modified.GetHashCode();

        public override string ToString() => $"{Modified}({Base})";

    #endregion

    }
}
