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

    [SerializeField]
    private int maxTraceHits = 32;
    private RaycastHit2D[] lastTraceHitResults;
    private int lastTraceHitCount = 0;

    [SerializeField]
    private LayerMask traceLayer;

    PlayerController2D()
    {
        lastTraceHitResults = new RaycastHit2D[maxTraceHits];
    }

    private void Update()
    {
        lastPlayerInput = new Vector2(Input.GetAxisRaw("Horizontal"),
                                      Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        float targetMoveSpeed = baseMoveSpeed * (Input.GetButton("Sprint") ? dashMultiplier : 1.0f);
        /*
        Vector2 targetPlayerDelta = lastPlayerInput * targetMoveSpeed * Time.deltaTime;
        lastTraceHitCount = coll2D.Cast(targetPlayerDelta, lastTraceHitResults);

        float nearestResultDistance = float.MaxValue;
        int nearestResultIndex = -1;
        for(int i = 0; i < lastTraceHitCount; ++i)
        {
            if(lastTraceHitResults[i].distance < nearestResultDistance)
            {
                nearestResultDistance = lastTraceHitResults[i].distance;
                nearestResultIndex = i;
            }
        }

        if(nearestResultIndex != -1)
        {
            Vector2 worldHitPoint = lastTraceHitResults[nearestResultIndex].point;
        }
        */

        rbody.velocity = lastPlayerInput * targetMoveSpeed;
    }

    void Reset()
    {
        traceLayer = LayerMask.GetMask("Player");
        rbody = GetComponent<Rigidbody2D>();
        coll2D = GetComponent<Collider2D>();
    }
}