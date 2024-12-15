using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RichString
{
    public static class RichStringExtensions
    {
        public static string Bold(this string value)
        {
            return $"<b>{value}</b>";
        }
        public static string Italic(this string value)
        {
            return $"<i>{value}</i>";
        }
        public static string Strikethrough(this string value)
        {
            return $"<s>{value}</s>";
        }
        public static string Underline(this string value)
        {
            return $"<u>{value}</u>";
        }
        /// <summary>
        /// Returns the colorized text.
        /// </summary>
        /// <param name="value">The string to be colorized.</param>
        /// <param name="colorId">The color identifier that could be a color name from the supported tags or color hex.</param>
        /// <returns></returns>
        public static string Colorize(this string value, string colorId)
        {
            return $"<color={colorId}>{value}</color>";
        }
        public static string Colorize(this string value, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{value}</color>";
        }
        
    }
}
