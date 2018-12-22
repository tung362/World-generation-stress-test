using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TransformSync : NetworkBehaviour
{
    [SyncVar]
    public Vector3 ServerPosition;
    [SyncVar]
    public Quaternion ServerRotation;

    void Start ()
    {
        if (isServer)
        {
            ServerPosition = transform.position;
            ServerRotation = transform.rotation;
        }
        else
        {
            transform.position = ServerPosition;
            transform.rotation = ServerRotation;
        }
    }
	
	void Update ()
    {
		if(isServer)
        {
            ServerPosition = transform.position;
            ServerRotation = transform.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ServerPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, ServerRotation, 0.1f);
        }
	}
}
