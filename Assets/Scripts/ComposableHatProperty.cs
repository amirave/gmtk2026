using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Game;

    
[Serializable]
public class ComposableHatProperty : IComposable
{
    [SerializeField] public List<HatType> possibleValues = new();
    [SerializeField] private GameObject hatPlace;

    public IProperty Compose()
    {
        var chosen = possibleValues[Random.Range(0, possibleValues.Count)];

        var chosenHat = chosen switch
        {
            HatType.Nothing => ResourceProvider.Instance.Nothing,
            HatType.BrimlessYanky => ResourceProvider.Instance.BrimlessYanky,
            HatType.Crown => ResourceProvider.Instance.Crown,
            _ => throw new Exception($"No pattern {chosen}")
        };

        Debug.Log($"Chosen: {chosenHat}");

        var spriteRenderer = hatPlace.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(chosenHat, new Rect(0, 0, chosenHat.width, chosenHat.height), new Vector2(0.5f, 0.5f), spriteRenderer.sprite.pixelsPerUnit);

        return new HatProperty();
    }
}