using UnityEngine;

public class CrosshairRayWeapon : MonoBehaviour
{
    public Camera arCamera;
    public GameManager gameManager;

    [Header("Ray")]
    public float maxDistance = 2.0f;
    public float fireCooldown = 0.25f;

    [Header("Optional Feedback")]
    public RectTransform crosshairUI; 
    public float hitScale = 1.25f;

    float nextFireTime;
    Vector3 baseScale;

    void Start()
    {
        if (crosshairUI) baseScale = crosshairUI.localScale;
    }

    void Update()
    {
        if (gameManager.state != GameManager.State.Playing) return;
        if (Time.time < nextFireTime) return;

        Ray ray = new Ray(arCamera.transform.position, arCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.Kill();
                nextFireTime = Time.time + fireCooldown;

                if (crosshairUI) crosshairUI.localScale = baseScale * hitScale;
            }
            else
            {
                if (crosshairUI) crosshairUI.localScale = baseScale;
            }
        }
        else
        {
            if (crosshairUI) crosshairUI.localScale = baseScale;
        }
    }
}
