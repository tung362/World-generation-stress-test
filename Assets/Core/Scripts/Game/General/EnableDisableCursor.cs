using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableCursor : MonoBehaviour
{
    public bool ShowCursorOnStart = true;

	void Start()
    {
		if(ShowCursorOnStart)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
	}
}
