using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithGrowth : MonoBehaviour
{
    private Vector3 baseScale;

    public float scaleFactor = 1f;

    private void Start() {
        baseScale = transform.localScale;
    }

    private void Update() {
        transform.localScale = baseScale * ((Progression.Growth - 1f) * scaleFactor + 1f);
    }
}
