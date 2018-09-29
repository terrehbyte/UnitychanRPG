using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTopDown2D : MonoBehaviour
{
    [SerializeField]
    Transform followTarget;

    public Vector3 followAxis = new Vector3(1, 1, 0);

    public float smoothFactor = 0.1f;
    public float maxSpeed = 1.0f;

    public float minDistance = 5.0f;    // min distance to start moving
    public float maxDistance = 10.0f;   // max distance away from target

    void LateUpdate()
    {
        MoreGizmos.DrawCircle(Vector3.Scale(transform.position, new Vector3(1,1,0)), Vector3.forward, minDistance, 9, Color.gray);
        MoreGizmos.DrawCircle(Vector3.Scale(transform.position, new Vector3(1,1,0)), Vector3.forward, maxDistance, 9, Color.red);

        // TODO: remove min
        // TODO: fix smoothing
        Vector3 velocity = Vector3.zero;

        Vector3 targetPosition = Vector3.Scale(followTarget.position, followAxis);  // 2D position for target
        Vector3 currentPosition = Vector3.Scale(transform.position, followAxis);    // 2D position for camera

        float currentDistance = Vector3.Distance(currentPosition, targetPosition);  // 2D distance

        // exit early if too close to act
        if(currentDistance < minDistance) { return; }

        float desiredDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        Vector3 clampedCameraPos = targetPosition + (currentPosition - targetPosition).normalized * Mathf.Min(currentDistance, maxDistance);
        Vector3 desiredPosition = targetPosition + (currentPosition - targetPosition).normalized * desiredDistance;

        Vector3 intermediaryPos = Vector3.SmoothDamp(clampedCameraPos, desiredPosition, ref velocity, smoothFactor, maxSpeed);

        Vector3 mask = Vector3.one - followAxis;
        transform.position = intermediaryPos + Vector3.Scale(transform.position, mask);
    }
}