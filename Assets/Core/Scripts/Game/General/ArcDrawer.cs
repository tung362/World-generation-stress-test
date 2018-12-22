using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ArcDrawer : MonoBehaviour
{
    public LineRenderer TheLineRenderer;

    public float Angle = 0;
    public float ArcOffset = 90;
    public int Segments = 50;
    public float Radius = 1;

    void Update()
    {
        if (!Application.isPlaying) UpdateArc();
    }

    void UpdateArc()
    {
        List<Vector3> points = new List<Vector3>();
        float angle = Angle - ArcOffset;
        float arcLength = (Angle + ArcOffset) - (Angle - ArcOffset);
        int MultipliedSegments = Mathf.CeilToInt(Segments * Radius);
        int totalSegments = Mathf.CeilToInt(MultipliedSegments * (ArcOffset / 180f));

        for (int i = 0; i <= totalSegments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            points.Add(new Vector3(x, transform.position.y, z));

            angle += (arcLength / totalSegments);
        }

        TheLineRenderer.positionCount = totalSegments + 1;
        TheLineRenderer.SetPositions(points.ToArray());
    }
}
