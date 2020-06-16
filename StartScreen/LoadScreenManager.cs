using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class LoadScreenManager : MonoBehaviour
{
    public Sprite[] advSprites;
    public LoadScreenCamera loadScreenCam;    //для встановлення початкової позиції камери, відносно пройдених рівнів. Встановити в редакторі.

    public SpriteRenderer advSpriteRender;
    private int curAdvIndex;
    
    public Text textCountExplosive;     //текстові поля куль, встановлюються в редакторі
    public Text textCountMulticolor;
    public Text textCountBeaver;
    public Text textCountTimestop;
    
    public Text textPlayerCurLvl;

    public GameObject direction;           //стрілка вказівки що є нерозподілені скіллпоінти. Встановлюється в редакторі

    public Text[] textLvl;

    SaveInfo save;
    SaveLoadGame saveLoadManager;

    // Start is called before the first frame update
    void Start()
    {
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize("3611265");
        }
        //Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        //Advertisement.Banner.Show("banner");

        saveLoadManager = new SaveLoadGame();
        save = saveLoadManager.LoadSave();

        curAdvIndex = UnityEngine.Random.Range(0, 4);
        advSpriteRender.sprite = advSprites[curAdvIndex];

        if (Advertisement.IsReady())
        {
            curAdvIndex = UnityEngine.Random.Range(0, 4);
            advSpriteRender.sprite = advSprites[curAdvIndex];
        }
        else
        {
            advSpriteRender.gameObject.SetActive(false);
        }

        if (save.skillPoint > 0)
            direction.SetActive(true);
        else
            direction.SetActive(false);

        if (loadScreenCam != null)
            for (int i = 0; i < textLvl.Length; i++)
                if (save.maxOpenLvl > (i + 1) * 10)
                    loadScreenCam.targetPos = textLvl[i].transform.position.y + 1.5f;
    }

    bool isStartAdvCor = false;
    // Update is called once per frame
    void Update()
    {
        TextUpdate();
        TextLvlUpdate();

        if (!isStartAdvCor && !advSpriteRender.gameObject.activeSelf)
        {
            StartCoroutine(AdvFinishCorutine());
            isStartAdvCor = true;
        }
    }

    private void TextUpdate()       //оновлення текстових полів головного екрану
    {
        levelUp.CheckLevelUp(save);
        textPlayerCurLvl.text = "LEVEL " + save.playerLvl;
        textCountExplosive.text = save.curCountExplosive + "/" + (save.maxCountExplosive+save.skill[(int)PlayerSkill.SK_BONUS_MAX_EXPLOSIVE]);
        textCountMulticolor.text = save.curCountMulticolor + "/" + (save.maxCountMulticolor + save.skill[(int)PlayerSkill.SK_BONUS_MAX_MULTICOLOR]);
        textCountBeaver.text = save.curCountBeaver + "/" + (save.maxCountBeaver + save.skill[(int)PlayerSkill.SK_BONUS_MAX_BEAVER]);
        textCountTimestop.text = save.curCountTimestop + "/" + (save.maxCountTimestop+save.skill[(int)PlayerSkill.SK_BONUS_MAX_TIMESTOP]);
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

         SaveLoadGame.SaveGame(save);
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
        SaveLoadGame.SaveGame(save);
        SceneManager.LoadScene(2);
    }

    public void button_adv()
    {
        advSpriteRender.gameObject.SetActive(false);
        isStartAdvCor = false;
        if (!AdShow())
            return;

        switch (curAdvIndex)
        {
            case 0:
                save.curCountMulticolor += save.curCountMulticolor < (save.maxCountMulticolor + save.skill[(int)PlayerSkill.SK_BONUS_MAX_MULTICOLOR]) ? 1 : 0;
                break;
            case 1:
                save.curCountBeaver += save.curCountBeaver < (save.maxCountBeaver + save.skill[(int)PlayerSkill.SK_BONUS_MAX_BEAVER] )? 1 : 0;
                break;
            case 2:
                save.curCountExplosive += save.curCountExplosive < (save.maxCountExplosive + save.skill[(int)PlayerSkill.SK_BONUS_MAX_EXPLOSIVE]) ? 1 : 0;
                break;
            case 3:
                save.curCountTimestop += save.curCountTimestop < (save.maxCountTimestop + save.skill[(int)PlayerSkill.SK_BONUS_MAX_TIMESTOP]) ? 1 : 0;
                break;
            default:
                break;
        }
        TextUpdate();

        StartCoroutine(AdvFinishCorutine());
    }

    public IEnumerator AdvFinishCorutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);

            if (Advertisement.IsReady())
            {
                advSpriteRender.gameObject.SetActive(true);

                curAdvIndex = UnityEngine.Random.Range(0, 4);
                advSpriteRender.sprite = advSprites[curAdvIndex];
                yield break;
            }
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if(pause) SaveLoadGame.SaveGame(save);
    }

#endif
    private void OnApplicationQuit()
    {
        SaveLoadGame.SaveGame(save);
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