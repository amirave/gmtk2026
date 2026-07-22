using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ColorProperty : Property, IProperty
    {
        [SerializeField] private List<Color> possibleValues;
        [SerializeField] private SpriteRenderer target;

        public new void Compose()
        {
            target.color = (Color)Chosen;
        }
    }
}