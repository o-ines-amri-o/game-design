using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
public float moveSpeed = 5f;

    private Vector2 movement;

    void Update()
    {
        // Get input
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float moveY = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        movement = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Move the fish
        transform.position += (Vector3)(movement * moveSpeed * Time.fixedDeltaTime);

    //// Optional: rotate the fish to face the movement direction
    //if (movement != Vector2.zero)
    //{
    //    float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
    //    transform.rotation = Quaternion.Euler(0, 0, angle);
    //}
    }
}



