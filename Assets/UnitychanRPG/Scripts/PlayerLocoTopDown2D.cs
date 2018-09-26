using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocoTopDown2D : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private Rigidbody2D rbody;

    [SerializeField]
    [HideInInspector]
    private Collider2D coll2D;

    // Last polled player input vector
    [Header("Player Movement")]
    private Vector2 lastMovementInput;

    [SerializeField]
    private float baseMoveSpeed = 5.0f;
    [SerializeField]
    private float dashMultiplier = 2.0f;

    public void SetMovementInput(Vector2 input)
    {
        lastMovementInput = input;
    }

    private void FixedUpdate()
    {
        float targetMoveSpeed = baseMoveSpeed * (Input.GetButton("Sprint") ? dashMultiplier : 1.0f);

        rbody.velocity = lastMovementInput.normalized * targetMoveSpeed;
    }

    private void Reset()
    {
        rbody = rbody != null ? rbody : GetComponent<Rigidbody2D>();
        coll2D = coll2D != null ? coll2D : GetComponent<Collider2D>();
    }
}
