using AuraDev;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace StringInterpolation
{
    [CreateAssetMenu(fileName = "RichString/RichString Settings", menuName = "RichString Settings")]
    public class RichStringSettings : ScriptableObject
    {
        [field: Header("General Specifiers")]
        [field: SerializeField] public string enumerableIndex { get; private set; } = "->";
        [field: SerializeField] public string propertyRefernce { get; private set; } = ".";
        [field: SerializeField] public string richText { get; private set; } = ":";
        [Header("Rich Text Specifiers")]
        [SerializeField] string _bold = "b";
        [SerializeField] string _italic = "i";
        [SerializeField] string _underline = "u";
        [SerializeField] string _strikethrough = "s";
        [SerializeField] List<AKeyValuePair<string, Color>> _colorKeys = new();
        [field: Space]
        [field: SerializeField] public ErrorHandlingMode errorHandlingMode { get; private set; } = ErrorHandlingMode.Error;
        public bool throwExepction => errorHandlingMode == ErrorHandlingMode.Error;

        public List<AKeyValuePair<string, RichString.RichTextDelegate>> actionTable { get; private set; } = new();

        private void Awake()
        {
            Save();
        }

        public void Save()
        {
            actionTable.Clear();

            actionTable.Add(new AKeyValuePair<string, RichString.RichTextDelegate>(_bold, x => x.Bold()));
            actionTable.Add(new AKeyValuePair<string, RichString.RichTextDelegate>(_italic, x => x.Italic()));
            actionTable.Add(new AKeyValuePair<string, RichString.RichTextDelegate>(_underline, x => x.Underline()));
            actionTable.Add(new AKeyValuePair<string, RichString.RichTextDelegate>(_strikethrough, x => x.Strikethrough()));

            foreach (var colorKey in _colorKeys)
            {
                actionTable.Add(new AKeyValuePair<string, RichString.RichTextDelegate>(colorKey.Key, x => x.Colorize(colorKey.Value)));
            }
        }

        public enum ErrorHandlingMode
        {
            Error,
            Warning
        }
    }
    // A simple and serializable KeyValuePair
    [System.Serializable]
    public class AKeyValuePair<TKey, TValue>
    {
        [field: SerializeField] public TKey Key { get; set; }
        [field: SerializeField] public TValue Value { get; set; }

        public AKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(RichStringSettings))]
    public class CustomRichStringSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var localTarget = (RichStringSettings)target;

            //
        }
    }
#endif 
}
