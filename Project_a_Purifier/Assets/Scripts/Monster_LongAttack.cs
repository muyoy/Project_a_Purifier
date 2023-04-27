using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_LongAttack : MonoBehaviour
{
    public GameObject spell;
    private void Start()
    {
        spell = gameObject.transform.GetChild(0).gameObject;
    }
    public void AnimationSetActive()
    {
        StartCoroutine(ObjectActive());
    }

    private IEnumerator ObjectActive()
    {
        yield return new WaitForSeconds(spell.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
