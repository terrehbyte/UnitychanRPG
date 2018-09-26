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

    [SerializeField]
    [HideInInspector]
    private PlayerLocoTopDown2D locomotion;

    // Last polled player input vector
    private Vector2 lastPlayerInput;

    private void Update()
    {
        lastPlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"),
                                      Input.GetAxisRaw("Vertical"));

        locomotion.SetMovementInput(lastPlayerInput);
    }

    private void Reset()
    {
        rbody = rbody != null ? rbody : GetComponent<Rigidbody2D>();
        coll2D = coll2D != null ? coll2D : GetComponent<Collider2D>();
        locomotion = locomotion != null ? locomotion : GetComponent<PlayerLocoTopDown2D>();
    }
}