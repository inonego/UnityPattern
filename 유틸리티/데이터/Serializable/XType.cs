using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Serializable
{
    // ========================================================================
    /// <summary>
    /// 직렬화 가능한 Type입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public struct XType
    {
        [SerializeField]
        private string name;

        public Type Value => Type.GetType(name);

        public XType(Type value)
        {
            name = value.AssemblyQualifiedName;
        }

        public static implicit operator XType(Type value) => new(value);
        public static implicit operator Type(XType value) => value.Value;
    }
}