using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public static partial class Utils 
{
    public static bool OverlapRange(float pos1, float width1, float pos2, float width2)
    {
        var x2 = pos1 + 0.5f * width1;
        var x1 = pos1 - 0.5f * width1;
        
        var y2 = pos2 + 0.5f * width2;
        var y1 = pos2 - 0.5f * width2;

        return x1 <= y2 && y1 <= x2;
    }

    public static bool Overlap2D(Vector3 pos1, Vector3 size1, Vector3 pos2, Vector3 size2)
    {
        return OverlapRange(pos1.x, size1.x, pos2.x, size2.x) &&
               OverlapRange(pos1.y, size1.y, pos2.y, size2.y);
    }
    
    public static string FormatScore(float scoreF, int zeroes, Color color)
    {
        var score = (int)scoreF;
        var len = score.ToString().Length;

        var final = new string('0', zeroes - len) + "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + score;
        return final;
    }
    
    public static string DisplayWithSuffix(this int num)
    {
        var number = num.ToString();
        if (number.EndsWith("11")) return number + "th";
        if (number.EndsWith("12")) return number + "th";
        if (number.EndsWith("13")) return number + "th";
        if (number.EndsWith("1")) return number + "st";
        if (number.EndsWith("2")) return number + "nd";
        if (number.EndsWith("3")) return number + "rd";
        return number + "th";
    }
    
    public static T PickRandom<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    
    public static T PickRandom<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    public static float Round(float value, int digits = 0)
    {
        var mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
    
    public static int GetRandomWeightedIndex(float[] weights)
    {
        // Get the total sum of all the weights.
        var weightSum = weights.Sum();

        // Step through all the possibilities, one by one, checking to see if each one is selected.
        var index = 0;
        var lastIndex = weights.Length - 1;
        while (index < lastIndex)
        {
            // Do a probability check with a likelihood of weights[index] / weightSum.
            if (Random.Range(0, weightSum) < weights[index])
            {
                return index;
            }
 
            // Remove the last item from the sum of total untested weights and try again.
            weightSum -= weights[index++];
        }
 
        // No other item was selected, so return very last index.
        return index;
    }

    // Generate a random point along one of the lines of a given rectangle
    public static Vector2 RandomOnPerimeter(Vector2 rect)
    {
        var i = GetRandomWeightedIndex(new[] { rect.x, rect.y, rect.x, rect.y });
        
        return i switch
        {
            0 => new Vector2(Random.Range(-rect.x, rect.x), rect.y),
            1 => new Vector2(rect.x, Random.Range(-rect.y, rect.y)),
            2 => new Vector2(Random.Range(-rect.x, rect.x), -rect.y),
            3 => new Vector2(-rect.x, Random.Range(-rect.y, rect.y)),
            _ => Vector2.zero
        };
    }
    
    public static Color LerpHSV(Color a, Color b, float t, bool shortest = true)
    {
        // convert from RGB to HSV
        Color.RGBToHSV(a, out var ahue, out var asat, out var aval);
        Color.RGBToHSV(b, out var bhue, out var bsat, out var bval);

        float hue;

        if ((Mathf.Abs(ahue - bhue) < 0.5f && shortest) || (Mathf.Abs(ahue - bhue) > 0.5f && !shortest))
            hue = Mathf.LerpUnclamped(ahue, bhue, t);
        else if (ahue > bhue)
            hue = Mathf.LerpUnclamped(ahue, bhue + 1, t) % 1;
        else
            hue = Mathf.LerpUnclamped(ahue + 1, bhue, t) % 1;

        var sat = Mathf.LerpUnclamped(asat, bsat, t);
        var val = Mathf.LerpUnclamped(aval, bval, t);

        // convert back to RGB and return the color
        return Color.HSVToRGB(hue, sat, val);
    }

    public static Quaternion ShortestRotation(Quaternion from, Quaternion to)
    {
        if (Quaternion.Dot(to, from) < 0)
            return to * Quaternion.Inverse(MultiplyQuaternionByScalar(from, -1));
        
        return to * Quaternion.Inverse(from);
    }
    
    public static Quaternion MultiplyQuaternionByScalar(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    // Makes sure a rotation is the "short way round"
    // https://theorangeduck.com/page/joint-limits#unrolling
    public static Quaternion AbsoluteRotation(Quaternion q)
    {
        return new Quaternion(q.x, q.y, q.z, Mathf.Abs(q.w));
    }
    
    public static float DistFromLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        var lineDirection = lineEnd - lineStart;
        var lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        var toPoint = point - lineStart;
        var dot = Vector3.Dot(toPoint, lineDirection);
        if (dot <= 0)
        {
            return (point - lineStart).magnitude;
        }
        else if (dot >= lineLength)
        {
            return (point - lineEnd).magnitude;
        }
        else
        {
            var projection = lineStart + dot * lineDirection;
            return (point - projection).magnitude;
        }
    }

    public static (Vector3 center, float size) CalculateOrthoSize(Camera cam, List<Collider> colliders)
    {
        var bounds = new Bounds();

        foreach (var col in colliders)
            bounds.Encapsulate(col.bounds);

        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * cam.pixelHeight / cam.pixelWidth;

        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var center = bounds.center + new Vector3(0, 0, -10);

        return (center, size);
    }

    public static float RoundToDecimal(float num, int dec)
    {
        var pow = Mathf.Pow(10, dec);
        return Mathf.Round(num * pow) / pow;
    }
    
    public static float Remap(this float value, float from1, float to1, float from2, float to2) 
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static (int, float) SplitIntAndFrac(float value)
    {
        return ((int)value, value % 1);
    }
    
    public static Vector2 RandomDirection()
    {
        var radians = Random.Range(0, 2 * Mathf.PI);
        return VectorFromAngle(radians);
    }

    public static Vector2 VectorFromAngle(float radians)
    {
        return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
    }
    
    public static float Angle(Vector2 v)
    {
        if (v.x < 0)
            return 2 * Mathf.PI - (Mathf.Atan2(v.x, v.y) * -1);
        
        return Mathf.Atan2(v.x, v.y);
    }

    public static Vector3 Sum<T>(this IEnumerable<T> list, Func<T, Vector3> func)
    {
        var sum = Vector3.zero;
        foreach (var item in list)
        {
            sum += func(item);
        }

        return sum;
    }
    
    public static byte PackBitfield(bool[] bools)
    {
        byte result = 0;
        for (var i = 0; i < Math.Min(8, bools.Length); i++)
        {
            if (bools[i])
                result |= (byte)(1 << i);
        }
        return result;
    }
    
    public static bool[] UnpackBitfield(byte b)
    {
        return Enumerable.Range(0, 8)
            .Select(i => (b & (1 << i)) != 0)
            .ToArray();
    }
    
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
    {
        if (dictionary.TryGetValue(key, out TValue value))
        {
            return value;
        }
        return defaultValue;
    }
    
    public static T OrNull<T> (this T obj) where T : UnityEngine.Object => obj ? obj : null;
    
    public static float AtMost(this float value, float max)
    {
        return value > max ? max : value;
    }
        
    public static float AtLeast(this float value, float min)
    {
        return value < min ? min : value;
    }
}
