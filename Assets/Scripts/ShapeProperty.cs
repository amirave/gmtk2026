using System;

using UnityEngine;

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

        public ShapeProperty()
        {
            isComposable = false;
        }

        public override void Compose()
        {
            Chosen = Shape.ToString();
        }
        public override void ComposeDefault()
        {
            Compose();
        }

        public override void Render() { }
        public override bool PossibleMatch(Property other)
        {
            return (other as ShapeProperty)?.Shape == Shape;
        }
    }
}