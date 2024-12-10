using System;

using UnityEngine;
using UnityEngine.Events;

namespace inonego.util
{
    public struct EventArgs { }

    public class Event<TSender, TEventArgs>
    {
        private event Action<TSender, TEventArgs> internal_event;

        [SerializeField]
        private UnityEvent<TSender, TEventArgs> internal_unity_event;

        public bool IsDirty { get; private set; } = false;

        public void SetDirty()
        {
            IsDirty = true;
        }

        public static Event<TSender, TEventArgs> operator +(Event<TSender, TEventArgs> target, Action<TSender, TEventArgs> action)
        {
            target.internal_event += action;

            return target;
        }

        public static Event<TSender, TEventArgs> operator -(Event<TSender, TEventArgs> target, Action<TSender, TEventArgs> action)
        {
            target.internal_event -= action;

            return target;
        }

        public void InvokeHere(TSender sender, TEventArgs eventArgs)
        {
            internal_event?.Invoke(sender, eventArgs);
            internal_unity_event?.Invoke(sender, eventArgs);
        }

        public void InvokeIfDirty(TSender sender, TEventArgs eventArgs)
        {
            if (IsDirty)
            {
                InvokeHere(sender, eventArgs);
                
                IsDirty = false;
            }
        }
    }
}

