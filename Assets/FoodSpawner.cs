using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public int maxfoodCount = 20;
    public Vector2 spawnAreaMin = new Vector2(-10f, -5f);
    public Vector2 spawnAreaMax = new Vector2(10f, 5f);
    public float spawnInterval = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        // Only spawn if under max count and interval has passed
        if (timer >= spawnInterval && GameObject.FindGameObjectsWithTag("Food").Length < maxfoodCount)
        {
            SpawnFood();
            timer = 0f;
        }
    }

    void SpawnFood()
    {
        Vector2 spawnPos = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Instantiate(foodPrefab, spawnPos, Quaternion.identity);
    }
}
