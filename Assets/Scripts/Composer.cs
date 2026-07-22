#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class Composer : MonoBehaviour
    {
        [SerializeField] private List<Item> _itemsPrefabs;
        [SerializeReference] private List<Property> _restrictions;

        public void Create(List<Property> restrictions)
        {
            var possibleItems = new List<Item>();
            
            Item? chosenItem = null;
            foreach (var item in _itemsPrefabs)
            {
                var match = false;
                foreach (var restriction in restrictions)
                {
                    chosenItem = item;
                 
                    match = item.PossibleMatchProperty(restriction);
                    if (!match)
                    {
                        break;
                    }
                }

                if (!match) continue;
                if (chosenItem != null) possibleItems.Add(chosenItem);
            }

            var pickedItem = possibleItems.Count > 0 ? possibleItems[Random.Range(0, possibleItems.Count)] : throw new Exception("No item match!");
            var instance = Instantiate(pickedItem);
            Compose(instance);
        }

        private static void Compose(Item item)
        {
            foreach (var property in item.properties)
            {
                if (property.isComposable)
                {
                    property.Compose();
                }
                else
                {
                    property.ComposeDefault();
                }
            }
            
            item.properties.ForEach(property => property.Render());
        }
    }
}