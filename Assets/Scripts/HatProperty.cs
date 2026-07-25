using System;
using Game;
using UnityEngine;

public enum HatType
{
    Nothing,
    BrimlessYanky,
    Crown,
}

[Serializable]
public class HatProperty : IProperty
{
    [SerializeField] private HatType _hatType;
    
    public HatType HatType => _hatType;
    
    public HatProperty(HatType colorType)
    {
        _hatType = colorType;
    }

    public HatProperty()
    {
        _hatType = HatType.Nothing;
    }

    public bool Match(IProperty property)
    {
        return HatType == (property as HatProperty)?.HatType;
    }

    public override string ToString()
    {
        return $"Color: {_hatType}";
    }

    public string Name()
    {
        return HatType.ToString();
    }
}