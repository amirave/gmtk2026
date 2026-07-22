using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IProperty
    {
        public void ComposeDefault();
        public void Compose();
        public bool Match(Property other);
        public bool PossibleMatch(Property other);
        public void Render();
    }
    
    [Serializable]
    public abstract class Property : IProperty
    {
        [SerializeField] public bool isComposable;
        [NonSerialized] public object Chosen;

        public virtual void ComposeDefault()
        {
            throw new NotImplementedException();
        }

        public virtual void Compose()
        {
            throw new NotImplementedException();
        }

        public virtual void Render()
        {
            throw new NotImplementedException();
        }

        public virtual bool PossibleMatch(Property other)
        {
            throw new NotImplementedException();
        }

        public bool Match(Property other)
        {
            if (other.GetType() != GetType()) return false;
            return Chosen == other.Chosen;
        }
    }
}