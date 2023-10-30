using System;
using TMPro;
using UnityEngine;

public class DragCamera : MonoBehaviour
{
    public float dragSpeed = 25;
    public float zoomSpeed = 10.0f;
    public float zoomMin = 10.0f;
    public float zoomMax = 15f;

    public Vector2 minPos = new(0, 0);
    public Vector2 maxPos = new(134, 76);

    private Vector3 lastMousePosition;

    #region Test
    public GameObject positionXText;
    public GameObject positionYText;
    public GameObject minXText;
    public GameObject maxXText;
    public GameObject minYText;
    public GameObject maxYText;
    #endregion

    public bool _canMove = true;
    void Update()
    {
        Navigate();
    }

    private void Navigate()
    {
        if (!_canMove)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if (IsMapClicked())
            {
                SetNewPosition();
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomMin, zoomMax);
            ClampPosition();
        }
    }

    private void SetNewPosition()
    {
        Vector3 position = Camera.main.ScreenToViewportPoint(lastMousePosition - Input.mousePosition);
        Vector3 move = new Vector3(position.x * dragSpeed, position.y * dragSpeed, 0);
        transform.Translate(move, Space.World);

        ClampPosition();
        lastMousePosition = Input.mousePosition;
    }

    private void ClampPosition()
    {
        // Calculate current camera bounds
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;


        float minX = minPos.x;
        float maxX = maxPos.x;
        float minY = minPos.y;
        float maxY = maxPos.y;


        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;
    }

    private bool IsMapClicked()
    {
        int layerMask = 1 << 8;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D raycastHit2D = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
        return raycastHit2D != null; // TODO: Fix
    }

    public void DisableMovement()
    {
        _canMove = false;
    }

    public void EnableMovement()
    {
        _canMove = true;
    }

}
