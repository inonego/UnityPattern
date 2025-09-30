using UnityEngine;

namespace inonego
{
    public interface IState<TSender>
    {
        void Enter(TSender sender);
        void Exit(TSender sender);
        void Update(TSender sender);
        void FixedUpdate(TSender sender);
        void LateUpdate(TSender sender);
        void OnTriggerEnter(TSender sender, Collider other);
        void OnTriggerExit(TSender sender, Collider other);
        void OnTriggerStay(TSender sender, Collider other);
        void OnTriggerEnter2D(TSender sender, Collider2D other);
        void OnTriggerExit2D(TSender sender, Collider2D other);
        void OnTriggerStay2D(TSender sender, Collider2D other);
        void OnCollisionEnter(TSender sender, Collision collision);
        void OnCollisionExit(TSender sender, Collision collision);
        void OnCollisionStay(TSender sender, Collision collision);
        void OnCollisionEnter2D(TSender sender, Collision2D collision);
        void OnCollisionExit2D(TSender sender, Collision2D collision);
        void OnCollisionStay2D(TSender sender, Collision2D collision);
    }
}