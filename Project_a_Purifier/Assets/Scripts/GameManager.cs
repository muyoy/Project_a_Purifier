using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    #region TestData
    public string user_ID;
    public float cha_Hp;
    public float cha_St;
    public int cha_St_Reg;
    public float cha_Atk;
    public float cri_Chan;
    public float cri_Magn;
    public float move_Speed;
    public float atk_spd;
    public int bullet_Cou;
    public float rer_Spd;
    public string wep_Type;

    public void CharacherData()
    {
        user_ID = null;
        cha_Hp = 10.0f;
        cha_St = 10.0f;
        cha_St_Reg = 1;
        cha_Atk = 10.0f;
        cri_Chan = 10.0f;
        cri_Magn = 200.0f;
        move_Speed = 10.0f;
    }

    public void TestSceneReset()
    {
        SceneManager.LoadScene("Developer_Scene");
    }
    #endregion
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
        CharacherData();
    }

    private void OnApplicationPause(bool pause)
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }
}

public class CharacherData
{
    public string user_ID;
    public float cha_Hp;
    public float cha_St;
    public int cha_St_Reg;
    public float cha_Atk;
    public float cri_Chan;
    public float cri_Magn;
    public float move_Speed;
    public float atk_spd;
    public int bullet_Cou;
    public float rer_Spd;
    public string wep_Type;

    public CharacherData()
    {
        user_ID = null;
        cha_Hp = 10.0f;
        cha_St = 10.0f;
        cha_St_Reg = 1;
        cha_Atk = 10.0f;
        cri_Chan = 10.0f;
        cri_Magn = 200.0f;
        move_Speed = 10.0f;
    }
}