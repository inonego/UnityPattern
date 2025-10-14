using System;

using UnityEngine;

namespace inonego.Pool
{
    // ===============================================================================
    /// <summary>
    /// <br/>유니티에서 사용하는 컴포넌트에 대하여 오브젝트 풀링을 할 수 있습니다.
    /// <br/>컴포넌트를 풀링하기 위해서는 해당 T 컴포넌트가 최상단에 포함된 프리팹이 필요합니다.
    /// </summary>
    // ===============================================================================
    [Serializable]
    public class GOCompPool<T> : GOPool where T : Component
    {

    }
}