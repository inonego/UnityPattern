using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace inonego
{
    // ========================================================================
    /// <summary>
    /// 데이터 테이블을 브라우징하고 선택할 수 있는 에디터 윈도우입니다.
    /// </summary>
    // ========================================================================
    public class TableBrowserWindow : EditorWindow
    {

    #region 필드

        // ======= 데이터 및 타입 =======
        private Type valueType = null;
        private IReadOnlyTable browsingTable = null;
        private Action<object> onSelect = null;

        // ======= 검색 및 페이징 =======
        private string searchKey = "";
        private List<string> filteredKeys = new List<string>();
        
        private int currentPage = 0;
        private int itemsPerPage = 20;
        private int MaxPage => Mathf.Max(0, (filteredKeys.Count - 1) / itemsPerPage);

        // ======= 레이아웃 및 UI =======
        private MultiColumnHeader multiColumnHeader = null;
        private Vector2 scrollPosition;
        private string selectedKey = null;

        private bool hasName = false;

        // ======= 상수 =======
        private const float ROW_HEIGHT = 22f;
        private const float TOOLBAR_HEIGHT = 30f;
        private const double DOUBLE_CLICK_TIME = 0.3;
        private double lastClickTime = 0;
        private string lastClickedKey = null;

    #endregion

    #region 정적 메서드

        // --------------------------------------------------------------------
        /// <summary>
        /// 데이터 테이블 브라우저 윈도우를 표시합니다.
        /// </summary>
        /// <param name="onSelect">항목 선택 시 호출될 콜백</param>
        // --------------------------------------------------------------------
        public static void Show<TTableValue>(Action<TTableValue> onSelect = null)
            where TTableValue : class, ITableValue
        {
            if (DataPackage.Loaded == null)
            {
                EditorUtility.DisplayDialog("오류", "데이터 패키지가 로드되지 않았습니다.", "확인");

                return;
            }

            var lTitle = $"Table Browser: {typeof(TTableValue).Name}";
            var window = GetWindow<TableBrowserWindow>(true, lTitle, true);

            window.valueType = typeof(TTableValue);
            window.onSelect = onSelect != null ? (obj) => onSelect(obj as TTableValue) : null;
            
            window.Initialize();
            window.ShowModalUtility();
        }

    #endregion

    #region Unity 이벤트

        private void OnEnable()
        {
            Initialize();
        }

        private void OnGUI()
        {
            if (browsingTable == null)
            {
                EditorGUILayout.HelpBox("데이터를 로드할 수 없거나 테이블이 존재하지 않습니다.", UnityEditor.MessageType.Error);
                return;
            }

            using (new EditorGUILayout.VerticalScope())
            {
                // 상단 툴바
                DrawTopToolbar();
                
                // 테이블 영역 (남은 공간 모두 사용)
                float lTableHeight = position.height - TOOLBAR_HEIGHT * 2;
                Rect lTableRect = EditorGUILayout.GetControlRect(false, lTableHeight, GUILayout.ExpandWidth(true));
                DrawTable(lTableRect);
                
                // 하단 툴바
                DrawBottomToolbar();
            }
        }

    #endregion

    #region 초기화

        // --------------------------------------------------------------------
        /// <summary>
        /// 데이터 및 헤더 초기화
        /// </summary>
        // --------------------------------------------------------------------
        private void Initialize()
        {
            if (DataPackage.Loaded == null || valueType == null) return;

            try
            {
                browsingTable = DataPackage.Loaded.Table(valueType);
                hasName = typeof(IHasName).IsAssignableFrom(valueType);
                
                InitializeHeader();
                UpdateFilter();
            }
            catch
            {
                browsingTable = null;
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 멀티 컬럼 헤더 초기화
        /// </summary>
        // --------------------------------------------------------------------
        private void InitializeHeader()
        {
            var columns = new List<MultiColumnHeaderState.Column>();

            void AddColumn(string headerContent)
            {
                columns.Add
                (
                    new()
                    {
                        headerContent = new GUIContent(headerContent),
                        width = 200, minWidth = 100,
                        autoResize = true,
                        canSort = true
                    }
                );
            }

            if (hasName)
            {
                AddColumn("이름 (Name)");
            }

            AddColumn("키 (Key)");

            var state = new MultiColumnHeaderState(columns.ToArray());

            multiColumnHeader = new MultiColumnHeader(state);
            multiColumnHeader.ResizeToFit();
        }

    #endregion

    #region UI 그리기

        // --------------------------------------------------------------------
        /// <summary>
        /// 상단 툴바 (검색) 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawTopToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Height(TOOLBAR_HEIGHT)))
            {
                GUILayout.FlexibleSpace();
                DrawSearchField();
                GUILayout.Space(5);
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 하단 툴바 (페이징) 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawBottomToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Height(TOOLBAR_HEIGHT)))
            {
                GUILayout.Space(5);
                DrawCount();
                GUILayout.FlexibleSpace();
                DrawPageInput();
                GUILayout.Space(3);
                DrawPagingButtons();
                GUILayout.Space(5);
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 검색 필드 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawSearchField()
        {
            string searched = EditorGUILayout.TextField(searchKey, EditorStyles.toolbarSearchField, GUILayout.Width(250));
            if (searched != searchKey)
            {
                searchKey = searched;
                UpdateFilter();
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 페이징 버튼 그리기 (좌우 버튼 함께)
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawPagingButtons()
        {
            using (new EditorGUI.DisabledScope(currentPage <= 0))
            {
                if (GUILayout.Button("◀", EditorStyles.toolbarButton, GUILayout.Width(25)))
                    currentPage--;
            }

            using (new EditorGUI.DisabledScope(currentPage >= MaxPage))
            {
                if (GUILayout.Button("▶", EditorStyles.toolbarButton, GUILayout.Width(25)))
                    currentPage++;
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 전체 개수 표시
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawCount()
        {
            EditorGUILayout.LabelField($"{filteredKeys.Count}개", EditorStyles.miniLabel);
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 페이지 입력 및 표시
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawPageInput()
        {
            // 페이지 번호의 최대 자릿수에 맞춰 입력 필드 너비 계산
            int maxPageDigits = (MaxPage + 1).ToString().Length;
            int fieldWidth = Mathf.Max(20, maxPageDigits * 6);

            var GUIContent = new GUIContent($"/ {MaxPage + 1}");
            float labelWidth = GUI.skin.label.CalcSize(GUIContent).x;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                int lTargetPage = EditorGUILayout.IntField(currentPage + 1, EditorStyles.toolbarTextField, GUILayout.Width(fieldWidth)) - 1;
                if (lTargetPage != currentPage)
                {
                    currentPage = Mathf.Clamp(lTargetPage, 0, MaxPage);
                }

                EditorGUILayout.LabelField(GUIContent, EditorStyles.miniLabel, GUILayout.Width(labelWidth));
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 테이블 본문 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawTable(Rect rect)
        {
            DrawTableHeader(rect);
            DrawTableContent(rect);
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 테이블 헤더 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawTableHeader(Rect rect)
        {
            Rect headerRect = new Rect(rect.x, rect.y, rect.width, ROW_HEIGHT);
            multiColumnHeader.OnGUI(headerRect, 0);
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 테이블 내용 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawTableContent(Rect rect)
        {
            var pageKeys = filteredKeys.Skip(currentPage * itemsPerPage).Take(itemsPerPage).ToList();
            float headerWidth = CalculateHeaderWidth();
            float contentHeight = pageKeys.Count * ROW_HEIGHT;
            float availableHeight = rect.height - ROW_HEIGHT;
            
            Rect scrollRect = new Rect(rect.x, rect.y + ROW_HEIGHT, rect.width, availableHeight);
            
            // 스크롤이 필요한지 확인
            bool needsScroll = contentHeight > availableHeight;
            
            if (needsScroll)
            {
                // 스크롤이 필요한 경우
                Rect viewRect = new Rect(0, 0, Mathf.Max(headerWidth, scrollRect.width - 20), contentHeight);
                scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, viewRect);

                for (int i = 0; i < pageKeys.Count; i++)
                {
                    Rect rowRect = new Rect(0, i * ROW_HEIGHT, headerWidth, ROW_HEIGHT);
                    DrawRowInRect(rowRect, i, pageKeys[i]);
                }

                GUI.EndScrollView();
            }
            else
            {
                // 스크롤이 필요없는 경우 - 테이블이 하단 툴바 위까지 확장되도록
                for (int i = 0; i < pageKeys.Count; i++)
                {
                    Rect rowRect = new Rect(scrollRect.x, scrollRect.y + i * ROW_HEIGHT, headerWidth, ROW_HEIGHT);
                    DrawRowInRect(rowRect, i, pageKeys[i]);
                }
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 컬럼 헤더의 실제 너비 계산
        /// </summary>
        // --------------------------------------------------------------------
        private float CalculateHeaderWidth()
        {
            float width = 0;
            for (int i = 0; i < multiColumnHeader.state.columns.Length; i++)
            {
                width += multiColumnHeader.state.columns[i].width;
            }
            return width;
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 개별 행 그리기 (Rect 직접 지정)
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawRowInRect(Rect rowRect, int index, string key)
        {
            DrawRowBackground(rowRect, index, key);
            DrawRowColumns(rowRect, key);
            HandleRowEvents(rowRect, key);
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 행 배경색 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawRowBackground(Rect rowRect, int index, string key)
        {
            bool isSelected = selectedKey == key;

            if (isSelected)
            {
                EditorGUI.DrawRect(rowRect, new Color(0.24f, 0.48f, 0.9f, 0.4f));
            }
            else if (index % 2 == 1)
            {
                EditorGUI.DrawRect(rowRect, new Color(0, 0, 0, 0.05f));
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 행 컬럼 데이터 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawRowColumns(Rect rowRect, string key)
        {
            int colIndex = 0;
            
            if (hasName)
            {
                DrawNameColumn(rowRect, ref colIndex, key);
            }

            DrawKeyColumn(rowRect, colIndex, key);
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 이름 컬럼 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawNameColumn(Rect rowRect, ref int colIndex, string key)
        {
            Rect nameRect = multiColumnHeader.GetCellRect(colIndex++, rowRect);
            // 왼쪽 패딩 추가
            nameRect.x += 5;
            nameRect.width -= 5;
            ITableValue val = browsingTable[key];
            string nameStr = (val as IHasName)?.Name ?? "N/A";
            EditorGUI.LabelField(nameRect, nameStr, EditorStyles.label);
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 키 컬럼 그리기
        /// </summary>
        // --------------------------------------------------------------------
        private void DrawKeyColumn(Rect rowRect, int colIndex, string key)
        {
            Rect keyRect = multiColumnHeader.GetCellRect(colIndex, rowRect);

            // 왼쪽 패딩 추가
            keyRect.x += 5;
            keyRect.width -= 5;

            EditorGUI.LabelField(keyRect, key, EditorStyles.boldLabel);
        }

    #endregion

    #region 이벤트 처리

        // --------------------------------------------------------------------
        /// <summary>
        /// 클릭 및 더블 클릭 이벤트 핸들링
        /// </summary>
        // --------------------------------------------------------------------
        private void HandleRowEvents(Rect rowRect, string key)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && rowRect.Contains(e.mousePosition))
            {
                double clickTime = EditorApplication.timeSinceStartup;
                
                if (lastClickedKey == key && (clickTime - lastClickTime) < DOUBLE_CLICK_TIME)
                {
                    ConfirmSelection(key);
                }
                else
                {
                    selectedKey = key;
                    lastClickedKey = key;
                    lastClickTime = clickTime;
                }
                
                e.Use();
                Repaint();
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 최종 선택 및 윈도우 닫기
        /// </summary>
        // --------------------------------------------------------------------
        private void ConfirmSelection(string key)
        {
            if (browsingTable.Has(key))
            {
                var value = browsingTable[key];

                onSelect?.Invoke(value);
                
                Close();
                
                GUIUtility.ExitGUI();
            }
        }

    #endregion

    #region 데이터 처리

        // --------------------------------------------------------------------
        /// <summary>
        /// 검색 필터 업데이트
        /// </summary>
        // --------------------------------------------------------------------
        private void UpdateFilter()
        {
            if (browsingTable == null)
            {
                filteredKeys.Clear();
                return;
            }

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                filteredKeys = browsingTable.Keys.OrderBy(k => k).ToList();
            }
            else
            {
                filteredKeys.Clear();
                string lowerSearch = searchKey.ToLower();

                foreach (var (key, value) in browsingTable)
                {
                    if (Check(key, value, lowerSearch))
                    {
                        filteredKeys.Add(key);
                    }
                }

                filteredKeys = filteredKeys.OrderBy(k => k).ToList();
            }

            currentPage = 0;
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// 검색어와 일치하는지 확인
        /// </summary>
        // --------------------------------------------------------------------
        private bool Check(string key, ITableValue value, string lowerSearch)
        {
            bool Contains(string text, string lowerSearch)
            {
                if (text != null)
                {
                    return text.ToLower().Contains(lowerSearch);
                }

                return false;
            }

            if (Contains(key, lowerSearch))
            {
                return true;
            }

            if (value is IHasName hasName)
            {
                return Contains(hasName.Name, lowerSearch);
            }

            return false;
        }

    #endregion

    }
}

#endif