using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IProperty
    {
        public bool Match(IProperty property);
        public string Name();
    }
}