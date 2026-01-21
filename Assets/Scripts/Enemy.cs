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

    public void Init(Spawner s, GameManager gm, Transform baseTarget)
    {
        spawner = s;
        gameManager = gm;
        target = baseTarget;

        // Optional: zufällige Größe minimal
        float scale = Random.Range(0.035f, 0.05f);
        transform.localScale = Vector3.one * scale;
    }

    void Update()
    {
        if (!alive) return;
        if (gameManager.state != GameManager.State.Playing) return;
        if (!target) return;

        Vector3 dir = (target.position - transform.position);
        float d = dir.magnitude;

        if (d <= reachDistance)
        {
            alive = false;
            gameManager.DamageBase(1);
            spawner.OnEnemyReachedBase();
            Destroy(gameObject);
            return;
        }

        transform.position += dir.normalized * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
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
