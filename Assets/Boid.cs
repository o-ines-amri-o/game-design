using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;
    public float maxForce = 0.02f;
    public float maxSpeed = 1f;
    public float perceptionRadius = 50f;

    private Transform player;

    private void Start()
    {
        position = transform.position;
        velocity = Random.insideUnitCircle.normalized * Random.Range(2f, 4f);
        acceleration = Vector2.zero;
    }

    public void SetPlayer(Transform _player)
    {
        player = _player;
    }

    

private void FollowPlayer(List<Boid> boids)
{
    // --- Follow player (arrival behavior) ---
    Vector2 desired = (Vector2)player.position - position;
    float distance = desired.magnitude;
    Vector2 direction = desired.normalized;

    // Smooth arrival speed based on distance
    float arrivalSpeed = Mathf.Min(maxSpeed, Mathf.Sqrt(distance) * 2f);
    Vector2 arriveVelocity = direction * arrivalSpeed;

    // --- Separation behavior ---
    Vector2 separation = Separation(boids);

    // --- Combine: prioritize separation more if very close ---
    float separationWeight = 100f;
    Vector2 combinedVelocity = arriveVelocity + separation * separationWeight;
    // Clamp and move
    velocity = Vector2.ClampMagnitude(combinedVelocity, maxSpeed);
    position += velocity * Time.deltaTime;
    transform.position = position;
    acceleration = Vector2.zero;
}





    private void Update()
    {
        if (player != null)
        {
            FollowPlayer(BoidManager.Instance.boids);
            DrawBoid();

        } else {
            Flock(BoidManager.Instance.boids);
            UpdateBoid();
            Edges();
            DrawBoid();
        }
        
    }

    public virtual void Flock(List<Boid> boids)
    {
        Vector2 alignment = Align(boids);
        Vector2 cohesion = Cohesion(boids);
        Vector2 separation = Separation(boids);

        alignment *= 1.0f;
        cohesion *= 1.0f;
        separation *= 1.5f;

        acceleration += alignment + cohesion + separation;
    }

    Vector2 Align(List<Boid> boids)
    {
        Vector2 steering = Vector2.zero;
        int total = 0;

        foreach (Boid other in boids)
        {
            if (other == this) continue;

            float d = Vector2.Distance(position, other.position);
            if (d < perceptionRadius)
            {
                steering += other.velocity;
                total++;
            }
        }

        if (total > 0)
        {
            steering /= total;
            steering = steering.normalized * maxSpeed;
            steering -= velocity;
            steering = Vector2.ClampMagnitude(steering, maxForce);
        }

        return steering;
    }

    Vector2 Cohesion(List<Boid> boids)
    {
        Vector2 steering = Vector2.zero;
        int total = 0;

        foreach (Boid other in boids)
        {
            if (other == this) continue;

            float d = Vector2.Distance(position, other.position);
            if (d < perceptionRadius)
            {
                steering += other.position;
                total++;
            }
        }

        if (total > 0)
        {
            steering /= total;
            steering -= position;
            steering = steering.normalized * maxSpeed;
            steering -= velocity;
            steering = Vector2.ClampMagnitude(steering, maxForce);
        }

        return steering;
    }

    Vector2 Separation(List<Boid> boids)
    {
        Vector2 steering = Vector2.zero;
        int total = 0;
        float desiredSeparation = 24f;

        foreach (Boid other in boids)
        {
            if (other == this) continue;

            float d = Vector2.Distance(position, other.position);
            if (d < desiredSeparation && d > 0)
            {
                Vector2 diff = position - other.position;
                diff /= (d * d);
                steering += diff;
                total++;
            }
        }

        if (total > 0)
        {
            steering /= total;
            steering = steering.normalized * maxSpeed;
            steering -= velocity;
            steering = Vector2.ClampMagnitude(steering, maxForce);
        }

        return steering;
    }

    public void UpdateBoid()
    {
        velocity += acceleration;
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        position += velocity * Time.deltaTime;
        transform.position = position;
        acceleration = Vector2.zero;
    }



    public void Edges()
    {
        Vector3 worldMin = Vector3.zero;
        Vector3 worldMax = new Vector3(500f, 300f, 0f);
    
        if (position.x > worldMax.x)
        {
            position.x = worldMax.x;
            velocity.x *= -1;
        }
        else if (position.x < worldMin.x)
        {
            position.x = worldMin.x;
            velocity.x *= -1;
        }
    
        if (position.y > worldMax.y)
        {
            position.y = worldMax.y;
            velocity.y *= -1;
        }
        else if (position.y < worldMin.y)
        {
            position.y = worldMin.y;
            velocity.y *= -1;
        }
    }

 

    public void DrawBoid()
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
