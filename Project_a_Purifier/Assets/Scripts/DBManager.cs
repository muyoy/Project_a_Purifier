using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class DBStruct
{
    [Serializable]
    public class TestDBData
    {
        public int gold;
        public int dia;
        public int playcount;

        public TestDBData(string[] data)
        {
            int count = 0;
            gold = int.Parse(data[count++]);
            dia = int.Parse(data[count++]);
            playcount = int.Parse(data[count]);
        }
    }
}
public class DBManager : MonoBehaviour
{
    public static DBManager instance = null;
    public List<DBStruct.TestDBData> testData = new List<DBStruct.TestDBData>();
    private const string testDBPath = "/TestDB.txt";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadTestDB(testDBPath);
    }

    private void LoadTestDB(string path)
    {
        string[] sheepUnitDB = File.ReadAllLines(Application.streamingAssetsPath + path);

        for (int i = 1; i < sheepUnitDB.Length; i++)
        {
            testData.Add(new DBStruct.TestDBData(sheepUnitDB[i].Split(',')));
        }
    }
}
