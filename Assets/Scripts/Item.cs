using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Item : MonoBehaviour
    {
        [SerializeReference] public List<Property> properties = new();

        public bool MatchProperty(Property property)
        {
            foreach (var currentProperty in properties)
            {
                if (currentProperty.Match(property)) return true;
            }

            return false;
        }
    }
}