using AuraDev;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class DemoInventory : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textComp;
    [field: SerializeField] public RichString description { get; private set; }

    [field: Header("Demo")]
    [field: SerializeField] public int MaxSlots { get; set; } = 10;
    [field: SerializeField, RichReference] public List<DemoWeapon> weapons { get; private set; }
    [field: SerializeField]
    [field: RichReference(richReferenceDraw = RichReferenceDrawType.Append)]
    public DemoWeapon mainWeapon { get; private set; }

    public void Refresh()
    {
        _textComp.text = description.GetParsedString();
    }
}
[CustomEditor(typeof(DemoInventory))]
public class CustomDemoDescriptionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var targetMono = (DemoInventory)target;

        if (GUILayout.Button("Refresh RichString"))
        {
            targetMono.Refresh();
        }
    }
}