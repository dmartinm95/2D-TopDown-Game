using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private static List<Rigidbody2D> EnemyRBs;

    public float moveSpeed = 5f;

    [Range(0f, 1f)]
    public float turnSpeed = .1f;

    public float repelRange = .5f;
    public float repelAmount = 1f;

    public float startMaxChaseDistance = 100f;
    private float maxChaseDistance;

    public ParticleSystem trailPS;

    [Header("Shooting")]
    public bool isShooter = false;
    public bool isBurst = false;
    public int burstCounter = 5;
    public float timeBetweenShots = 0.15f;
    private bool doingBurst = false;
    public float strafeSpeed = 1f;
    public float shootDistance = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    private float nextTimeToFire = 1f;

    [Header("Charging")]
    public bool isCharger = false;
    public float chargeSpeed = 1f;
    public float chargeDistance = 50f;
    public float chargeRate = 1f;
    public float nextTimeToCharge = 1f;
    public float waitTimeBeforeCharge = 0.5f;
    public float chargeDuration = 3f;
    public float slowDownDuration = 1f;
    public float dragValueDuringCharge = 3f;
    public ParticleSystem trailEffect;
    public GameObject incomingArrowEffect;
    private SpriteRenderer enemyBirdieSprite;
    public bool isCharging = false;
    public LayerMask layerMask;

    private Rigidbody2D rb;

    private Vector3 velocity;
    private Camera mainCamera;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        if (EnemyRBs == null) {
            EnemyRBs = new List<Rigidbody2D>();
        }

        moveSpeed *= (Progression.Growth - 1f) * 0.5f + 1f;

        if (isCharger) {
            enemyBirdieSprite = GetComponentInChildren<SpriteRenderer>();
        }

        mainCamera = Camera.main;

        chargeSpeed *= (Progression.Growth - 1f) * 0.10f + 1f;

        chargeDistance *= (Progression.Growth - 1f) * 0.5f + 1f;

        //waitTimeBeforeCharge /= (Progression.Growth - 1f) * 0.5f + 1f;

        EnemyRBs.Add(rb);
    }
    
    private void OnDestroy() {
        EnemyRBs.Remove(rb);
    }

    private void FixedUpdate() {
        maxChaseDistance = startMaxChaseDistance * Progression.Growth;
        
        float distance = Vector2.Distance(rb.position, PlayerController.Position);
        
        if (distance > maxChaseDistance) {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (PlayerController.Position - rb.position).normalized;

        Vector2 newPos;

        if (isShooter) {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

            if (distance > shootDistance) {
                newPos = MoveRegular(direction);
            }
            else {
                newPos = MoveStrafing(direction);
                
            }

            if (isBurst && !doingBurst) {
                StartCoroutine(ShootInBurst(burstCounter, timeBetweenShots));
            }
            else {
                Shoot();
            }

            if (trailPS != null) {
                trailPS.Emit(1);
            }
            
            newPos -= rb.position;

            rb.AddForce(newPos, ForceMode2D.Impulse);
        }
        else if (isCharger) {
            Vector2 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (distance < chargeDistance) {
                if (Time.time >= nextTimeToCharge) {
                    if (!onScreen) {
                        Debug.Log("Not visible, need to show charging direction!");
                        CreateArrowIndicator();
                    }
                    StartCoroutine(PerformCharge(waitTimeBeforeCharge, chargeDuration * Progression.Growth, slowDownDuration, dragValueDuringCharge));
                    nextTimeToCharge = Time.time + 1f / chargeRate;
                }
            }

            if (!isCharging) {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rb.rotation = Mathf.LerpAngle(rb.rotation, angle, turnSpeed);

                newPos = MoveRegular(direction);

                rb.MovePosition(newPos);
            }
        }
        else {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = Mathf.LerpAngle(rb.rotation, angle, turnSpeed);

            newPos = MoveRegular(direction);

            rb.MovePosition(newPos);
        }
    }

    void CreateArrowIndicator () {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, transform.right, 100f * Progression.Growth, layerMask);
        Debug.DrawRay(transform.position, transform.right * 100f * Progression.Growth, Color.green);

        GameObject effect = Instantiate(incomingArrowEffect, hit2D.point, transform.rotation);
        //effect.transform.parent = Camera.main.transform;
        Destroy (effect, 5f);
        // effect.transform.parent = mainCamera.transform;
    }

    IEnumerator ShootInBurst (int burstCount, float timeBetweenShots) {

        if (Time.time > nextTimeToFire) {
            doingBurst = true;
            for (int i=0; i<burstCount; i++) {
                yield return new WaitForSecondsRealtime(timeBetweenShots);
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                bullet.transform.localScale *= Progression.Growth;
                Destroy(bullet, 7f);
            }
            doingBurst = false;
            nextTimeToFire = Time.time + 1f / fireRate;
        }
    }

    IEnumerator PerformCharge (float waitTimeBeforeCharge, float chargeDuration, float slowDownDuration, float dragValueDuringCharge) {
        isCharging = true;

        rb.drag = dragValueDuringCharge;

        trailEffect.Play();

        yield return new WaitForSeconds(waitTimeBeforeCharge);

        rb.velocity = Vector2.zero;
        rb.drag = 0f;

        while (chargeDuration >= 0f) {
            rb.AddForce(transform.right * chargeSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            chargeDuration -= Time.fixedDeltaTime;
            yield return null;
        }

        rb.drag = dragValueDuringCharge;

        yield return new WaitForSeconds(slowDownDuration);

        isCharging = false;
        rb.drag = 0f;
    }

    void Shoot () {
        if (Time.time >= nextTimeToFire) {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.transform.localScale *= Progression.Growth;

            Destroy(bullet, 5f);

            nextTimeToFire = Time.time + 1f / fireRate;
        }
    }

    Vector2 MoveStrafing (Vector2 direction) {
        Vector2 newPos = transform.position + transform.right * Time.fixedDeltaTime * strafeSpeed;

        return newPos;
    }

    Vector2 MoveRegular (Vector2 direction) {
        Vector2 repelForce = Vector2.zero;

        foreach (Rigidbody2D enemy in EnemyRBs) {
            if (enemy == rb) {
                continue;
            }

            if (Vector2.Distance(enemy.position, rb.position) <= repelRange) {
                Vector2 repelDir = (rb.position - enemy.position).normalized;
                repelForce += repelDir;
            }
        }

        Vector2 newPos = transform.position + transform.right * Time.fixedDeltaTime * moveSpeed;
        newPos += repelForce * Time.fixedDeltaTime * repelAmount;

        return newPos;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, startMaxChaseDistance);

        if (isShooter) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, shootDistance);  
        }
        if (isCharger) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chargeDistance);
        }
         
    }

}
