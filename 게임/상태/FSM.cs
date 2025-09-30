using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public class FSM<TSource> where TSource : class
    {
        [SerializeReference]
        private TSource sender = null;
        public TSource Sender => sender;

        public event ValueChangeEvent<TSource, IState<TSource>> OnStateChange;

        [SerializeReference]
        private IState<TSource> state = null;
        public IState<TSource> State => state;  

        public FSM(TSource sender)
        {
            this.sender = sender;
        }

        public void MoveTo(IState<TSource> state)
        {
            var (prev, next) = (this.state, state);

            if (prev == next) return;

            prev?.Exit(sender);

            this.state = next;

            next?.Enter(sender);

            OnStateChange?.Invoke(sender, new() { Previous = prev, Current = next });
        }
    }
}