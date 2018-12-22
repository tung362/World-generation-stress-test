using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestCircle : MonoBehaviour
{
    public LineRenderer TheLineRenderer;
    public float ThetaScale = 0.01f;
    public float radius = 3f;
    private int Size;
    private float Theta = 0f;

    void Update()
    {
        Theta = 0f;
        Size = (int)((1f / ThetaScale) + 1f);
        TheLineRenderer.SetVertexCount(Size);
        for (int i = 0; i < Size; i++)
        {
            Theta += (2.0f * Mathf.PI * ThetaScale);
            float x = radius * Mathf.Cos(Theta);
            float y = radius * Mathf.Sin(Theta);
            TheLineRenderer.SetPosition(i, new Vector3(x, 0, y));
        }
    }
}
