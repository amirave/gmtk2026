using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ColorProperty : IProperty
    {
        [SerializeField] private ColorType _colorType;
        
        public ColorType ColorType => _colorType;
        
        public ColorProperty(ColorType colorType)
        {
            _colorType = colorType;
        }

        public ColorProperty()
        {
            _colorType = ColorType.Red;
        }

        public bool Match(IProperty property)
        {
            return ColorType == (property as ColorProperty)?.ColorType;
        }

        public override string ToString()
        {
            return $"Color: {_colorType}";
        }

        public string Name()
        {
            return ColorType.ToString();
        }
    }
}