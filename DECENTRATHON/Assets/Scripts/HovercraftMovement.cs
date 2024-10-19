using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HovercraftMovement : MonoBehaviour
{
    // Movement variables
    public float hoverHeight = 3.0f; // Height above ground
    public float hoverForce = 50.0f; // Force applied to keep hovering
    public float forwardAcceleration = 25.0f;
    public float brakeForce = 30.0f;
    public float turnSpeed = 5.0f;
    public float driftMultiplier = 1.5f;
    public float maxSpeed = 50.0f;

    private Rigidbody rb;
    private float currentSpeed = 0.0f;
    private bool isBraking = false;

    // Hovering settings
    public LayerMask groundLayer;
    private RaycastHit hit;
    public float driftBoostDistance = 2.0f; // Distance at which hovercraft will gain speed when near walls/floor

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We don't want gravity pulling the hovercraft down
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        Hover();
        Move();
        ApplyDriftBoost();
        CapSpeed();
    }

    void HandleInput()
    {
        // Forward/Backward Acceleration
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += forwardAcceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentSpeed -= forwardAcceleration * Time.deltaTime;
        }

        // Braking
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpeed -= brakeForce * Time.deltaTime;
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

        // Turning
        float turnInput = Input.GetAxis("Horizontal");
        rb.AddTorque(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
    }

    void Hover()
    {
        // Raycast downward to simulate hovering
        if (Physics.Raycast(transform.position, Vector3.down, out hit, hoverHeight, groundLayer))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            rb.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }
    }

    void Move()
    {
        Vector3 forwardForce = transform.forward * currentSpeed * Time.deltaTime;
        rb.AddForce(forwardForce, ForceMode.Acceleration);
    }

    void ApplyDriftBoost()
    {
        // Apply drift boost based on proximity to walls or ground
        if (Physics.Raycast(transform.position, transform.right, out hit, driftBoostDistance, groundLayer) ||
            Physics.Raycast(transform.position, -transform.right, out hit, driftBoostDistance, groundLayer) ||
            Physics.Raycast(transform.position, Vector3.down, out hit, driftBoostDistance, groundLayer))
        {
            currentSpeed += forwardAcceleration * driftMultiplier * Time.deltaTime;
        }
    }

    void CapSpeed()
    {
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
    }
}
