using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace inonego.UI
{
    using Serializable;

    [Flags]
    public enum MouseButton : int
    {
        Left    = 1 << 0,
        Right   = 1 << 1,
        Middle  = 1 << 2,
    }
    
    [Serializable]
    public struct DragEventArgs
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 드래그 시작 위치
        /// </summary>
        // ------------------------------------------------------------
        public Vector2 Origin;

        // ------------------------------------------------------------
        /// <summary>
        /// 마우스 위치와 UI 위치 간의 오프셋
        /// </summary>
        // ------------------------------------------------------------
        public Vector2 Offset;

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 UI 위치
        /// </summary>
        // ------------------------------------------------------------
        public Vector2 Point;

        // ------------------------------------------------------------
        /// <summary>
        /// 목표 위치
        /// </summary>
        // ------------------------------------------------------------
        public Vector2 Target;
    }

    // ========================================================================
    /// <summary>
    /// UI 요소를 드래그할 수 있게 해주는 컴포넌트입니다.
    /// </summary>
    // ========================================================================
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class DraggableUI : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

    #region 정적 관리

        private static HashSet<DraggableUI> activeCollection = new();
        public static IReadOnlyCollection<DraggableUI> ActiveCollection => activeCollection;

        public static event Action OnActiveCollectionChange = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnRuntimeInitializeOnLoad()
        {
            activeCollection.Clear();
        }

    #endregion

    #region 필드

        [Header("설정")]
        public bool CanMove = true;
        public bool CanDrop = true;
        public bool RaycastTarget = false;

        public MouseButton Button = MouseButton.Left;

        [Header("드롭존 정보")]
        public DropZoneUI SpecificDropZone = null;

        [Header("드래그 정보")]
        public bool IsDragging => current != null;
        private PointerEventData current = null;

        [SerializeField, ReadOnly]
        private XNullable<Vector2> origin = null;
        public Vector2? Origin => origin;

        [SerializeField, ReadOnly]
        private XNullable<Vector2> offset = null;
        public Vector2? Offset => offset;
        
        private bool originalBlocksRaycasts = true;

    #endregion

    #region 컴포넌트

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

    #endregion

    #region 이벤트

        public delegate void DragEventHandler(DraggableUI sender, DragEventArgs e);

        public event DragEventHandler OnDragBegin = null;
        public event DragEventHandler OnDrag = null;
        public event DragEventHandler OnDragEnd = null;
        
    #endregion

    #region 유니티 이벤트

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (IsDragging)
            {
                _OnDrag(current);
            }
        }

        private void OnDisable()
        {
            ForceDragEnd();
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// PointerEventData.InputButton을 MouseButton으로 변환합니다.
        /// </summary>
        // ------------------------------------------------------------
        private MouseButton ConvertToMouseButton(PointerEventData.InputButton button)
        {
            return button switch
            {
                PointerEventData.InputButton.Left   => MouseButton.Left,
                PointerEventData.InputButton.Right  => MouseButton.Right,
                PointerEventData.InputButton.Middle => MouseButton.Middle,
                _ => 0,
            };
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 지정된 버튼이 드래그를 허용하는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        private bool IsButtonAllowed(PointerEventData.InputButton button)
        {
            MouseButton mouseButton = ConvertToMouseButton(button);

            return (Button & mouseButton) != 0;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 화면 좌표를 Canvas 로컬 좌표로 변환합니다.
        /// </summary>
        // ------------------------------------------------------------
        private Vector2 ScreenToLocalPoint(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            return localPoint;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 마우스 위치와 오프셋을 기반으로 목표 위치를 계산합니다.
        /// </summary>
        // ------------------------------------------------------------
        private Vector2 CalculateTarget(Vector2 localPoint)
        {
            return localPoint + (offset.HasValue ? offset.Value : Vector2.zero);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 드래그 이벤트 인자를 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        private DragEventArgs CreateDragEventArgs(Vector2 localPoint)
        {
            return new DragEventArgs
            {
                Origin = origin.HasValue ? origin.Value : Vector2.zero,
                Offset = offset.HasValue ? offset.Value : Vector2.zero,
                Point = rectTransform.anchoredPosition,
                Target = CalculateTarget(localPoint),
            };
        }
        
    #endregion

    #region 이벤트 핸들러

        // ------------------------------------------------------------
        /// <summary>
        /// 드래그를 시작할 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void _OnDragBegin(PointerEventData eventData)
        {
            if (eventData == null) return;

            if (!activeCollection.Contains(this))
            {
                activeCollection.Add(this);
                OnActiveCollectionChange?.Invoke();
            }

            originalBlocksRaycasts = canvasGroup.blocksRaycasts;

            if (!RaycastTarget)
            {
                canvasGroup.blocksRaycasts = false;
            }

            Vector2 localPoint = ScreenToLocalPoint(eventData);

            // ------------------------------------------------------------
            // 드래그 정보 설정
            // ------------------------------------------------------------
            offset = rectTransform.anchoredPosition - localPoint;
            origin = rectTransform.anchoredPosition;

            current = eventData;
            
            eventData.pointerDrag = gameObject;

            // ------------------------------------------------------------
            // 드래그 이벤트 발생
            // ------------------------------------------------------------
            var dragEventArgs = CreateDragEventArgs(localPoint);
            
            OnDragBegin?.Invoke(this, dragEventArgs);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 드래그 중일 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void _OnDrag(PointerEventData eventData)
        {
            if (eventData == null) return;

            // Canvas 좌표계로 변환
            Vector2 localPoint = ScreenToLocalPoint(eventData);

            if (CanMove)
            {
                rectTransform.anchoredPosition = CalculateTarget(localPoint);
            }

            // ------------------------------------------------------------
            // 드래그 이벤트 발생
            // ------------------------------------------------------------
            var dragEventArgs = CreateDragEventArgs(localPoint);
            
            OnDrag?.Invoke(this, dragEventArgs);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 드래그를 종료할 때 호출됩니다.
        /// </summary>
        // ------------------------------------------------------------
        private void _OnDragEnd(PointerEventData eventData)
        {
            if (eventData == null) return;

            if (activeCollection.Contains(this))
            {
                activeCollection.Remove(this);
                OnActiveCollectionChange?.Invoke();
            }

            canvasGroup.blocksRaycasts = originalBlocksRaycasts;

            Vector2 localPoint = ScreenToLocalPoint(eventData);

            // ------------------------------------------------------------
            // 드래그 이벤트 발생
            // ------------------------------------------------------------
            var dragEventArgs = CreateDragEventArgs(localPoint);
            
            // ------------------------------------------------------------
            // 드래그 정보 설정
            // ------------------------------------------------------------
            origin = null;
            offset = null;
            
            eventData.pointerDrag = null;
            
            current = null;

            // 드래그 이벤트 발생
            OnDragEnd?.Invoke(this, dragEventArgs);
        }

    #endregion

    #region 강제 드래그 이벤트 발생

        // ------------------------------------------------------------
        /// <summary>
        /// 강제로 드래그를 종료합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void ForceDragEnd()
        {
            _OnDragEnd(current);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 마우스 위치와 부모를 기준으로 드래그 오프셋을 재계산합니다.
        /// 드래그 도중에 부모가 변경된 경우 호출해야 합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void RefreshDragOffset()
        {
            if (!IsDragging) return;

            Vector2 localPoint = ScreenToLocalPoint(current);
            offset = rectTransform.anchoredPosition - localPoint;
        }

    #endregion

    #region 인터페이스 구현

        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            // 드래그 시작 전에 버튼 체크
            if (!IsButtonAllowed(eventData.button))
            {
                // 드래그를 시작하지 않도록 설정
                eventData.pointerDrag = null;
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                _OnDragBegin(eventData);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {           
            // 업데이트에서 처리하도록 합니다.
            // 이 구현은 유지하여야 Drop이 작동됩니다.
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (IsDragging)
            {
                _OnDragEnd(eventData);
            }
        }

    #endregion


    }

}
