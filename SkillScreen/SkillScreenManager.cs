using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class SkillScreenManager : MonoBehaviour
{
    SaveInfo save = new SaveInfo();


    // Start is called before the first frame update
    void Start()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize("3611265");
        }
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show("banner");

        save = SaveLoadGame.Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void button_exit()
    {
        SaveLoadGame.Save(save);
        SceneManager.LoadScene(0);
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if(pause) SaveLoadGame.Save(save);
    }
#endif
    private void OnApplicationQuit()
    {
        SaveLoadGame.Save(save);
    }
}
