using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace inonego
{
    public static partial class Utility
    {
        // --------------------------------------------------------------------- 
        /// <summary>
        /// 문자열 배열의 각 요소를 Trim하고, 유효한 요소들만 남기도록합니다.
        /// </summary>
        // --------------------------------------------------------------------- 
        public static string[] TrimAndRemoveInvalid(this string[] source, out int length)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source가 null입니다.");
            }

            int writeIndex = 0; // 유효한 문자열을 쓸 위치

            for (int i = 0; i < source.Length; i++)
            {
                string entry = source[i]?.Trim();

                if (!string.IsNullOrEmpty(entry))
                {
                    source[writeIndex] = entry;

                    writeIndex++;
                }
            }

            // 남은 부분을 null로 채움
            for (int i = writeIndex; i < source.Length; i++)
            {
                source[i] = null;
            }

            length = writeIndex;

            return source;
        }
        
        // ------------------------------------------------------------
        /// <summary>
        /// 정수를 서수형(1st, 2nd, 3rd, 4th 등)으로 변환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static string ToOrdinal(this int number)
        {
            if (number <= 0) return number.ToString();

            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            // 11, 12, 13은 특별한 경우
            if (11 <= lastTwoDigits && lastTwoDigits <= 13)
            {
                return number + "th";
            }

            // 마지막 자리수에 따른 처리
            return lastDigit switch
            {
                1 => number + "st",
                2 => number + "nd", 
                3 => number + "rd",
                _ => number + "th"
            };
        }

        private static readonly int[] romanValues = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
        private static readonly string[] romanNumerals = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

        // ------------------------------------------------------------
        /// <summary>
        /// 정수를 로마숫자로 변환합니다. (1-3999 범위)
        /// </summary>
        // ------------------------------------------------------------
        public static string ToRoman(this int number)
        {
            if (number <= 0 || number > 3999) number.ToString();

            StringBuilder result = new StringBuilder();
            
            // 로마숫자 값과 해당 문자들을 내림차순으로 정의
            for (int i = 0; i < romanValues.Length; i++)
            {
                int count = number / romanValues[i];
                
                if (count > 0)
                {
                    // 해당 로마숫자를 count만큼 추가
                    for (int j = 0; j < count; j++)
                    {
                        result.Append(romanNumerals[i]);
                    }

                    number %= romanValues[i];
                }
            }

            return result.ToString();
        }
    }
}