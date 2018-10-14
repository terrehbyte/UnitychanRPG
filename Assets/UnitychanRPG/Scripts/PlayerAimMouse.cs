using UnityEngine;
using UnityEngine.UI;

public class PlayerAimMouse : MonoBehaviour, IAimable
{
    [Header("Dependencies")]
    [SerializeField]
    public Camera playerCamera; // TODO: inject from player? interface?

    [Header("Cursor Settings")]
    public bool constrainToScreen = true;
    public bool trackOutsideScreen = false;
    // TODO: support constraining rendering to radius around player for controllers

    private Vector2 screenDimensions;   // TODO: support resolution changes

    public bool keepWorldSpacePosition;
    private Vector3 worldSpaceAnchor;

    private Vector2 screenSpacePosition;
    private Vector2 accumulatedChanges;

    [Header("Extra")]
    [SerializeField]
    private Image driveImage;
    [SerializeField]
    private GameObject dummy;

    public void AimUp(float delta)
    {
        accumulatedChanges.y += delta;
    }

    public void AimRight(float delta)
    {
        accumulatedChanges.x += delta;
    }

    public void Aim(Vector2 delta)
    {
        accumulatedChanges += delta;
    }

    public void Set(Vector2 newPosition)
    {
        accumulatedChanges += newPosition - screenSpacePosition;
    }

    private void Start()
    {
        screenDimensions = new Vector2(Screen.width, Screen.height);
    }

    private void LateUpdate()
    {
        // do we need to adjust reticule to retain world-space position?
        if(keepWorldSpacePosition) { screenSpacePosition = playerCamera.WorldToScreenPoint(worldSpaceAnchor); }

        Vector2 candidatePosition = screenSpacePosition;
        candidatePosition += accumulatedChanges;

        accumulatedChanges = Vector2.zero;

        if(constrainToScreen)
        {
            candidatePosition.x = Mathf.Clamp(candidatePosition.x, 0.0f, screenDimensions.x);
            candidatePosition.y = Mathf.Clamp(candidatePosition.y, 0.0f, screenDimensions.y);
        }

        // update bookkeeping variables
        screenSpacePosition = candidatePosition;
        worldSpaceAnchor = playerCamera.ScreenToWorldPoint(screenSpacePosition);

        // where to apply changes?
        if(driveImage != null)
        {
            Vector2 normalizedPosition = new Vector2(screenSpacePosition.x / screenDimensions.x, screenSpacePosition.y / screenDimensions.y);
            driveImage.rectTransform.anchorMin = driveImage.rectTransform.anchorMax = normalizedPosition;
        }
        else
        {
            transform.position = worldSpaceAnchor;
        }

        // update dummy gameobject in world-space?
        if(dummy != null)
        {
            dummy.transform.position = worldSpaceAnchor;
        }
    }
}