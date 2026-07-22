using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Utils
{
    public static Vector3 ToVector3(this Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }
        
    public static Vector2 ClampMagnitude(this Vector2 v, float min, float max)	
    {	
        var sm = v.sqrMagnitude;	
        if(sm > max * max) return v.normalized * max;	
        if(sm < min * min) return v.normalized * min;	
        return v;	
    }	

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        var tx = v.x;
        var ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Vector2 Inverse(this Vector2 v)
    {
        return v / v.sqrMagnitude;
    }
    
    public static Vector2 To(this Vector2 start, Vector2 end)
    {
        return end - start;
    }
}
