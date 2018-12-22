using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class ConnectionCheck : MonoBehaviour
{
    /*Data*/
    private bool RunOnce = true;

    /*Callable Functions*/
    public UnityEvent OnToggle;

    /*Required Components*/
    private NetworkManager TheNetworkManager;

    void Start()
    {
        TheNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        UpdateConnection();
    }

    void UpdateConnection()
    {
        if (!TheNetworkManager.IsClientConnected())
        {
            if(RunOnce)
            {
                OnToggle.Invoke();
                RunOnce = false;
            }
        }
        else RunOnce = true;
    }
}
