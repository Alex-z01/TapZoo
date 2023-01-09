using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region Properties
    public Transform CameraPivot;

    [SerializeField] private float zoomSpeed = 1f;
    private float minZoom = 2.75f;
    private float maxZoom = 15f;

    [SerializeField] private float panSpeed;
    private Vector3 mouseOrigin;
    private bool isPanning;
    #endregion

    private void Update()
    {
        Pan();

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll != 0)
        {
            Zoom(scroll * zoomSpeed);
        }
    }

    void Pan()
    {
        if(Input.GetMouseButtonDown(1))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        if(!Input.GetMouseButton(1))
        {
            isPanning = false;
            return;
        }

        if(isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);

            CameraPivot.position += move;
        }
    }

    void Zoom(float zoomDelta)
    {
        // Calculate the new zoom level
        float newZoom = Camera.main.orthographicSize - zoomDelta;

        // Clamp the zoom level between the min and max values
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

        // Set the cam's orthographic size to the new zoom level
        Camera.main.orthographicSize = newZoom;
    }

}
