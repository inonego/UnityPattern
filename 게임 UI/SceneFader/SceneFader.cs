using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

#if DOTWEEN
using DG.Tweening;
#endif

namespace inonego.UI
{
    public enum FadeType
    {
        In, Out,
    }

    public class SceneFader : MonoSingleton<SceneFader>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitSceneFader()
        {
            // 캔버스 생성 및 설정
            var canvasGo = new GameObject("SceneFaderCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();

            DontDestroyOnLoad(canvasGo);

            // SceneFader 게임 오브젝트 생성
            var faderGo = new GameObject("SceneFader");
            faderGo.transform.SetParent(canvasGo.transform);

            // Image 컴포넌트 추가 및 화면 채우기 설정
            var image = faderGo.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
            image.raycastTarget = false;

            // SceneFader 컴포넌트 추가
            faderGo.AddComponent<SceneFader>();

            // RectTransform을 이용해 화면 전체 꽉 차게 설정
            var rect = faderGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        
        private Image image;

        private Tween lFadeTween;

        protected override void Awake()
        {
            base.Awake();

            image = GetComponent<Image>();
        }

        public void Color(Color color)
        {
            image.color = color.A(0f);
        }

    #if DOTWEEN

        public void Fade(FadeType type, TweenCurve curve, Action onComplete = null)
        {
            lFadeTween?.Kill();

            var alpha = type == FadeType.In ? 1f : 0f;

            lFadeTween = image.DOFade(alpha, curve.Duration).SetDelay(curve.Delay).SetEase(curve.Ease);

            void OnComplete()
            {
                onComplete?.Invoke();

                lFadeTween = null;
            }

            lFadeTween.onComplete = OnComplete;
        }

    #endif

    }
}