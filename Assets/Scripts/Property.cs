using System.Collections.Generic;
using UnityEngine;

namespace Property
{
    public enum PropertyType
    {
        SHAPE,
        COLOR,
    }

    public class Property
    {
        public PropertyType type;
        public string value;
    }
}