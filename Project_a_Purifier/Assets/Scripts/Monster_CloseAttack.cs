using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_CloseAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponent<Unit>().HpChange(1);
    }
}
