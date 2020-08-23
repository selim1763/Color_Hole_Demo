using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Vector3? ScreenPointToWorldPoint(Camera camera, Vector3 screenPoint, int layer, int distance = 300)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(screenPoint);

        if(Physics.Raycast(ray, out hit, distance, layer))
        {
#if UNITY_DEBUG
            Debug.DrawLine(camera.transform.position, hit.point, Color.red);
#endif
            return hit.point;
        }

        return null;
    }

}
