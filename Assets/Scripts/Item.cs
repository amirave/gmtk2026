using System;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class Item : MonoBehaviour
    {
        [SerializeReference] private List<Property.Property> properties = new();
    }
}