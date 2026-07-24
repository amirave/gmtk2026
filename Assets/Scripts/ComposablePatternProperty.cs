using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public enum PatternType
    {
        Plain,
        Striped,
        Dotted
    }
    
    [Serializable]
    public class ComposablePatternProperty : IComposable
    {
        [SerializeField] public List<PatternType> possibleValues = new();
        [SerializeField] private SpriteRenderer target;

        public IProperty Compose()
        {
            var chosen = possibleValues[Random.Range(0, possibleValues.Count)];

            var patternMat = chosen switch
            {
                PatternType.Plain => ResourceProvider.Instance.PlainMaterial,
                PatternType.Striped => ResourceProvider.Instance.StripedMaterial,
                PatternType.Dotted => ResourceProvider.Instance.DottedMaterial,
                _ => throw new Exception($"No pattern {chosen}")
            };

            target.material = patternMat;

            return new PatternProperty(chosen);
        }
    }
}