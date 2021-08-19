using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private Camera mainCamera;
    private BoxCollider2D boxCollider2D;
    private Vector2 boxCollider2DSize;

    private void Start() {
        mainCamera = Camera.main;
        boxCollider2D = mainCamera.GetComponent<BoxCollider2D>();
        boxCollider2DSize = boxCollider2D.size;
    }

    private void Update() {
        boxCollider2D.size = boxCollider2DSize * Progression.Growth;
    }

    private void LateUpdate() {
        virtualCamera.m_Lens.OrthographicSize = Progression.Growth * 100f;
    }
}
