using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    public Transform firePoint;

    public GameObject bulletPrefab;

    public void Fire(Vector3 target)
    {
        GameObject babyBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        babyBullet.transform.right = target - babyBullet.transform.position;
    }
}
