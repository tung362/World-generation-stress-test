using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOcclusion : MonoBehaviour
{
    private Renderer TheRenderer;
    private MeshRenderer TheMeshRenderer;

    void Start()
    {
        TheRenderer = GetComponent<Renderer>();
        TheMeshRenderer = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        if (IsVisibleFrom(TheRenderer, Camera.main)) TheMeshRenderer.enabled = true;
        else TheMeshRenderer.enabled = false;
    }

    public static bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
