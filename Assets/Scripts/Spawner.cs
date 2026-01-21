using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameManager gameManager;
    public Enemy enemyPrefab;

    [Header("Difficulty")]
    public float spawnInterval = 1.2f;
    public float intervalMin = 0.35f;
    public float intervalDecay = 0.02f; // pro Kill
    public int maxAlive = 10;

    List<Transform> spawnPoints = new();
    Transform baseTarget;

    float nextSpawnTime;
    bool running;

    int aliveCount;

    public void ConfigureFromBoard(GameObject board)
    {
        spawnPoints.Clear();

        // Erwartet: Board hat Child "SpawnPoints" mit 4 empties
        var sp = board.transform.Find("SpawnPoints");
        if (sp != null)
        {
            for (int i = 0; i < sp.childCount; i++)
                spawnPoints.Add(sp.GetChild(i));
        }

        var baseObj = board.transform.Find("Base");
        baseTarget = baseObj != null ? baseObj : board.transform;
    }

    public void Begin()
    {
        running = true;
        nextSpawnTime = Time.time + 0.5f;
        aliveCount = 0;
    }

    public void Stop()
    {
        running = false;
        aliveCount = 0;
    }

    void Update()
    {
        if (!running) return;
        if (gameManager.state != GameManager.State.Playing) return;
        if (spawnPoints.Count == 0 || baseTarget == null) return;

        if (Time.time >= nextSpawnTime && aliveCount < maxAlive)
        {
            SpawnOne();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnOne()
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Enemy e = Instantiate(enemyPrefab, sp.position, Quaternion.identity);
        e.Init(this, gameManager, baseTarget);
        aliveCount++;
    }

    public void OnEnemyKilled()
    {
        aliveCount = Mathf.Max(0, aliveCount - 1);

        // Difficulty scaling: schneller spawnen je mehr Kills
        spawnInterval = Mathf.Max(intervalMin, spawnInterval - intervalDecay);
    }

    public void OnEnemyReachedBase()
    {
        aliveCount = Mathf.Max(0, aliveCount - 1);
    }
}
