using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Item : MonoBehaviour
    {
        [SerializeReference] private List<Property> properties = new();
    }
}