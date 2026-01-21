using UnityEngine;

public class CameraBlockerEnemy : MonoBehaviour
{
    [Header("Auto refs")]
    Camera arCamera;
    GameManager gameManager;

    [Header("Approach")]
    public float approachSpeed = 0.8f;
    public float minDistanceToCamera = 0.25f;

    [Header("Make it block more")]
    public float startScale = 0.10f;
    public float nearScale = 0.45f;
    public float scaleStartDistance = 1.4f; // when far
    public float scaleNearDistance = 0.35f; // when near

    [Header("Grace")]
    public float graceTime = 5f;

    [Header("Shake to remove")]
    public float shakeThreshold = 2.2f;
    public float shakeCooldown = 0.8f;

    float lastShakeTime;
    bool active;

    void Start()
    {
        active = true;

        arCamera = Camera.main;
        if (arCamera == null) arCamera = FindObjectOfType<Camera>();

        gameManager = FindObjectOfType<GameManager>();

        transform.localScale = Vector3.one * startScale;

        if (gameManager != null)
            gameManager.FreezeForSeconds(graceTime);
    }

    void Update()
    {
        if (!active) return;
        if (arCamera == null) return;

        MoveTowardsCamera();
        UpdateScale();
        DetectShake();
    }

    void MoveTowardsCamera()
    {
        Vector3 camPos = arCamera.transform.position;
        Vector3 dir = camPos - transform.position;

        float dist = dir.magnitude;
        if (dist <= minDistanceToCamera) return;

        dir.Normalize();
        transform.position += dir * approachSpeed * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(transform.position - camPos, Vector3.up);
    }

    void UpdateScale()
    {
        float d = Vector3.Distance(transform.position, arCamera.transform.position);
        float t = Mathf.InverseLerp(scaleStartDistance, scaleNearDistance, d);
        float s = Mathf.Lerp(nearScale, startScale, t);
        transform.localScale = Vector3.one * s;
    }

    void DetectShake()
    {
        float sqr = Input.acceleration.sqrMagnitude;
        if (sqr > shakeThreshold * shakeThreshold)
        {
            if (Time.time - lastShakeTime > shakeCooldown)
            {
                lastShakeTime = Time.time;
                Dismiss();
            }
        }
    }

    void Dismiss()
    {
        active = false;
        Destroy(gameObject);
    }
}
