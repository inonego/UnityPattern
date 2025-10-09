using UnityEngine;

namespace inonego
{
    public interface IState<TSender>
    {
        protected internal void Enter(TSender sender);
        protected internal void Exit(TSender sender);
        protected internal void Update(TSender sender);
        protected internal void FixedUpdate(TSender sender);
        protected internal void LateUpdate(TSender sender);
    }
}