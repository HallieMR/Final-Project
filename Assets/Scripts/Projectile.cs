using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    public GameObject cogPrefab;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }

    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    } 

    void OnCollisionEnter2D(Collision2D other)
    {
        HardEnemyController h = other.collider.GetComponent<HardEnemyController>();
        EnemyController e = other.collider.GetComponent<EnemyController>();
        RubyController player = other.gameObject.GetComponent<RubyController>();
        if (e != null)
        {
            e.Fix();
            //player.setScore(1);
        }
        if(h != null)
        {
            h.Fix();
            //player.setScore(1);
        }

        Destroy(gameObject);
    }

}
