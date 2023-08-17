using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollingSize : MonoBehaviour
{
    private Camera cam;
    private float mouseVel = 0.3f;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 7;
    }

    void Update()
    {
        float newValue = cam.orthographicSize - Input.mouseScrollDelta.y * mouseVel;
        cam.orthographicSize = Mathf.Clamp(newValue, 2, 10);
    }


}
