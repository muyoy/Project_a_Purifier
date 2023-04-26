using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float atk = 0;

    public void SetAtk(float _atk)
    {
        atk = _atk;
    }
    public void Shoot(float shootTime)
    {
        Invoke("DestroyBullet", shootTime);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9)
            other.gameObject.GetComponentInParent<Monster>().HpChange(atk * 1.2f);
        else if (other.gameObject.layer == 10)
            other.gameObject.GetComponentInParent<Monster>().HpChange(atk * 0.8f);
        DestroyBullet();
    }

    public void DestroyBullet()
    {
        ObjectPool.ReturnObject(gameObject);
    }
}
