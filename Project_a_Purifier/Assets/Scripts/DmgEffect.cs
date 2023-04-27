using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DmgEffect : MonoBehaviour
{
    private const float lerpTime = 2.0f;

    public IEnumerator Dmg(float damage)
    {
        float time = 0.0f;

        transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
        GetComponent<TextMeshProUGUI>().text = damage.ToString();
        while (gameObject.activeSelf)
        {
            time += Time.deltaTime;
            transform.position += new Vector3(0.0f, 0.02f, 0.0f);
            GetComponent<TextMeshProUGUI>().color = Color.Lerp(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), time / lerpTime);
            if (GetComponent<TextMeshProUGUI>().color.a < 0.1f)
                gameObject.SetActive(false);
            yield return null;
        }
    }
}
