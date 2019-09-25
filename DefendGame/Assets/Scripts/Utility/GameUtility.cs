using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtility{

	public static Vector3 Vector2StrToVector3(string vector2Str)
    {
        // convert vector2 string to vector3
        if (vector2Str.Length > 2 && vector2Str[0] == '(' && vector2Str[vector2Str.Length - 1] == ')')
        {
            string pureVector3Str = vector2Str.Substring(1, vector2Str.Length - 2);
            string[] values = pureVector3Str.Split(',');
            if (values.Length == 2)
            {
                return new Vector3(float.Parse(values[0]), 0, float.Parse(values[1]));
            }
        }
        return new Vector3();
    }

    public static Vector3 Vector3StrToVector3(string vector3Str)
    {
        // convert vector3 string to vector3
        if (vector3Str[0] == '(' && vector3Str[vector3Str.Length - 1] == ')')
        {
            string pureVector3Str = vector3Str.Substring(1, vector3Str.Length - 2);
            string[] values = pureVector3Str.Split(',');
            if (values.Length == 3)
            {
                return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
            }
        }
        return new Vector3();
    }

    public static Vector3 GetRoundVector3(Vector3 vector)
    {
        // get round vector3
        return new Vector3(Mathf.Floor(vector.x), Mathf.Floor(vector.y), Mathf.Floor(vector.z));
    }
}
