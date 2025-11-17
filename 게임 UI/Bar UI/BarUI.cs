using UnityEngine;
using UnityEngine.UI;

#if DOTWEEN
using DG.Tweening;
#endif

namespace inonego
{
    using Serializable;

    [ExecuteInEditMode]
    public class BarUI : MonoBehaviour
    {
        public enum BarDirection
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom,
        }

    #region 필드

        [SerializeField]
        private RangeValue<float> value;
        public RangeValue<float> Value => value;

        // ------------------------------------------------------------
        /// <summary>
        /// 현재 값이 범위 내에서 차지하는 비율입니다. (0.0 - 1.0)
        /// </summary>
        // ------------------------------------------------------------
        public float Ratio => (Value.Base - Value.Min) / (Value.Max - Value.Min);

        [Header("Animation")]
        [SerializeField] private TweenCurve changeCurve;

        [Header("UI")]
        [SerializeField] private Image ForeFillImage;
        [SerializeField] private Image BackFillImage;
        [SerializeField] private Image BackgroundImage;

        [Header("Direction")]
        [SerializeField] private BarDirection direction;
        public BarDirection Direction => direction;

        [Header("Color")]
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;

        private float lCurrentRatio = 0f;

    #if DOTWEEN
        private Tween lCurrentTween;
    #endif

    #endregion

    #region 초기화

        private void OnEnable()
        {
            value.OnBaseChange += OnBaseChange;
            value.Range.OnBaseChange += OnRangeChange;

            // 초기화
            UpdateBarInstantly();
        }

        private void OnDisable()
        {
            value.OnBaseChange -= OnBaseChange;
            value.Range.OnBaseChange -= OnRangeChange;
        }

    #endregion

    #region 이벤트 핸들러
    
        public void Refresh()
        {
            var ratio = Ratio;

        #if DOTWEEN
        
            if (Application.isPlaying)
            {  
                lCurrentTween.Kill();

                float Getter() => lCurrentRatio;
                void Setter(float value) => lCurrentRatio = value;

                void OnUpdate() => UpdateBar(lCurrentRatio, ratio);

                OnUpdate();

                lCurrentTween = DOTween.To(Getter, Setter, ratio, changeCurve.Duration)
                .SetDelay(changeCurve.Delay)
                .SetEase(changeCurve.Ease);
                
                lCurrentTween.onUpdate = OnUpdate;
            }
            else
            {
                // 에디터 모드에서는 DOTween이 작동하지 않으므로
                // 즉시 업데이트하도록 합니다.
                UpdateBarInstantly();
            }

        #else

            UpdateBarInstantly();
            
        #endif
        
        }

        private void OnBaseChange(object sender, ValueChangeEventArgs<float> args)
        {
            Refresh();
        }

        private void OnRangeChange(object sender, ValueChangeEventArgs<MinMax<float>> args)
        {
            Refresh();
        }

    #endregion

    #region 업데이트

        private void UpdateBar(float lCurrentRatio, float lTargetRatio)
        {
            if (lCurrentRatio < lTargetRatio)
            {
                // 증가
                SetFillRatio(ForeFillImage, 0, lCurrentRatio);
                SetFillRatio(BackFillImage, lCurrentRatio, lTargetRatio);
                if (BackFillImage != null) BackFillImage.color = positiveColor;
            }
            else
            {
                // 감소
                SetFillRatio(ForeFillImage, 0, lTargetRatio);
                SetFillRatio(BackFillImage, lTargetRatio, lCurrentRatio);
                if (BackFillImage != null) BackFillImage.color = negativeColor;
            }
        }

        private void UpdateBarInstantly()
        {
            var ratio = Ratio;

            if (ForeFillImage != null) ForeFillImage.color = defaultColor;
            if (BackFillImage != null) BackFillImage.color = defaultColor;
            
            lCurrentRatio = ratio;

            SetFillRatio(ForeFillImage, 0, ratio);
            SetFillRatio(BackFillImage, ratio, ratio);
        }

        private void SetFillRatio(Image image, float beginRatio, float endRatio)
        {
            if (image == null) return;

            var rectTransform = image.rectTransform;

            switch (direction)
            {
                case BarDirection.TopToBottom:
                    rectTransform.anchorMin = new Vector2(0, 1 - endRatio);
                    rectTransform.anchorMax = new Vector2(1, 1 - beginRatio);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    break;

                case BarDirection.BottomToTop:
                    rectTransform.anchorMin = new Vector2(0, beginRatio);
                    rectTransform.anchorMax = new Vector2(1, endRatio);
                    rectTransform.pivot = new Vector2(0.5f, 0);
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    break;

                case BarDirection.RightToLeft:
                    rectTransform.anchorMin = new Vector2(1 - endRatio, 0);
                    rectTransform.anchorMax = new Vector2(1 - beginRatio, 1);
                    rectTransform.pivot = new Vector2(1, 0.5f);
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    break;

                case BarDirection.LeftToRight:
                    rectTransform.anchorMin = new Vector2(beginRatio, 0);
                    rectTransform.anchorMax = new Vector2(endRatio, 1);
                    rectTransform.pivot = new Vector2(0, 0.5f);
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    break;
            }
        }

    #endregion

    }
}

