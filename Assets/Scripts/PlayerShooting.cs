using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    public Transform firePoint;
    public Weapon currentWeapon;
    public AudioSource audioSource;

    public LineRenderer lineRenderer;

    private float nextTimeToFire = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire2")) {
            if (Time.time >= nextTimeToFire) {
                
                if (currentWeapon.shootsRaycasts) {
                    ShootRaycast ();
                    audioSource.volume = 0.75f;
                    audioSource.PlayOneShot(currentWeapon.shootSound);
                }
                else {
                    currentWeapon.Shoot(firePoint);
                    if (currentWeapon.shootSound != null) {
                        audioSource.volume = 0.5f;
                        audioSource.PlayOneShot(currentWeapon.shootSound);
                    }
                    else {
                        Debug.Log("No audio clip for " + currentWeapon.name);
                    }
                }
                 
                //CinemachineShake.Instance.ShakeCamera(5f, 0.1f);
                nextTimeToFire = Time.time + 1f / currentWeapon.fireRate;
            }
        }
    }

    void ShootRaycast () {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, lineRenderer.startWidth, firePoint.right);
        foreach (RaycastHit2D hit in hits) {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(currentWeapon.raycastDamage);
            }
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 150);

        StartCoroutine(FlashLineRenderer());
    }

    IEnumerator FlashLineRenderer () {
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.025f);

        lineRenderer.enabled = false;
    }

}
