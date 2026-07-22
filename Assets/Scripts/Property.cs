using System;
using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    public interface IProperty
    {
        public void Compose();
    }
    
    [Serializable]
    public abstract class Property : IProperty
    {
        [SerializeField] public bool isComposable;
        [NonSerialized] public object Chosen;

        public void Compose()
        {
            throw new NotImplementedException();
        }
    }
    
}