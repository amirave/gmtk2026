using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public enum ColorType
    {
        Red,
        Green,
        Blue
    }
    
    [Serializable]
    public class ComposableColorProperty : IComposable
    {
        [SerializeField] public List<ColorType> possibleValues = new();
        [SerializeField] private SpriteRenderer target;

        public IProperty Compose()
        {
            var chosen = possibleValues[Random.Range(0, possibleValues.Count)];

            target.color = chosen switch
            {
                ColorType.Red => ResourceProvider.Instance.RedColor,
                ColorType.Green => ResourceProvider.Instance.GreenColor,
                ColorType.Blue => ResourceProvider.Instance.BlueColor,
                _ => target.color
            };

            return new ColorProperty(chosen);
        }
    }
}