using UnityEngine;

public class DragCamera : MonoBehaviour
{
    public float dragSpeed = 20;
    public float zoomSpeed = 10.0f;
    public float zoomMin = 6f;
    public float zoomMax = 20f;

    public Vector2 minPos = new Vector2(22, 12);
    public Vector2 maxPos = new Vector2(112, 64);

    private Vector3 lastMousePosition;
    /**
X min:
11 - 6
38 - 20
    **/
    void Update()
    {
        Navigate();
    }

    private void Navigate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            SetNewPosition();
        }
        // On scroll zoom in and out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomMin, zoomMax);
            SetNewPosition();
        }
    }

    private void SetNewPosition()
    {
        Vector3 delta = Input.mousePosition - lastMousePosition;
        Vector3 position = Camera.main.ScreenToViewportPoint(lastMousePosition - Input.mousePosition);
        Vector3 move = new Vector3(position.x * dragSpeed, position.y * dragSpeed, 0);

        transform.Translate(move, Space.World);

        // Calculate current camera bounds
        float vertExtent = Camera.main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        // Calculate minX and maxX based on current zoom level
        float minX = minPos.x + horzExtent;
        float maxX = maxPos.x - horzExtent;
        float minY = minPos.y + vertExtent;
        float maxY = maxPos.y - vertExtent;
        
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;

        lastMousePosition = Input.mousePosition;
    }
}
