using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CamController : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public float distance = 10.0f; // The initial distance from the target
    public float zoomSpeed = 2.0f; // The speed of zooming
    public float rotationSpeed = 100.0f; // The speed of rotation

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float minYAngle = -60.0f; // Minimum vertical angle
    private float maxYAngle = 60.0f; // Maximum vertical angle

    void Start()
    {
        // Initialize the camera position
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void Update()
    {
        // Rotate the camera based on input
        if (Input.GetMouseButton(1)) // Right mouse button held
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle); // Clamp vertical rotation
        }

        // Zoom the camera based on input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, 2.0f, 15.0f); // Clamp zoom distance

        // Calculate the new position and rotation of the camera
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        Vector3 position = rotation * direction + target.position;

        // Apply the position and rotation to the camera
        transform.position = position;
        transform.LookAt(target.position);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }
}
