using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFollower : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 0.18f;
    public float turnSpeed = 8f;
    public float waypointTolerance = 0.02f;

    [Header("Random Path")]
    [Range(0, 8)] public int minWaypoints = 2;
    [Range(0, 12)] public int maxWaypoints = 4;
    public float boardPadding = 0.06f;
    public float minDistanceBetweenPoints = 0.10f;

    [Header("Debug")]
    public bool drawDebugPath = false;

    List<Vector3> path = new();
    int index;

    GameManager gm;
    Transform baseTarget;
    Bounds boardBounds;
    float boardY;

    public void Init(GameManager gameManager, Bounds bounds, Transform baseTf)
    {
        gm = gameManager;
        baseTarget = baseTf;
        boardBounds = bounds;
        boardY = bounds.center.y;

        BuildPath();
    }

    void Update()
    {
        if (gm != null && gm.isFrozen) return;
        if (path.Count == 0 || index >= path.Count) return;

        Vector3 target = path[index];
        Vector3 to = target - transform.position;
        to.y = 0f;

        if (to.magnitude <= waypointTolerance)
        {
            index++;
            return;
        }

        transform.position += to.normalized * moveSpeed * Time.deltaTime;

        if (to.sqrMagnitude > 0.0001f)
        {
            Quaternion desired = Quaternion.LookRotation(to.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, turnSpeed * Time.deltaTime);
        }
    }

    void BuildPath()
    {
        path.Clear();
        index = 0;

        Vector3 start = transform.position;
        start.y = boardY;
        path.Add(start);

        int count = Random.Range(minWaypoints, maxWaypoints + 1);
        Vector3 last = start;

        for (int i = 0; i < count; i++)
        {
            Vector3 p = RandomPointOnBoard();

            int safety = 0;
            while (Vector3.Distance(p, last) < minDistanceBetweenPoints && safety < 30)
            {
                p = RandomPointOnBoard();
                safety++;
            }

            path.Add(p);
            last = p;
        }

        Vector3 end = baseTarget.position;
        end.y = boardY;
        path.Add(end);
    }

    Vector3 RandomPointOnBoard()
    {
        float minX = boardBounds.min.x + boardPadding;
        float maxX = boardBounds.max.x - boardPadding;
        float minZ = boardBounds.min.z + boardPadding;
        float maxZ = boardBounds.max.z - boardPadding;

        return new Vector3(
            Random.Range(minX, maxX),
            boardY,
            Random.Range(minZ, maxZ)
        );
    }

    void OnDrawGizmos()
    {
        if (!drawDebugPath || path == null || path.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(path[i], path[i + 1]);
    }
}
