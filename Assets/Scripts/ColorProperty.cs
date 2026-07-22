using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum ColorType
    {
        RED,
        GREEN,
        BLUE
    }
    
    [Serializable]
    public class ColorProperty : Property, IProperty
    {
        [SerializeField] private List<ColorType> possibleValues;
        [SerializeField] private SpriteRenderer target;

        public new void Compose()
        {
            switch ((ColorType)Chosen)
            {
                case ColorType.RED:
                    target.color = Color.red;
                    break;
                case ColorType.GREEN:
                    target.color = Color.green;
                    break;
                case ColorType.BLUE:
                    target.color = Color.blue;
                    break;
            }
        }

        public bool Match(Property other)
        {
            if (other.GetType() != typeof(ColorProperty)) return false;
            
            var otherColor = (ColorProperty)other;
            return otherColor.Chosen == Chosen;
        }
    }
}