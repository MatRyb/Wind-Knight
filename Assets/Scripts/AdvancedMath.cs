using UnityEngine;

public static class AdvancedMath
{
    /// <summary>
    /// Return angle between line (origin -> point1) and line (origin -> point2)
    /// </summary>
    /// <param name="originPoint">Origin Poitn of both lines</param>
    /// <param name="point1">point of first line</param>
    /// <param name="point2">point of second line</param>
    /// <returns>eural angle in float value</returns>
    public static float GetAngleBetweenPoints(Vector2 originPoint, Vector2 point1, Vector2 point2)
    {
        return GetAngleOfLine(originPoint, point1) - GetAngleOfLine(originPoint, point2);
    }

    private static float GetAngleOfLine(Vector2 originPoint, Vector2 lineEndPoint)
    {
        float deltaX = lineEndPoint.x - originPoint.x;
        float deltaY = lineEndPoint.y - originPoint.y;
        float r = Vector2.Distance(lineEndPoint, originPoint);
        float degree;
        if (deltaY > 0)
            degree = Mathf.Acos(deltaX / r) * 180f / Mathf.PI;
        else
            degree = -Mathf.Acos(deltaX / r) * 180f / Mathf.PI;

        return degree;
    }
}
