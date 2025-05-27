using UnityEngine;
using System.Collections.Generic;

public class PlayerBoid : Boid
{
    public GameObject foodPrefab; // Prefab for food
    private bool hasFood = false;
    public Transform foodHoldPoint; // Optional visual
    private  GameObject foodVisual;

    public override void Flock(List<Boid> boids)
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mouseWorld.x, mouseWorld.y);

        Vector2 desired = mousePos - position;
        desired = desired.normalized * maxSpeed;

        Vector2 steer = desired - velocity;
        steer = Vector2.ClampMagnitude(steer, maxForce);

        acceleration += steer;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasFood && other.CompareTag("Food"))
        {
            hasFood = true;
            Destroy(other.gameObject);  // Pick up food

            // Optional: Instantiate a visual food near the player
            if (foodHoldPoint != null)
            {
                foodVisual = Instantiate(foodPrefab, foodHoldPoint.position, Quaternion.identity);
                foodVisual.transform.SetParent(foodHoldPoint);
            }
        }
        if(hasFood && other.CompareTag("Boid"))
        {
            hasFood = false;
            Destroy(foodVisual);  // Drop food
            other.GetComponent<Boid>().SetPlayer(transform); // Set the player as the target
        }
    }
}
