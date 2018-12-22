using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseShow : MonoBehaviour
{
    /*Data*/
    private bool StopUpdatingVisiblity = false;

    void LateUpdate()
    {
        if (StopUpdatingVisiblity) StopUpdatingVisiblity = false;
        else
        {
            //Shows mouse
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void DisableUpdateVisibility(bool TrueFalse)
    {
        StopUpdatingVisiblity = TrueFalse;
    }
}
