using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;

    public GameObject impactEffect;

    public bool explodeOnImpact = false;
    public bool explodeAfterTime = false;
    public float explosionRange = 4f;
    public float explosionWaitTime = 2f;

    public bool enemyBullet = false;
    
    private float explodeTime;

    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        speed *= (Progression.Growth - 1f) * 0.5f + 1f;
        rb.velocity = transform.right * speed;

        if (enemyBullet) {
            rb.AddTorque(50f, ForceMode2D.Impulse);
        }

        explodeTime = Time.time + explosionWaitTime;

    }

    private void Update() {
        if (!explodeAfterTime) {
            return;
        }

        if (Time.time >= explodeTime) {
            Explode(null);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {

        // if (explodeAfterTime) {
        //     return;
        // }

        if (!enemyBullet) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null) {
                Explode(enemy);
            }
        }
        else {
            Player player = collider.GetComponent<Player>();
            if (player != null) {
                Explode(player);
            }
        }
    
    }

    void Explode (Entity target) {
        if (target != null) {
            target.TakeDamage(damage);
        }

        if (explodeOnImpact && !enemyBullet) {
            Collider2D[] targets = Physics2D.OverlapCircleAll(rb.position, explosionRange);
            foreach (Collider2D t in targets) {
                Enemy enemy = t.GetComponent<Enemy>();
                if (enemy != null) {
                    enemy.TakeDamage(damage);
                }

                Bullet bullet = t.GetComponent<Bullet>();
                if (bullet != null && bullet != this) {
                    bullet.Remove();
                }
            }
        }

        Remove();
    }

    public void Remove () {
        if (impactEffect != null) {
            GameObject effect = Instantiate(impactEffect, transform.position, transform.rotation);
            effect.transform.localScale = transform.localScale;
            Destroy(effect, 5f);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
    
}
