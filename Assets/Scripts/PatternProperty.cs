using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class PatternProperty : IProperty
    {
        [SerializeField] private PatternType _patternType;
        
        public PatternType PatternType => _patternType;
        
        public PatternProperty(PatternType patternType)
        {
            _patternType = patternType;
        }

        public PatternProperty()
        {
            _patternType = PatternType.Plain;
        }

        public bool Match(IProperty property)
        {
            return PatternType == (property as PatternProperty)?.PatternType;
        }
    }
}