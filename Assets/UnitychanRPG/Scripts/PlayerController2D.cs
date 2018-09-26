using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
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
    private PlayerHUD hud;

    // Last polled player input vector
    private Vector2 lastPlayerInput;

    [Header("Player Actions")]
    public float interactRadius = 2.0f;
    public LayerMask interactLayer;

    private Vector3 effectiveCenter
    {
        get
        {
            return coll2D.bounds.center;
        }
    }

    // TODO: Bias find candidate towards the last movement direction

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

    private void Update()
    {
        lastPlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"),
                                      Input.GetAxisRaw("Vertical"));

        locomotion.SetMovementInput(lastPlayerInput);

        var interactCandidate = FindInteractCandidate();
        hud.SetInteractTarget(interactCandidate?.transform);

        if(interactCandidate == null) { return; }

        if(Input.GetButtonDown("Use"))
        {
            interactCandidate.GetComponent<IInteractible>()?.TryInteract(this);
        }
    }

    private void Reset()
    {
        rbody = rbody != null ? rbody : GetComponent<Rigidbody2D>();
        coll2D = coll2D != null ? coll2D : GetComponent<Collider2D>();
        locomotion = locomotion != null ? locomotion : GetComponent<PlayerLocoTopDown2D>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(effectiveCenter, interactRadius);
    }
}