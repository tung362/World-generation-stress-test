using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthOfFieldCalculator : MonoBehaviour
{
    public GameObject FocusObject;
    //Get values from post processing controller depth of field
    public float MinFocusDistance = 1.77f;
    public float MaxFocusDistance = 3.03f;

    public GameObject Center;
    private Vector3 ElevationVelocity = Vector3.zero;

    private GameCamera CameraController;

    void Start()
    {
        CameraController = GetComponent<GameCamera>();
    }

    void LateUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            Center.transform.position = Vector3.SmoothDamp(Center.transform.position, new Vector3(Center.transform.position.x, hit.point.y, Center.transform.position.z), ref ElevationVelocity, 0.3f);
            float maxDistance = CameraController.CameraMinZoom - CameraController.CameraMaxZoom;
            float currentDistance = hit.distance - CameraController.CameraMaxZoom;
            FocusObject.transform.position = transform.position + (transform.forward * Mathf.Lerp(MinFocusDistance, MaxFocusDistance, currentDistance / maxDistance));
        }
    }
}
