using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

//Клас реалізує ігрові збереження та завантаження
public class SaveLoadGame
{
    private SaveInfo save = new SaveInfo();
    public SaveInfo Save { get; set; }
    /// public SaveInfo save = new SaveInfo();
    private static string path
    {
        get 
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return Path.Combine(Application.persistentDataPath, "Save.json");
#else
            return Path.Combine(Application.dataPath, "Save.json");
#endif
        }
    }

    /*static SaveLoadGame()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "Save.json");
#else
        path = Path.Combine(Application.dataPath, "Save.json");
#endif
        Debug.Log("path: " + path);

        Save = LoadSave();
    }*/
    
    public SaveInfo LoadSave()
    {/*
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "Save.json");
#else
        path = Path.Combine(Application.dataPath, "Save.json");
#endif
        Debug.Log("path: " + path);*/

        if (!File.Exists(path))
        {
           // Debug.Log("NewSave");
            Save = new PlayerInit();
            return Save;
        }

        Save = JsonUtility.FromJson<SaveInfo>(File.ReadAllText(path));
        return Save;
    }

    public static void SaveGame(SaveInfo save)
    {

        File.WriteAllText(path, JsonUtility.ToJson(save));
    }
}

[Serializable]
public class SaveInfo
{
    public int[] skill = new int[20];  //навички
    public int curLvl;                 //поточний рівень мапи
    public int maxOpenLvl;
    public int playerExp;
    public int playerLvl;
    public int skillPoint;

    public int curCountExplosive;      //поточна кількість вибухівки
    public int maxCountExplosive;      //максимальна кількість вибухівки
    public int strengthExplosive;      //
    public int curCountMulticolor;     //поточна кількість мультиколірних куль
    public int maxCountMulticolor;     //максимальна кількість мультиколірних куль
    public int strengthMulticolor;
    public int curCountBeaver;           //поточна кількість бобрів
    public int maxCountBeaver;           //максимальна кількість бобрів
    public int strengthBeaver;
    public int curCountTimestop;       //поточна кількість зповільнень часу
    public int maxCountTimestop;       //максимальна кількість сповільнень часу
    public int strengthTimestop;

    public float playerBallSpeedBonus;
}