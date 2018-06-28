using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityTools
{
    public static bool isPointInViewport (Vector3 screenPoint) // returns true if point is inside viewport
    {

        if (screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f)
        {
            return true;
        }
        else
            return false;
    }

    public static bool isPointInViewport (Vector3[] points) // returns true if all points are visible
    {

        bool result = true;
        foreach (Vector3 p in points)
        {

            if (!(p.z > 0f && p.x > 0f && p.x < 1f && p.y > 0f && p.y < 1f))
            {

                result = false;
            }

        }

        return result;
    }

    public  static bool IsSameOrSubclass(System.Type potentialBase, System.Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
           || potentialDescendant == potentialBase;
    }

}