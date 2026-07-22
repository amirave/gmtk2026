using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class Item : MonoBehaviour
    {
        [SerializeReference] public List<Property> properties = new();
        
        public List<Property> Properties => properties;

        public bool MatchProperty(Property property)
        {
            foreach (var currentProperty in properties)
            {
                if (currentProperty.Match(property)) return true;
            }

            return false;
        }
        
        public bool PossibleMatchProperty(Property property)
        {
            return properties.Any(currentProperty => currentProperty.PossibleMatch(property));
        }
    }
}