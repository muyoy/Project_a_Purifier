using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public void Shoot(float shootTime)
    {
        Invoke("DestroyBullet", shootTime);
    }

    public void DestroyBullet()
    {
        ObjectPool.ReturnObject(gameObject);
    }
}
