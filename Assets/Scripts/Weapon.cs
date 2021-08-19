using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject {
    public GameObject bulletPrefab;
    public float fireRate;

    public GameObject shootEffect;
    public AudioClip shootSound;

    public bool shootsExplosions = false;
    public bool shootsRaycasts = false;
    public int raycastDamage = 20;

    public void Shoot (Transform firePoint) {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.transform.localScale *= Progression.Growth;

        // GameObject effect = Instantiate(shootEffect, firePoint.position, firePoint.rotation);
        // effect.transform.localScale *= Progression.Growth;

        // Destroy(effect, 5f);
        Destroy(bullet, 10f);
    }
}
