using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    private void Start()
    {
        rb.velocity = transform.right * speed;
        Destroy(gameObject, 5f);
        //Invoke("DestroyBullet", 5f);
    }

    public void DestroyBullet()
    {
        ObjectPool.ReturnObject(this);
    }
}
