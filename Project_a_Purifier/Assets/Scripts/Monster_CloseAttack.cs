using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_CloseAttack : MonoBehaviour
{
    public Animator animator;

    public void AnimationSetActive()
    {
        //StartCoroutine(ObjectActive());
    }

    //private IEnumerator ObjectActive()
    //{
    //    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.gameObject.GetComponent<Unit>().HpChange(1);
    }
}
