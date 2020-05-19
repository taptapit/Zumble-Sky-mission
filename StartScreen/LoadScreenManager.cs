using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using System;
using System.IO;
public class LoadScreenManager : MonoBehaviour
{
    public Sprite[] advSprites;
    public SpriteRenderer advSpriteRender;
    private int curAdvIndex;
    
    public Text textCountExplosive;     //текстові поля куль, встановлюються в редакторі
    public Text textCountMulticolor;
    public Text textCountBeaver;
    public Text textCountTimestop;
    
    public Text textPlayerCurLvl;

    public Text[] textLvl;

    SaveInfo save = new SaveInfo();

    // Start is called before the first frame update
    void Start()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize("3611265");
        }
        //Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        //Advertisement.Banner.Show("banner");

        save = SaveLoadGame.Load();

        if (save.maxCountMulticolor < 3) save.maxCountMulticolor = 3;
        if (save.maxCountBeaver < 3) save.maxCountBeaver = 3;
        if (save.maxCountExplosive < 3) save.maxCountExplosive = 3;
        if (save.maxCountTimestop < 3) save.maxCountTimestop = 3;

        curAdvIndex = UnityEngine.Random.Range(0, 4);
        advSpriteRender.sprite = advSprites[curAdvIndex];
    }

    // Update is called once per frame
    void Update()
    {
        TextUpdate();
        TextLvlUpdate();
    }

    private void TextUpdate()       //оновлення текстових полів головного екрану
    {
        textPlayerCurLvl.text = "LEVEL " + save.playerLvl;
        textCountExplosive.text = save.curCountExplosive + "/" + save.maxCountExplosive;
        textCountMulticolor.text = save.curCountMulticolor + "/" + save.maxCountMulticolor;
        textCountBeaver.text = save.curCountBeaver + "/" + save.maxCountBeaver;
        textCountTimestop.text = save.curCountTimestop + "/" + save.maxCountTimestop;
    }
    private void TextLvlUpdate()       //оновлення текстових полів головного екрану
    {
        for (int i = 0; i < textLvl.Length; i++)
        {
            if (save.maxOpenLvl > (i+1) * 10)
            {
                textLvl[i].text = "10/10";
                textLvl[i].color = new Color(0.83f, 0.83f, 0.025f);
            }
            else if (save.maxOpenLvl >= (i == 0 ? 0 : (i) * 10))
            {
                textLvl[i].text = (i == 0 ? save.maxOpenLvl : save.maxOpenLvl - (i * 10)) + "/10";
                textLvl[i].color = new Color(0.83f, 0.83f, 0.025f);
            }
            else
            {
                textLvl[i].text = "0/10";
                textLvl[i].color = new Color(0.39f, 0.39f, 0.31f);
            }
        }
    }

    //обробка кнопок
    void StartLvl(int lvl)
    {
        if (lvl * 10 > save.maxOpenLvl)
            return;

        if(lvl==-1)                     //кнопка старт, або поточна максимальна кнопка
            save.curLvl = save.maxOpenLvl + save.maxOpenLvl < 80?1:0;

        if (lvl < save.maxOpenLvl / 10)
        {
            Debug.Log("(lvl) < maxOpenLvl / 10");
            save.curLvl = (lvl * 10) + 10;
        }
        else
            save.curLvl = save.maxOpenLvl + (save.maxOpenLvl < 80 ? 1 : 0);

         SaveLoadGame.Save(save);
         SceneManager.LoadScene(1);
    }
    public void button_start() => StartLvl(-1);
    public void button_1 () => StartLvl(0);
    public void button_2() => StartLvl(1);
    public void button_3() => StartLvl(2); //3
    public void button_4() => StartLvl(3);
    public void button_5() => StartLvl(4);
    public void button_6() => StartLvl(5);
    public void button_7() => StartLvl(6);
    public void button_8() => StartLvl(7);

    public void button_skills()
    {
        SaveLoadGame.Save(save);

        SceneManager.LoadScene(2);
    }

    public void button_adv()
    {

        if (!AdShow())
            return;

        switch (curAdvIndex)
        {
            case 0:
                save.curCountMulticolor += save.curCountMulticolor < save.maxCountMulticolor ? 1 : 0;
                break;
            case 1:
                save.curCountBeaver += save.curCountBeaver < save.maxCountBeaver ? 1 : 0;
                break;
            case 2:
                save.curCountExplosive += save.curCountExplosive < save.maxCountExplosive ? 1 : 0;
                break;
            case 3: 
                save.curCountTimestop += save.curCountTimestop < save.maxCountTimestop ? 1 : 0;
                break;
            default:
                break;
        }
        TextUpdate();

        curAdvIndex = UnityEngine.Random.Range(0, 4);
        advSpriteRender.sprite = advSprites[curAdvIndex];
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

    public static bool AdShow()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
            return true;
        }
        else
        {
            return false;
        }
    }
}