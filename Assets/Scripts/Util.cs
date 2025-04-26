using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static Vector3 ToVector3(Vector2 argVector2)
    {
        return new Vector3(argVector2.x, argVector2.y, 0);
    }
    
    public static Vector2 ToVector2(Vector3 argVector3)
    {
        return new Vector2(argVector3.x, argVector3.y);
    }
    
    public static T GetNextEnum<T>(T current) where T : Enum
    {
        T[] values = (T[])Enum.GetValues(typeof(T));
        int index = Array.IndexOf(values, current);
        index = (index + 1) % values.Length; // Loop back to start if exceeding last index
        return values[index];
    }

    public static int GetSumHeldNumberKeys()
    {
        int sum = 0;
        
        for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
        {
            if (Input.GetKey(key))
            {
                sum += (key - KeyCode.Alpha0);
            }
        }

        return sum;
    }
}
