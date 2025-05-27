using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance;
    public List<Boid> boids;
    public Text text;
    public Boid playerBoid;
    public GameObject boidPrefab;

    private float shoalTimer = 0f;
    private float requiredTime = 7f;
    private bool endSceneTriggered = false;

    void Awake()
    {
        Instance = this;
        boids = new List<Boid>();

        Vector3 worldMin = Vector3.zero;
        Vector3 worldMax = new Vector3(500f, 300f, 0f);

        for (int i = 0; i < 100; i++)
        {
            GameObject boidObject = Instantiate(boidPrefab, new Vector3(Random.Range(worldMin.x, worldMax.x), Random.Range(worldMin.y, worldMax.y), 0), Quaternion.identity);
            Boid boid = boidObject.GetComponent<Boid>();
            boids.Add(boid);
        }

        if (!boids.Contains(playerBoid))
        {
            boids.Add(playerBoid);
        }
    }

    void Update()
    {
        if (playerBoid == null)
        {
            Debug.LogWarning("playerBoid is not assigned!");
            return;
        }

        int clusterSize = CountCluster(playerBoid, 30f);

        if (text != null)
            text.text = "Size of shoal: " + clusterSize.ToString();

        Debug.Log("Shoal size: " + clusterSize);

        // Timer logic
        if (clusterSize > 30)
        {
            shoalTimer += Time.deltaTime;

            if (shoalTimer >= requiredTime && !endSceneTriggered)
            {
                endSceneTriggered = true;
                LoadEndScene();
            }
        }
        else
        {
            shoalTimer = 0f; // Reset if below threshold
        }
    }

    public int CountCluster(Boid start, float threshold = 40f)
    {
        HashSet<Boid> visited = new HashSet<Boid>();
        Queue<Boid> queue = new Queue<Boid>();

        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Boid current = queue.Dequeue();
            Vector2 currentPos = current.position;

            foreach (Boid other in boids)
            {
                if (!visited.Contains(other))
                {
                    float distance = Vector2.Distance(currentPos, other.position);

                    if (distance < threshold)
                    {
                        visited.Add(other);
                        queue.Enqueue(other);
                    }
                }
            }
        }

        return visited.Count;
    }

    void LoadEndScene()
    {
        Debug.Log("Shoal stayed large enough — loading End scene!");
        SceneManager.LoadScene("End"); // Make sure "End" scene is added in Build Settings
    }
}
