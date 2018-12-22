using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestNavmeshBake : MonoBehaviour
{
    public string key = " ";

    private NavMeshSurface Surface;

    void Start()
    {
        Surface = GetComponent<NavMeshSurface>();
    }

	void Update ()
    {
		if(Input.GetKeyDown(key))
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Surface.Bake();
            stopwatch.Stop();
            Debug.Log("Time taken: " + (stopwatch.Elapsed));
            stopwatch.Reset();
        }
	}
}
