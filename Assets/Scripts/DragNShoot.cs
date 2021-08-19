using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNShoot : MonoBehaviour
{
    public float power = 10f;

    public Rigidbody2D rb;

    public Vector2 minPower;
    public Vector2 maxPower;

    public PathLine pathLine;

    public float holdTimer = 2f;
    private float currentHoldTimer;
    private float holdPower;
    public bool powerMode = false;

    Camera cam;

    Vector2 force;
    Vector3 startPoint;
    Vector3 endPoint;
    float pullDistance;

    private int consecutiveHits = 0;
    private bool isMoving = false;
    private bool hitEnemy = false;
    private bool shotTaken = false;

    private void Start() {
        cam = Camera.main;
        pathLine = GetComponent<PathLine>();
    }

    private void Update() {

        if (Input.GetMouseButtonDown(0)) {
            shotTaken = false;
            startPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            startPoint.z = 15;
            currentHoldTimer = holdTimer;
        }

        // If mouse button is held
        if (Input.GetMouseButton(0)) {
            Vector3 currentPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            currentPoint.z = 15;

            pathLine.RenderLine(startPoint, currentPoint);

            Vector2 lookDir = (startPoint - currentPoint).normalized;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
		    rb.rotation = Mathf.LerpAngle(rb.rotation, angle, 1f);
            
            pullDistance = Mathf.Clamp(Vector2.Distance(startPoint, currentPoint), 0.1f, 20f);

            holdPower = 1f;

            currentHoldTimer -= Time.deltaTime;
            if (currentHoldTimer < 0f) {
                Debug.Log("Extra Power!");
                holdPower = 2f;
                powerMode = true;
            }
            
        }

        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Mouse Up");
            shotTaken = true;
            hitEnemy = false;

            powerMode = false;

            endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
            endPoint.z = 15;

            Vector2 lookDir = (startPoint - endPoint).normalized;

            rb.velocity = Vector2.zero;

            rb.AddForce(lookDir * power * pullDistance * holdPower, ForceMode2D.Impulse);

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
		    rb.rotation = angle;

            pathLine.EndLine();
        }

        if (shotTaken) {
            if (rb.velocity == Vector2.zero && !hitEnemy) {
                consecutiveHits = 0;
            }
        }
        else {
            // Need to reset counter if you shoot twice while the first shot
        }

        //Debug.Log(consecutiveHits);


    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Enemy")) {
            hitEnemy = true;
            consecutiveHits++;
        }
    }

}
