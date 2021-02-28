// https://answers.unity.com/questions/827105/smooth-2d-camera-zoom.html
// https://forum.unity.com/threads/camera-dragging-with-mouse.468765/

using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{
    public float zoomSpeed = 1;
    public float targetOrtho;
    public float smoothSpeed = 2.0f;
    public float minOrtho = 1.0f;
    public float maxOrtho = 20.0f;

    private float dist = -10;
    private Vector3 mouseStart, mouseMove;
    private Vector3 derp;

    void Start()
    {
        targetOrtho = Camera.main.orthographicSize;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            targetOrtho -= scroll * zoomSpeed;
            targetOrtho = Mathf.Clamp(targetOrtho, minOrtho, maxOrtho);
        }

        Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);
        
        if (Input.GetMouseButtonDown(1))
        {
            mouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
        }
        else if (Input.GetMouseButton(1))
        {
            mouseMove = new Vector3(Input.mousePosition.x - mouseStart.x, Input.mousePosition.y - mouseStart.y, dist);
            mouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);

            Vector3 tempPosition = new Vector3(transform.position.x - mouseMove.x * Time.deltaTime, transform.position.y - mouseMove.y * Time.deltaTime, dist);

            // If possible should try to find a way to clamp this to bounds

            transform.position = tempPosition;
        }
    }
}