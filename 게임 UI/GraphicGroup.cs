using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace inonego.UI
{
    public class GraphicGroup : Graphic
    {
        [SerializeField]
        private Graphic[] graphics = null;

        public override Color color
        { 
            get => base.color;
            set
            {
                base.color = value;

                foreach (var graphic in graphics)
                {
                    graphic.color = value;
                }
            }
        }

        public override bool raycastTarget 
        { 
            get => base.raycastTarget; 
            set
            {
                base.raycastTarget = value;

                foreach (var graphic in graphics)
                {
                    graphic.raycastTarget = value;
                }
            }
        }

        public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
        {
            base.CrossFadeAlpha(alpha, duration, ignoreTimeScale);

            foreach (var graphic in graphics)
            {
                graphic.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
            }
        }

        public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
        {
            base.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);

            foreach (var graphic in graphics)
            {
                graphic.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
            }
        }

        public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
        {
            base.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB);

            foreach (var graphic in graphics)
            {
                graphic.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB);
            }
        }
    }
}