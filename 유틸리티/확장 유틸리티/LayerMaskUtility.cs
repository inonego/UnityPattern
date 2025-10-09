using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    public static class LayerMaskUtility
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 특정 레이어가 레이어 마스크에 포함되어 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 게임 오브젝트가 레이어 마스크에 포함되는지 확인합니다.    
        /// </summary>
        // ------------------------------------------------------------
        public static bool Contains(this LayerMask mask, GameObject obj)
        {
            return mask.Contains(obj.layer);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 레이어 마스크에 레이어 추가합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static LayerMask AddLayer(this LayerMask mask, int layer)
        {
            return mask | (1 << layer);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 레이어 마스크에서 레이어를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static LayerMask RemoveLayer(this LayerMask mask, int layer)
        {
            return mask & ~(1 << layer);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 단일 레이어를 레이어 마스크로 변환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static LayerMask ToLayerMask(this int layer)
        {
            return 1 << layer;
        }
    }
}