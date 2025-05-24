
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StringInterpolation.Demo
{
    [System.Serializable]
    public class DemoWeapon : IRichStringCustomFormat
    {
        [field: SerializeField, RichReference] public float damage { get; private set; }
        [SerializeField, RichReference] float _fireRate;

        public string GetNormalForm()
        {
            return $"Damage: {damage}";
        }
        public string GetAlternateForm()
        {
            return $"Damage: {damage}, Fire Rate: {_fireRate}";
        }
    } 
}
