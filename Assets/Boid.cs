using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed = 2f;
    public float neighborRadius = 2f;
    public float separationRadius = 1f;

    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float separationWeight = 1.5f;

    private Vector2 velocity;

    public LayerMask obstacleMask;
    public float obstacleAvoidanceDistance = 1.5f;
    public float obstacleAvoidanceForce = 2f;


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
                separation += (Vector2)(transform.position - boid.transform.position) / dist;

            neighborCount++;
        }

        if (neighborCount > 0)
        {
            alignment = (alignment / neighborCount).normalized * alignmentWeight;
            cohesion = ((cohesion / neighborCount - (Vector2)transform.position).normalized) * cohesionWeight;
            separation = (separation / neighborCount).normalized * separationWeight;
        }

        Vector2 obstacleAvoidance = Vector2.zero;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity.normalized, obstacleAvoidanceDistance, obstacleMask);
        if (hit.collider != null)
        {
            float distanceToObstacle = hit.distance;
            Debug.Log("Distance to obstacle: " + distanceToObstacle);        
            // The closer the obstacle, the stronger the avoidance force
            float avoidanceStrength = (1f - (distanceToObstacle / obstacleAvoidanceDistance)) * obstacleAvoidanceForce;
        
            // Avoid in the direction of the hit normal
            obstacleAvoidance = hit.normal * avoidanceStrength;
        }

        Vector2 acceleration = alignment + cohesion + separation + obstacleAvoidance;
        velocity += acceleration * Time.deltaTime;
        velocity = velocity.normalized * speed;

        transform.position += (Vector3)(velocity * Time.deltaTime);

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
