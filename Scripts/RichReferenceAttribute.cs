using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

using UnityEngine.Windows;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AuraDev
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    /// <summary>
    /// Attribute for drawing properties in the Unity Inspector with a custom label that displays the 
    /// original property name for runtime referencing within RichString expressions.
    /// </summary>
    public class RichReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="showCopyButton">Determines whether a copy button is displayed alongside the property in the Inspector,
        /// allowing easy copying of the reference's path.</param>
        /// <param name="richReferenceDraw">Determines whether a reference format for RichString expressions is displayed in the Inspector,
        /// helping users understand how to reference the property in RichString syntax.</param>
        public RichReferenceAttribute(bool showCopyButton = true, RichReferenceDrawType richReferenceDraw = RichReferenceDrawType.DontDraw)
        {
            this.showCopyButton = showCopyButton;
            this.richReferenceDraw = richReferenceDraw;
        }

        /// <summary>
        /// Determines whether a copy button is displayed alongside the property in the Inspector,
        /// allowing easy copying of the reference's path.
        /// </summary>
        public bool showCopyButton { get; set; } = true;

        /// <summary>
        /// Determines whether a reference format for RichString expressions is displayed in the Inspector,
        /// helping users understand how to reference the property in RichString syntax.
        /// </summary>
        public RichReferenceDrawType richReferenceDraw { get; set; } = RichReferenceDrawType.DontDraw;

        
    }
    public enum RichReferenceDrawType
    {
        DontDraw,
        Replace,
        Append
    }
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(RichReferenceAttribute), true)]
    public class RichReferenceAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetAttr = attribute as RichReferenceAttribute;

            GUIContent richGui = new GUIContent();
            string actualPropertyName = Regex.Replace(property.name, @"^<(.+)>k__BackingField$", "$1");

            string richLabel = string.Empty;

            switch (targetAttr.richReferenceDraw)
            {
                case RichReferenceDrawType.DontDraw:
                    richLabel = property.displayName;
                    break;
                case RichReferenceDrawType.Replace:
                    richLabel = actualPropertyName;
                    break;
                case RichReferenceDrawType.Append:
                    richLabel = $"{property.displayName} (\"{actualPropertyName}\")";
                    break;
                default:
                    richLabel = property.displayName;
                    break;
            }

            richGui.text = richLabel;
            richGui.tooltip = $"For referencing this into your RichString Expression, you have to use \"{actualPropertyName}\"";

            if (targetAttr.showCopyButton)
            {
                // Calculate rects for property field and button
                Rect propertyRect = new Rect(position.x, position.y, position.width - 50, 20);
                Rect buttonRect = new Rect(position.x + position.width - 50, position.y, 50, 20);

                // Draw the property field
                EditorGUI.PropertyField(propertyRect, property, richGui, true);

                if (GUI.Button(buttonRect, "Copy"))
                {
                    // Copy to clipboard and update button state
                    
                    GUIUtility.systemCopyBuffer = GetCorrectPropertyPath(property.propertyPath);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, richGui, true);
            }
        }
        
        private bool IsPropertyArray(SerializedProperty property)
        {
            string variableName = property.propertyPath;
            SerializedProperty p = property.serializedObject.FindProperty(variableName);

            return p.isArray;
        }

        private string GetCorrectPropertyPath(string propertyPath)
        {
            string cleanPath = propertyPath
                        .Replace("<", "")
                        .Replace(">", "")
                        .Replace("k__BackingField.", ".")
                        .Replace("k__BackingField", "");

            string result = cleanPath;

            if (propertyPath.Contains("Array"))
            {
                result = Regex.Replace(cleanPath, @"\.Array\.data\[(\d+)\]", $"{RichString.sharedSettings.enumerableIndex}$1");
            }

            return $"{{{result}}}";
        }
    }
#endif 
}
