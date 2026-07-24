using System;

using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public enum ShapeType
    {
        Square,
        Circle,
        Triangle,
    }
    
    [Serializable]
    public class ShapeProperty : IProperty
    {
        [SerializeField] public ShapeType shape;

        public ShapeProperty(ShapeType shape)
        {
            this.shape = shape;
        }
        
        public ShapeProperty()
        {
            shape = ShapeType.Square;
        }
        
        public bool Match(IProperty property)
        {
            return shape == (property as ShapeProperty)?.shape;
        }
        
        public override string ToString()
        {
            return $"Shape: {shape}";
        }
    }
}