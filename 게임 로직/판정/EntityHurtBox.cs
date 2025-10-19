using System;

using UnityEngine;

namespace inonego
{
    public class EntityHurtBox : MonoEntityBehaviour
    {
        [SerializeField]
        private InvokeEventFlag invokeEvent = new();
        public InvokeEventFlag InvokeEvent => invokeEvent;
    }
}