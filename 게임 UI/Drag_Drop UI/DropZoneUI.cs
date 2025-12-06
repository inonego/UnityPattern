using UnityEngine;
using UnityEngine.EventSystems;

using System;

namespace inonego.UI
{
    [Serializable]
    public struct DropEventArgs
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 드롭된 게임오브젝트
        /// </summary>
        // ------------------------------------------------------------
        public GameObject DroppedGO;

        // ------------------------------------------------------------
        /// <summary>
        /// 드롭된 DraggableUI 컴포넌트
        /// </summary>
        // ------------------------------------------------------------
        public DraggableUI DraggableUI;
    }

    // ========================================================================
    /// <summary>
    /// 드래그된 UI 요소를 받을 수 있는 드롭존 컴포넌트입니다.
    /// </summary>
    // ========================================================================
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class DropZoneUI : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {

    #region 필드

        [Header("설정")]
        public bool CanDrop = true;

        [Header("드롭 정보")]
        [SerializeField, ReadOnly]
        private bool isDropping = false;
        public bool IsDropping => isDropping;

        [SerializeField, ReadOnly]
        private DraggableUI draggable = null;
        public DraggableUI Draggable => draggable;

    #endregion

    #region 이벤트

        // ------------------------------------------------------------
        // 드롭 이벤트 정의
        // ------------------------------------------------------------
        public delegate void DropEventHandler(DropZoneUI sender, DropEventArgs e);

        public event DropEventHandler OnDropEnter = null;
        public event DropEventHandler OnDropExit = null;
        public event DropEventHandler OnDropDone = null;

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 드롭 가능 여부를 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        private bool CheckCanDrop(DraggableUI draggableUI)
        {
            if (draggableUI == null) return false;

            var specificDropZone = draggableUI.SpecificDropZone;

            var check = specificDropZone == null || specificDropZone == this;

            return draggableUI.IsDragging && draggableUI.CanDrop && CanDrop && check;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 드롭 이벤트 인자를 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        private DropEventArgs CreateDropEventArgs(DraggableUI draggableUI)
        {
            if (draggableUI == null) 
            {
                return new DropEventArgs();
            }

            return new DropEventArgs
            {
                DroppedGO = draggableUI.gameObject,
                DraggableUI = draggableUI,
            };
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 포인터가 드롭존에 들어올 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void _OnDropEnter(PointerEventData eventData)
        {
            if (eventData == null) return;
            if (isDropping) return;

            var pointerDrag = eventData.pointerDrag;
            if (pointerDrag == null) return;

            var draggable = pointerDrag.GetComponent<DraggableUI>();
            if (draggable == null) return;

            if (!CheckCanDrop(draggable)) return;

            // ------------------------------------------------------------
            // 드롭 정보 설정
            // ------------------------------------------------------------
            isDropping = true;

            this.draggable = draggable;

            // ------------------------------------------------------------
            // 드롭 진입 이벤트 발생
            // ------------------------------------------------------------
            var dropEventArgs = CreateDropEventArgs(draggable);

            OnDropEnter?.Invoke(this, dropEventArgs);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 드롭존에 드래그된 객체가 드롭될 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void _OnDrop(PointerEventData eventData)
        {
            if (eventData == null) return;
            if (!isDropping) return;

            var pointerDrag = eventData.pointerDrag;
            if (pointerDrag == null) return;
            
            if (draggable == null) return;

            if (pointerDrag != draggable.gameObject) return;

            if (!CheckCanDrop(draggable)) return;

            // ------------------------------------------------------------
            // 드롭 완료 이벤트 발생
            // ------------------------------------------------------------
            var dropEventArgs = CreateDropEventArgs(draggable);
            
            OnDropDone?.Invoke(this, dropEventArgs);

            // 드롭 이탈 이벤트 발생
            _OnDropExit(eventData);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 포인터가 드롭존에서 나갈 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void _OnDropExit(PointerEventData eventData)
        {
            if (eventData == null) return;
            if (!isDropping) return;
            
            // ------------------------------------------------------------
            // 드롭 이탈 이벤트 발생
            // ------------------------------------------------------------
            var dropEventArgs = CreateDropEventArgs(draggable);

            // ------------------------------------------------------------
            // 드롭 정보 설정
            // ------------------------------------------------------------
            isDropping = false;

            this.draggable = null;

            // 드롭 이탈 이벤트 발생
            OnDropExit?.Invoke(this, dropEventArgs);
        }
        
    #endregion

    #region 인터페이스 구현

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _OnDropEnter(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _OnDropExit(eventData);
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            _OnDrop(eventData);
        }

    #endregion

    }

}
