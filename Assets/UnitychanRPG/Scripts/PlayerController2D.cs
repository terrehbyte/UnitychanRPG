using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Bias find candidate towards the last movement direction
// TODO: Abstract aim input to not rely on mouse input
// TODO: Support aiming on controllers (maybe replace the axis for delta?)

public class PlayerController2D : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private Camera _cam;
    public Camera cam { get { return _cam; }}

    [SerializeField]
    [HideInInspector]
    private Rigidbody2D rbody;

    [SerializeField]
    [HideInInspector]
    private Collider2D coll2D;

    [SerializeField]
    [HideInInspector]
    private PlayerLocoTopDown2D locomotion;

    [SerializeField]
    private PlayerAimMouse aim;

    [SerializeField]
    private PlayerHUD hud;

    // Last polled player input vector
    private Vector2 lastPlayerMoveInput;
    private Vector2 lastCursorPosition;

    [Header("Input Settings")]
    public float deltaSensitivity = 20.0f;
    public bool requireFocus = false;
    private bool isFocused = true;

    private enum AimSource
    {
        Absolute,
        Delta
    }
    [Header("Aim Settings")]
    [SerializeField]
    private AimSource aimSource;

    // Last polled player aim vector
    private Vector2 lastPlayerAimInput;

    [Header("Player Actions")]
    public float interactRadius = 2.0f;
    public LayerMask interactLayer;

    private Vector3 effectiveCenter => coll2D.bounds.center;

    private GameObject FindInteractCandidate()
    {
        var candidates = Physics2D.OverlapCircleAll(effectiveCenter, interactRadius, interactLayer);

        GameObject closest = null;
        float distShortest = float.MaxValue;
        foreach(var candidate in candidates)
        {
            float curDist = Vector3.Distance(effectiveCenter, candidate.transform.position);

            if(curDist < distShortest)
            {
                closest = candidate.gameObject;
                distShortest = curDist;
            }
        }

        return closest;
    }

    public void CaptureCursor()
    {
        Cursor.visible = false;
        switch(aimSource)
        {
            case AimSource.Absolute:
                Cursor.lockState = CursorLockMode.Confined;
                break;
            case AimSource.Delta:
                Cursor.lockState = CursorLockMode.Locked;
                break;
            default:
                throw new System.Exception("Unknown aim source.");
        }
    }

    public void ReleaseCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ProcessInput()
    {
        switch (aimSource)
        {
            case AimSource.Absolute:
                lastPlayerAimInput = (Vector2)Input.mousePosition - lastCursorPosition;
                aim.Set(Input.mousePosition);
                break;
            case AimSource.Delta:
                lastPlayerAimInput = new Vector2(Input.GetAxisRaw("Mouse X") * deltaSensitivity,
                                                 Input.GetAxisRaw("Mouse Y") * deltaSensitivity);
                aim.Aim(lastPlayerAimInput);
                break;
        }
        lastCursorPosition = Input.mousePosition;

        lastPlayerMoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        locomotion.SetMovementInput(lastPlayerMoveInput);

        // interaction logic

        var interactCandidate = FindInteractCandidate();
        hud.SetInteractTarget(interactCandidate?.transform);

        if(interactCandidate == null) { return; }

        if(Input.GetButtonDown("Use"))
        {
            interactCandidate.GetComponent<IInteractible>()?.TryInteract(this);
        }
    }

    private void Start()
    {
        CaptureCursor();
    }

    private void Update()
    {
        // check focus
        if(Input.GetMouseButtonDown(0))             { CaptureCursor(); isFocused = true; }
        else if (Input.GetKeyDown(KeyCode.Escape))  { ReleaseCursor(); isFocused = false; }

        if(!requireFocus || (requireFocus && isFocused)) { ProcessInput(); }
        
        MoreGizmos.DrawCircle(effectiveCenter, Vector3.forward, interactRadius, 9, Color.green);
    }

    private void Reset()
    {
        rbody = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<Collider2D>();
        locomotion = GetComponent<PlayerLocoTopDown2D>();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // WARN: this functionality is duplicated in Update() to be
        //       consistent with editor

        isFocused = hasFocus;

        if(hasFocus) { CaptureCursor(); }
        else         { ReleaseCursor(); }
    }
}