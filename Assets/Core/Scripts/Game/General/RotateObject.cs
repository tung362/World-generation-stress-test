using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 Direction = Vector3.zero;
    public float Speed = 0;
    public bool CanRun = true;

    void Update()
    {
        UpdateRotation();
    }

    void UpdateRotation()
    {
        if(CanRun) transform.Rotate(Direction * Speed * Time.deltaTime);
    }

    public void Toggle(bool TrueFalse)
    {
        CanRun = TrueFalse;
    }
}
