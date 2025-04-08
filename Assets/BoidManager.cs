using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int boidCount = 100;
    public Vector2 spawnBounds = new Vector2(10, 10);

    void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(-spawnBounds.x, spawnBounds.x),
                Random.Range(-spawnBounds.y, spawnBounds.y)
            );

            Instantiate(boidPrefab, pos, Quaternion.identity);
        }
    }
}
