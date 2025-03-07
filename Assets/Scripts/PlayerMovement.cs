using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    public Rigidbody2D body;
    Vector3 movement;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement = Vector3.zero;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        body.MovePosition(transform.position + movement.normalized * moveSpeed * Time.deltaTime);
    }
}
