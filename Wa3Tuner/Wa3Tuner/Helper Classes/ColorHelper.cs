using System;
using System.Collections.Generic;
using System.Numerics;

public static class ColorHelper
{
    public static Vector3 FindClosestColor(Vector3 inputColor, List<Vector3> colorList)
    {
        if (colorList == null || colorList.Count == 0)
            throw new ArgumentException("Color list must not be null or empty.");

        Vector3 closest = colorList[0];
        float minDistance = Vector3.Distance(inputColor, closest);

        for (int i = 1; i < colorList.Count; i++)
        {
            float distance = Vector3.Distance(inputColor, colorList[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = colorList[i];
            }
        }

        return closest;
    }


}
