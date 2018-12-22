using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DestroyOnTime : NetworkBehaviour
{
    public float Duration = 2;
    private float Timer = 0;

    void Update()
    {
        if (!isServer) return;
        Timer += Time.deltaTime;
        if (Timer >= Duration) Destroy(gameObject);
    }
}
