using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 2f;
    public float neighborRadius = 2.5f;
    public float separationRadius = 1.2f;

    public float alignmentWeight = 1f;
    public float cohesionWeight = 0.6f;
    public float separationWeight = 2f;

    public LayerMask obstacleMask;
    public float obstacleAvoidanceDistance = 1.5f;
    public float obstacleAvoidanceForce = 2f;

    private Vector2 velocity;

    void Start()
    {
        velocity = Random.insideUnitCircle.normalized * speed;
    }

    void Update()
    {
        List<Boid> neighbors = GetNeighbors();

        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        Vector2 separation = Vector2.zero;

        int neighborCount = 0;

        foreach (Boid boid in neighbors)
        {
            if (boid == this) continue;

            float dist = Vector2.Distance(transform.position, boid.transform.position);

            alignment += boid.velocity;
            cohesion += (Vector2)boid.transform.position;

            if (dist < separationRadius)
            {
                Vector2 diff = (Vector2)(transform.position - boid.transform.position);
                separation += diff.normalized / (dist * dist);
            }

            neighborCount++;
        }

        if (neighborCount > 0)
        {
            // Alignment: match average neighbor velocity (scaled diff)
            Vector2 avgVelocity = alignment / neighborCount;
            alignment = (avgVelocity - velocity) * 0.1f * alignmentWeight;

            // Cohesion: move a little toward center of mass
            Vector2 centerOfMass = cohesion / neighborCount;
            Vector2 directionToCenter = (centerOfMass - (Vector2)transform.position);
            cohesion = directionToCenter * 0.05f * cohesionWeight;

            // Separation: already weighted by distance
            separation *= separationWeight;
        }

        // Obstacle avoidance
        Vector2 obstacleAvoidance = Vector2.zero;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity.normalized, obstacleAvoidanceDistance, obstacleMask);
        if (hit.collider != null)
        {
            float distanceToObstacle = hit.distance;
            if (distanceToObstacle < 1f)
            {
                // Steer to the side
                Vector2 rightTurn = new Vector2(-velocity.y, velocity.x).normalized;
                Vector2 leftTurn = new Vector2(velocity.y, -velocity.x).normalized;
                Vector2 sideStep = Vector2.Dot(rightTurn, hit.normal) > 0 ? rightTurn : leftTurn;
                obstacleAvoidance = sideStep * obstacleAvoidanceForce * 2f;
            }
            else
            {
                float avoidanceStrength = (1f - (distanceToObstacle / obstacleAvoidanceDistance)) * obstacleAvoidanceForce;
                obstacleAvoidance = hit.normal * avoidanceStrength;
            }
        }

        // Final velocity and movement
        Vector2 acceleration = alignment + cohesion + separation + obstacleAvoidance;
        velocity += acceleration * Time.deltaTime;
        velocity = Vector2.ClampMagnitude(velocity, speed);

        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Rotate to face movement direction
        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    List<Boid> GetNeighbors()
    {
        Boid[] allBoids = FindObjectsOfType<Boid>();
        List<Boid> neighbors = new List<Boid>();

        foreach (Boid boid in allBoids)
        {
            if (boid == this) continue;
            if (Vector2.Distance(transform.position, boid.transform.position) <= neighborRadius)
                neighbors.Add(boid);
        }

        return neighbors;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(velocity.normalized * obstacleAvoidanceDistance));
    }
}
