using UnityEngine;

namespace inonego
{
    public interface IState<TSender>
    {
        public void Enter(TSender sender);
        public void Exit(TSender sender);
        public void Update(TSender sender);
        public void FixedUpdate(TSender sender);
        public void LateUpdate(TSender sender);
    }
}