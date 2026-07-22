using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Utils
{
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 SetX(this Vector3 v, float x)
    {
        v.x = x;
        return v;
    }
    
    public static Vector3 SetY(this Vector3 v, float y)
    {
        v.y = y;
        return v;
    }
    
    public static Vector3 SetZ(this Vector3 v, float z)
    {
        v.z = z;
        return v;
    }

    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    
    public static Vector2 yz(this Vector3 v)
    {
        return new Vector2(v.y, v.z);
    }
    
    public static Vector2 xz(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
    
    public static Vector3 ClampMagnitude(this Vector3 v, float min, float max)	
    {	
        var sm = v.sqrMagnitude;	
        if(sm > max * max) return v.normalized * max;	
        if(sm < min * min) return v.normalized * min;	
        return v;	
    }	

    public static Vector3 Rotate(this Vector3 v, float degrees)	
    {	
        return Quaternion.AngleAxis(degrees, Vector3.up) * v;	
    }
    
    public static Vector3 Inverse(this Vector3 v)
    {
        return v / v.sqrMagnitude;
    }
    
    public static Vector3 Abs(this Vector3 v)
    {
        return ApplyFunction(v, Mathf.Abs);
    }

    public static Vector3 ApplyFunction(this Vector3 v, Func<float, float> func)
    {
        return new Vector3(func(v.x), func(v.y), func(v.z));
    }

    public static Vector3 To(this Vector3 start, Vector3 end)
    {
        return end - start;
    }
}
