using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeReference] public List<IProperty> _properties = new();
    [SerializeReference] public List<IComposable> _composableProperties = new();

    private List<IProperty> Properties => _properties;
    private List<IComposable> ComposableProperties => _composableProperties;

    public Item Compose()
    {
        foreach (var composableProperty in ComposableProperties)
        {
            var property = composableProperty.Compose();
            _properties.Add(property);
        }

        return this;
    }

    public bool Match(IProperty property)
    {
        return Properties.Any(currentProperty => currentProperty.Match(property));
    }
}