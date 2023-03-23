using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform firepoint;
    public GameObject firePrefab;
    public void Shoot()
    {
        Instantiate(firePrefab, firepoint.position, firepoint.rotation);
    }
}
