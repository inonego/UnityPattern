using System;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace inonego
{
    public static class StackTraceUtility
    {
        // StackTrace 내 "in ...Assets/...cs:라인" 찾는 Regex
        private static readonly Regex regex = new Regex(@"in (.*Assets.*\.cs):(\d+)", RegexOptions.Compiled);

        // -------------------------------------------------------------
        /// <summary>
        /// e.StackTrace를 Unity Console에서 클릭 가능한 <a> 링크로 변환해서 출력합니다.
        /// </summary>
        // -------------------------------------------------------------
        public static string ReplaceWithHyperlink(string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                return stackTrace;
            }

            // \n으로 나누고 각 줄에 대해 처리 (Windows 환경의 \r\n 대응을 위해 \r 제거)
            var lines = stackTrace.Split('\n');
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd('\r');
                var match = regex.Match(line);

                if (match.Success)
                {
                    string fullPath = match.Groups[1].Value.Replace("\\", "/");
                    int idx = fullPath.IndexOf("Assets");
                    if (idx >= 0) fullPath = fullPath.Substring(idx);

                    string lineNumber = match.Groups[2].Value;

                    // <a href> 링크로 변환 (Unity 스타일: (at path:line))
                    string hyperlink = $"<a href=\"{fullPath}\" line=\"{lineNumber}\">{fullPath}:{lineNumber}</a>";
                    builder.Append(line.Replace(match.Value, $"(at {hyperlink})"));
                }
                else
                {
                    builder.Append(line);
                }

                // 마지막 줄이 아니면 줄바꿈 추가
                if (i < lines.Length - 1)
                {
                    builder.Append('\n');
                }
            }

            return builder.ToString();
        }
    }
}
