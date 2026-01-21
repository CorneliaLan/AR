using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceOnPlane : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject boardPrefab;
    public GameManager gameManager;

    GameObject placedBoard;
    static readonly List<ARRaycastHit> hits = new();

    void Update()
    {
        if (placedBoard != null) return;
        if (Input.touchCount != 1) return;

        Touch t = Input.GetTouch(0);
        if (t.phase != TouchPhase.Began) return;

        if (!raycastManager.Raycast(t.position, hits, TrackableType.PlaneWithinPolygon))
            return;

        Pose p = hits[0].pose;
        placedBoard = Instantiate(boardPrefab, p.position, p.rotation);

        Vector3 cam = Camera.main.transform.position;
        Vector3 dir = cam - placedBoard.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            placedBoard.transform.rotation = Quaternion.LookRotation(-dir.normalized, Vector3.up);

        gameManager.OnBoardPlaced(placedBoard);
    }
}
