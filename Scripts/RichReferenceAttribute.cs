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
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
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
        /// allowing easy copying of the reference.</param>
        /// <param name="showRichReferenceForm">Determines whether a reference format for RichString expressions is displayed in the Inspector,
        /// helping users understand how to reference the property in RichString syntax.</param>
        public RichReferenceAttribute(bool showCopyButton = true, bool showRichReferenceForm = true)
        {
            this.showCopyButton = showCopyButton;
            this.showRichReferenceForm = showRichReferenceForm;
        }

        /// <summary>
        /// Determines whether a copy button is displayed alongside the property in the Inspector,
        /// allowing easy copying of the reference.
        /// </summary>
        public bool showCopyButton { get; set; } = true;

        /// <summary>
        /// Determines whether a reference format for RichString expressions is displayed in the Inspector,
        /// helping users understand how to reference the property in RichString syntax.
        /// </summary>
        public bool showRichReferenceForm { get; set; } = true;
    }
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(RichReferenceAttribute))]
    public class RichReferenceAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetAttr = attribute as RichReferenceAttribute;

            GUIContent richLabel = new GUIContent();
            string actualPropertyName = Regex.Replace(property.name, @"^<(.+)>k__BackingField$", "$1");

            string richRefForm = targetAttr.showRichReferenceForm ? $" (\"{actualPropertyName}\")" : string.Empty;

            richLabel.text = $"{property.displayName}{richRefForm}";
            richLabel.tooltip = $"For referencing this into your RichString Expression, you have to use \"{actualPropertyName}\"";

            if (targetAttr.showCopyButton)
            {
                // Calculate rects for property field and button
                Rect propertyRect = new Rect(position.x, position.y, position.width - 50, 20);
                Rect buttonRect = new Rect(position.x + position.width - 50, position.y, 50, 20);

                // Draw the property field
                EditorGUI.PropertyField(propertyRect, property, richLabel, true);

                if (GUI.Button(buttonRect, "Copy"))
                {
                    // Copy to clipboard and update button state
                    GUIUtility.systemCopyBuffer = actualPropertyName;
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, richLabel, true);
            }
        }
    }
#endif 
}
