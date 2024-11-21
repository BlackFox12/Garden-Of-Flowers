using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    private Vector2 direction = Vector2.down;
    public float speed = 5f;

    public KeyCode inputUp = KeyCode.UpArrow;
    public KeyCode inputDown = KeyCode.DownArrow;
    public KeyCode inputLeft = KeyCode.LeftArrow;
    public KeyCode inputRight = KeyCode.RightArrow;

    private void Awake() 
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Update() 
    {
        if (Input.GetKey(inputUp)) {
            SetDirection(Vector2.up);
        } else if (Input.GetKey(inputDown)) {
            SetDirection(Vector2.down);
        } else if (Input.GetKey(inputLeft)) {
            SetDirection(Vector2.left);
        } else if (Input.GetKey(inputRight)) {
            SetDirection(Vector2.right);
        } else {
            SetDirection(Vector2.zero);
        }
    }

    private void FixedUpdate() 
    {
        Vector2 position = rigidbody.position;
        Vector2 translation = direction * speed * Time.fixedDeltaTime;

        rigidbody.MovePosition(position + translation);
    }

    private void SetDirection(Vector2 newDirection) 
    {
        direction = newDirection;
    }
}
