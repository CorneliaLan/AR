using UnityEngine;

public class CameraBlockerSpawner : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public GameObject blockerPrefab;

    [Header("Timing")]
    public float spawnEverySeconds = 10f;

    [Header("Blocker Size")]
    public float blockerScale = 1.5f;
    public float freezeSeconds = 2f;

    float nextSpawnTime;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        nextSpawnTime = Time.time + spawnEverySeconds;
    }

    void Update()
    {
        if (gameManager == null || blockerPrefab == null) return;
        if (gameManager.state != GameManager.State.Playing) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnBlockerAtWorldZero();
            nextSpawnTime += spawnEverySeconds;
        }
    }

    void SpawnBlockerAtWorldZero()
    {
        gameManager.FreezeForSeconds(freezeSeconds);

        Vector3 spawnPos = Vector3.zero;

        Debug.Log("[Blocker] Spawn at WORLD ZERO (0,0,0)");

        GameObject go = Instantiate(blockerPrefab, spawnPos, Quaternion.identity);

        go.transform.localScale = Vector3.one * blockerScale;

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 dir = cam.transform.position - go.transform.position;
            if (dir.sqrMagnitude > 0.001f)
                go.transform.rotation = Quaternion.LookRotation(-dir.normalized, Vector3.up);
        }

        if (go.GetComponent<CameraBlockerEnemy>() == null)
        {
            Debug.LogError("Blocker prefab has NO CameraBlockerEnemy component!", go);
        }
    }
}
