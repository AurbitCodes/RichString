using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StringInterpolation.Demo
{
    public class ParseStringToggle : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _textComp;

        private DemoInventory _demoDesc;
        private void Start()
        {
            _demoDesc = GetComponent<DemoInventory>();

            _demoDesc.description.Initialize(_demoDesc);
            SwitchParsedString(true);
        }
        public void SwitchParsedString(bool parsed)
        {
            _textComp.text = parsed ? _demoDesc.description.GetParsedString() : _demoDesc.description.Expression;
        }
    } 
}
