using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : EnemyController
{
    [SerializeField]
    public float range;
    public float shootSpeed;
    public float aim;
    private bool canShoot = true;

    public GameObject bullet;
    public Transform shootPosition;
    public Transform bulletStorage;

    private float distanceToPlayer;
    // Start is called before the first frame update
    void Start()
    {
        aim = (100 - aim) / 100;
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer <= range)
        {
            if (canShoot == true)
            {
                Attack();
            }
        }
    }

    public override void Attack()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(attackRecharge);
        Vector2 direction = (player.position - transform.position).normalized;
        GameObject newBullet = Instantiate(bullet, shootPosition.position, Quaternion.identity, bulletStorage);
        newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y - UnityEngine.Random.Range(-0.2f, 0.15f) * aim) * shootSpeed;
        canShoot = true;
    }
}
