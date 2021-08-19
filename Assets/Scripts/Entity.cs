using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int health = 20;
    public GameObject deathEffect;
    public AudioClip hitSound;
    
    public void TakeDamage (int amount) {

        health -= amount;

        GetComponent<AudioSource>().volume = 0.30f;
        GetComponent<AudioSource>().PlayOneShot(hitSound);

        if (health <= 0) {
            Die();
        }
    }

    public virtual void Die () {
        GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
        effect.transform.localScale = transform.localScale;
        Destroy(effect, 2f);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        Destroy(gameObject, 0.1f);
        
        
    }
}
