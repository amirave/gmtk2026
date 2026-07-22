using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IProperty
    {
        public void Compose();
        public void Pick(List<Property> restrictions);
        public bool Match(Property other);
    }
    
    [Serializable]
    public abstract class Property : IProperty
    {
        [SerializeField] public bool isComposable;
        [NonSerialized] public object Chosen;

        public void Compose() { }
        public void Pick(List<Property> restrictions) { }
        public virtual bool Match(Property other)
        {
            throw new NotImplementedException();
        }
    }
}