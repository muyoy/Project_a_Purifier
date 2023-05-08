using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public GameObject poolingObjectPrefab;

    Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;

        Initialize(10);
    }

    private void Initialize(int initCount)
    {
        for(int i = 0; i < initCount; i++)
        {
            poolingObjectQueue.Enqueue(CreateNewObject());
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(poolingObjectPrefab);
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public static GameObject GetObject()
    {
        if(Instance.poolingObjectQueue.Count > 0)
        {
            var obj = Instance.poolingObjectQueue.Dequeue();
            obj.GetComponent<Bullet>().SetAtk(GameManager.instance.cha_Atk);
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(null);
            return obj;
        }
        else
        {
            var newObj = Instance.CreateNewObject();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            return newObj;
        }
    }

    public static void ReturnObject(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.poolingObjectQueue.Enqueue(obj);
    }
}
