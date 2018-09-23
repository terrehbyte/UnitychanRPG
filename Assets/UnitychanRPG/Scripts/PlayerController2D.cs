using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private Rigidbody2D rbody;

    [SerializeField]
    [HideInInspector]
    private Collider2D coll2D;

    // Last polled player input vector
    [Header("Player Movement")]
    private Vector2 lastPlayerInput;

    [SerializeField]
    private float baseMoveSpeed = 5.0f;
    [SerializeField]
    private float dashMultiplier = 2.0f;

    private void Update()
    {
        lastPlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"),
                                      Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        float targetMoveSpeed = baseMoveSpeed * (Input.GetButton("Sprint") ? dashMultiplier : 1.0f);

        rbody.velocity = lastPlayerInput.normalized * targetMoveSpeed;
    }

    void Reset()
    {
        rbody = rbody != null ? rbody : GetComponent<Rigidbody2D>();
        coll2D = coll2D != null ? coll2D : GetComponent<Collider2D>();
    }
}