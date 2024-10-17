using AuraDev;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AuraDev
{
    [CreateAssetMenu(fileName = "RichString Settings", menuName = "RichString Settings")]
    public class RichStringSettings : ScriptableObject
    {
        [field: Header("General Specifiers")]
        [field: SerializeField] public string enumerableIndex { get; private set; } = "->";
        [field: SerializeField] public string propertyRefernce { get; private set; } = ".";
        [field: SerializeField] public string richText { get; private set; } = ":";
        [Header("Rich Text Specifiers")]
        [SerializeField] string _bold = "b";
        [SerializeField] string _italic = "i";
        [SerializeField] List<AKeyValuePair<string, Color>> _colorKeys = new();

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

            foreach (var colorKey in _colorKeys)
            {
                actionTable.Add(new AKeyValuePair<string, RichString.RichTextDelegate>(colorKey.Key, x => x.Colorize(colorKey.Value)));
            }
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

            EditorGUILayout.HelpBox("Make sure to hit the \"Save\" button after you've changed the properties.", MessageType.Info);
            if (GUILayout.Button("Save")) localTarget.Save();
        }
    }
#endif 
}