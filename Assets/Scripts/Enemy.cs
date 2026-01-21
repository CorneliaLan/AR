using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    public float speed = 0.18f;
    public float reachDistance = 0.04f;

    Spawner spawner;
    GameManager gameManager;
    Transform target;

    bool alive = true;

    EnemyPathFollower pathFollower;

    public void Init(Spawner s, GameManager gm, Transform baseTarget)
    {
        spawner = s;
        gameManager = gm;
        target = baseTarget;

        pathFollower = GetComponent<EnemyPathFollower>();

        float scale = Random.Range(0.035f, 0.05f);
        transform.localScale = Vector3.one * scale;
    }

    void Update()
    {
        if (!alive) return;
        if (gameManager == null) return;
        if (gameManager.state != GameManager.State.Playing) return;

        if (gameManager.isFrozen) return;

        if (!target) return;

        if (pathFollower != null)
        {
            CheckReachedBase();
            return;
        }

        Vector3 dir = (target.position - transform.position);
        float d = dir.magnitude;

        if (d <= reachDistance)
        {
            ReachBaseAndDie();
            return;
        }

        transform.position += dir.normalized * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    void CheckReachedBase()
    {
        Vector3 dir = (target.position - transform.position);
        float d = dir.magnitude;

        if (d <= reachDistance)
        {
            ReachBaseAndDie();
        }
    }

    void ReachBaseAndDie()
    {
        alive = false;
        gameManager.DamageBase(1);
        spawner.OnEnemyReachedBase();
        Destroy(gameObject);
    }

    public void Kill()
    {
        if (!alive) return;
        alive = false;

        spawner.OnEnemyKilled();
        gameManager.AddScore(1);

        Destroy(gameObject);
    }
}
