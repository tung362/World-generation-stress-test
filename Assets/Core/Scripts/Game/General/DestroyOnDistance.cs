using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyOnDistance : NetworkBehaviour
{
    public float Distance = 7;
    private Vector3 StartingPosition = Vector3.zero;

    void Start()
    {
        StartingPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!isServer) return;
        if (Vector3.Distance(StartingPosition, transform.position) >= Distance) Destroy(gameObject);
    }
}
