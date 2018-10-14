using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: create generic binding system that provides Vector3s (or anything, like UnityEvent)
// TODO: fix/remove smoothing

public class CameraTopDown2D : MonoBehaviour
{
    [System.Serializable]
    public class CameraTarget
    {
        public Transform target;
        public float weight = 1.0f;
    }

    public Vector3 followAxis = new Vector3(1, 1, 0);
    public List<CameraTarget> followTargets; 

    [Header("Camera Settings")]
    public Transform origin;

    public float smoothFactor = 0.1f;
    public float maxSpeed = 1.0f;

    public float maxDistance = 10.0f;   // max distance away from target

    private Vector3 GetCameraTarget()
    {
        Vector3 candidate = Vector3.zero;

        foreach(var target in followTargets)
        {
            candidate += (target.target.position - origin.position) * target.weight;
        }

        candidate /= followTargets.Count + 1; // add one for the origin

        return origin.position + candidate;
    }

    private void LateUpdate()
    {
        MoreGizmos.DrawCircle(Vector3.Scale(transform.position, new Vector3(1,1,0)), Vector3.forward, maxDistance, 9, Color.red);

        Vector3 targetPosition = Vector3.Scale(GetCameraTarget(), followAxis);      // 2D position for target
        Vector3 currentPosition = Vector3.Scale(transform.position, followAxis);    // 2D position for camera

        float currentDistance = Vector3.Distance(currentPosition, targetPosition);  // 2D distance

        Vector3 clampedCameraPos = targetPosition + (currentPosition - targetPosition).normalized * Mathf.Min(currentDistance, maxDistance);
        Vector3 desiredPosition = targetPosition;

        MoreGizmos.DrawCircle(desiredPosition, Vector3.forward, 0.5f);

        Vector3 velocity = Vector3.zero;
        Vector3 intermediaryPos = Vector3.SmoothDamp(clampedCameraPos, desiredPosition, ref velocity, smoothFactor, maxSpeed);

        Vector3 mask = Vector3.one - followAxis; // get one's complement for mask to extra unaffected variables
        transform.position = intermediaryPos + Vector3.Scale(transform.position, mask);
    }
}