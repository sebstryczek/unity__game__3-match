using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsVector2
{
    public static float GetAngleBetween(Vector2 v1, Vector2 v2)
    {
        float angle = Mathf.Atan2(v1.y, v1.x) - Mathf.Atan2(v2.y, v2.x);

        if (angle < 0)
        {
            angle += Mathf.PI * 2;
        }

        return angle * Mathf.Rad2Deg;
    }
}
