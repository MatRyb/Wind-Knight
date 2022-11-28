using System;
using System.Collections;
using UnityEngine;

public class EnemyRanged : EnemyController
{
    [SerializeField] private float shootSpeed = 2f;
    [Range(-100f,100f)][SerializeField] private float aim = 50.0f;
    private bool canShoot = true;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private Transform bulletStorage;

    private float distanceToPlayer;

    void Start()
    {
        aim = (100 - aim) / 100;
    }

    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer <= range)
        {
            if (canShoot)
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
        yield return new WaitForSeconds(attackRecharge);
        canShoot = true;
    }
}
