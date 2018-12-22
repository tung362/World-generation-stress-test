using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavMeshVolumeBake : MonoBehaviour
{
    public GameObject Target;
    private NavMeshSurface Surface;

    void Start()
    {
        Surface = GetComponent<NavMeshSurface>();
    }

    void Update ()
    {
        transform.position = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);

        Surface.Bake();
    }
}
