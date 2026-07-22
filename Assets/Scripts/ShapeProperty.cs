using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public enum ShapeType
    {
        SQUARE,
        CIRCLE,
        TRIANGLE,
    }
    
    [Serializable]
    public class ShapeProperty : Property, IProperty
    {
        [SerializeField] public ShapeType Shape;
        [SerializeField] public Sprite Sprite;

        public void Compose() { }

        public bool Match(Property other)
        {
            if (other.GetType() != typeof(ShapeProperty)) return false;
            
            var otherShape = (ShapeProperty)other;
            return Shape == otherShape.Shape;
        }
    }
}