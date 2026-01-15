using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI;
using UnityEditor.IMGUI.Controls;

namespace inonego.Editor
{
    [Serializable]
    public class TypeFilter
    {
        public Type BaseType;
        public string Assembly;
        public string Namespace;
        
        public bool Group = true;
    }

    // ==============================================================
    /// <summary>
    /// 계층 구조로 타입을 선택할 수 있는 Dropdown 클래스입니다.
    /// </summary>
    // ==============================================================
    public class TypeAdvancedDropdown : AdvancedDropdown
    {
        private readonly TypeFilter filter;
        private readonly Action<Type> onSelected;

        public TypeAdvancedDropdown(AdvancedDropdownState state, TypeFilter filter, Action<Type> onSelected) : base(state)
        {
            this.filter = filter;
            this.onSelected = onSelected;
            this.minimumSize = new UnityEngine.Vector2(300, 400);
        }

        // -------------------------------------------------------------
        /// <summary>
        /// 루트 아이템을 구성합니다.
        /// </summary>
        // -------------------------------------------------------------
        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Select Type");

            // 모든 어셈블리에서 타입 수집
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            bool useGroup = filter == null || filter.Group;
            var flatTypes = !useGroup ? new List<Type>() : null;

            foreach (var assembly in assemblies)
            {
                string assemblyName = assembly.GetName().Name;

                // 어셈블리 필터
                bool checkAssembly = true;  
                if (filter != null && !string.IsNullOrEmpty(filter.Assembly))
                {
                    checkAssembly = !string.IsNullOrEmpty(assemblyName)
                    && assemblyName.Contains(filter.Assembly, StringComparison.OrdinalIgnoreCase);
                }

                if (!checkAssembly) continue;

                try
                {
                    // 타입 필터
                    bool IsValidType(Type lType) 
                    {
                        if (lType == null) return false;
                        
                        var isConcrete = !lType.IsAbstract && !lType.IsInterface;

                        var isAssignableFrom = filter == null || filter.BaseType == null || filter.BaseType.IsAssignableFrom(lType);

                        bool checkNamespace = true;
                        if (filter != null && !string.IsNullOrEmpty(filter.Namespace))
                        {
                            checkNamespace = !string.IsNullOrEmpty(lType.Namespace) 
                            && lType.Namespace.Contains(filter.Namespace, StringComparison.OrdinalIgnoreCase);
                        }

                        return isConcrete && isAssignableFrom && checkNamespace && lType.IsVisible;
                    }

                    Type[] allTypes;
                    try { allTypes = assembly.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { allTypes = e.Types; }

                    if (allTypes == null) continue;

                    var types = allTypes.Where(IsValidType).ToList();
                    if (types.Count == 0) continue;

                    if (useGroup)
                    {
                        var assemblyItem = new AdvancedDropdownItem(assemblyName);
                        root.AddChild(assemblyItem);

                        var namespaceGroups = types.GroupBy(lType => lType.Namespace ?? "Global Namespace")
                                                   .OrderBy(group => group.Key);

                        foreach (var group in namespaceGroups)
                        {
                            var nsItem = new AdvancedDropdownItem(group.Key);
                            assemblyItem.AddChild(nsItem);

                            var orderedTypes = group.OrderBy(t => t.Name);
                            foreach (var type in orderedTypes)
                            {
                                nsItem.AddChild(new TypeDropdownItem(type));
                            }
                        }
                    }
                    else
                    {
                        flatTypes.AddRange(types);
                    }
                }
                catch { }
            }

            // 평면 리스트 출력
            if (!useGroup && flatTypes != null)
            {
                var orderedTypes = flatTypes.OrderBy(t => t.Name);
                foreach (var type in orderedTypes)
                {
                    root.AddChild(new TypeDropdownItem(type));
                }
            }

            return root;
        }

        // -------------------------------------------------------------
        /// <summary>
        /// 타입을 선택했을 때 호출됩니다.
        /// </summary>
        // -------------------------------------------------------------
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            if (item is TypeDropdownItem lTypeItem)
            {
                onSelected?.Invoke(lTypeItem.Type);
            }
        }

        // ==============================================================
        /// <summary>
        /// 타입을 선택할 수 있는 드롭다운 아이템 클래스입니다.
        /// </summary>
        // ==============================================================
        private class TypeDropdownItem : AdvancedDropdownItem
        {
            public Type Type;

            public TypeDropdownItem(Type type) : base(type.Name)
            {
                Type = type;
            }
        }
    }
}

#endif