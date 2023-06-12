using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUIObjcet : MonoBehaviour
{
    private int hpCount;
    private int stemiaCount;

    private GameObject hpObjectPrefab;
    private GameObject stemiaObjectPrefab;

    public List<GameObject> ObjectList = new List<GameObject>();

    private void Start()
    {
        hpCount = GameManager.instance.cha_Hp;
        stemiaCount = GameManager.instance.cha_St;

        hpObjectPrefab = Resources.Load<GameObject>("Prefab/Hp");
        stemiaObjectPrefab = Resources.Load<GameObject>("Prefab/Stemia");

        if (gameObject.tag == "HpUI")
        {
            Initialize(hpCount, hpObjectPrefab);
        }
        else if(gameObject.tag == "StemiaUI")
        {
            Initialize(stemiaCount, stemiaObjectPrefab);
        }

        Reposition();
    }

    private void Initialize(int initCount, GameObject obj)
    {
        for (int i = 0; i < initCount; i++)
        {
            ObjectList.Add(CreateNewObject(obj));
        }
    }

    private GameObject CreateNewObject(GameObject obj)
    {
        GameObject newObj = Instantiate(obj);
        newObj.transform.SetParent(transform, false);
        return newObj;
    }

    private void Reposition()
    {
        float tab_x = ObjectList[0].transform.localPosition.x;
        float tab_y = ObjectList[0].transform.localPosition.y;

        for(int i = 0; i < ObjectList.Count; i++)
        {
            ObjectList[i].gameObject.transform.localPosition = new Vector3(tab_x, tab_y, 0);
            tab_x += 60;
        }
    }
}
