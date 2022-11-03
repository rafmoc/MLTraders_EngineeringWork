using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CameraControl : MonoBehaviour
{
    public int xMax = 0, xMin = 0, yMax = 0, yMin = 0;
    public float speed = 200.0f;

    private bool buttonDownFlag = false;

    private float minFov = 20f;
    private float maxFov = 400f;
    private float sensitivity = 50f;

    private Vector3 hitPosition = Vector3.zero;
    private Vector3 currentPosition = Vector3.zero;
    private Vector3 cameraPosition = Vector3.zero;
    private Vector3 target_position;

    private void Update()
    {
        Scroll();
        Move();
    }

    private void Scroll()
    {
        float fieldOfView = Camera.main.orthographicSize;
        fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fieldOfView = Mathf.Clamp(fieldOfView, minFov, maxFov);
        Camera.main.orthographicSize = fieldOfView;
    }

    private void Move()
    {
        if (Input.GetMouseButtonDown(1))
        {
            hitPosition = Input.mousePosition;
            cameraPosition = transform.position;
        }

        if (Input.GetMouseButton(1))
        {
            currentPosition = Input.mousePosition;
            LeftMouseDrag();
            buttonDownFlag = true;
        }

        if (buttonDownFlag)
        {
            transform.position = Vector3.MoveTowards(transform.position, target_position, Time.deltaTime * speed);
            if (transform.position == target_position)
            {
                buttonDownFlag = false;
            }
        }
    }

    void LeftMouseDrag()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(currentPosition) - Camera.main.ScreenToWorldPoint(hitPosition);
        target_position = cameraPosition + (direction * -1);
        target_position.x = Mathf.Clamp(target_position.x, xMin, xMax);
        target_position.y = Mathf.Clamp(target_position.y, yMin, yMax);

    }
}
