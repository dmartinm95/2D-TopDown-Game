using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protect : MonoBehaviour
{
    public static Vector2 Position;

    public float rewardRate = 1f;

    private float nextRewardTime = 1f;

    private void Start() {
        // Might be better to select where it spawns manually for each level
        //transform.position = SpawnWithinCamera();
        Position = transform.position;
    }

    private void Update() {
        // IF you are near the protect object then maybe it heals you or the object
        // rather than giving points
        // if (Vector2.Distance(PlayerController.Position, transform.position) < 15f) {
            
        //     if (Time.time >= nextRewardTime) {
        //         Debug.Log("Getting more points!");
        //         Progression.instance.AddScore(1);
        //         nextRewardTime = Time.time + 1f / rewardRate;
        //     }
        // }
    }

    public Vector3 SpawnWithinCamera() {

        Vector3 screenPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Random.Range(0,Screen.width * 0.95f), 
                        Random.Range(0,Screen.height * 0.95f),
                        Camera.main.farClipPlane/2));
        return screenPosition;
    }

    private void OnDrawGizmosSelected() {
        // Get radius from EnemeAI: maxStartGoalDistance
        Gizmos.DrawWireSphere(transform.position, 15f);
    }
}
