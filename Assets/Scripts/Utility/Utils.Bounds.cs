using UnityEngine;

public static partial class Utils
{
    public static Bounds Scale(this Bounds bounds, Vector3 scale)
    {
        return new Bounds(Vector3.Scale(bounds.center, scale), Vector3.Scale(bounds.size, scale));
    }

    public static Bounds EncapsulateAndReturnBounds(this Bounds bounds, Bounds other)
    {
        bounds.Encapsulate(other);
        return bounds;
    }
}