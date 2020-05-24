using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class SkillScreenManager : MonoBehaviour
{
    SaveLoadGame saveLoadManager;
    public SaveInfo save;
    public List<SkillButton> skillButtons;                //кнопки навиків. Встановити в редакторі
    public Text textCountSkillPoint;                    //текстове поле для відображення скіллпоінтів

    public ProgressBar progressBar;                     //прогрес бар до наступного рівня. Встановити в редакторі


    private void Awake()
    {
        saveLoadManager = new SaveLoadGame();
        save = saveLoadManager.LoadSave();
        if (save == null)
        {
            Debug.Log("save==null");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Рекламний банер
        if (Advertisement.isSupported)
        {
            Advertisement.Initialize("3611265");
            StarBanner();
        }
        else
        {
            Debug.Log("Adv not supported");
        }

        if (Advertisement.IsReady() && save.playerLvl>2 && Random.Range(0,1.0f) > (save.playerLvl>6?0.8f:0.9f))
            Advertisement.Show();

            //Debug.Log(save.skill[(int)PlayerSkill.SK_BONUS_SPEED]);
            // Завантаження збереження
            // save = SaveLoadGame.Load();

            levelUp.CheckLevelUp(save);

        // установка значень прогресбара
        int pointToPrevLvl = save.playerLvl < 1 ? 0 : levelUp.pointToNextLvl(save.playerLvl - 1);
        progressBar.fullValue = levelUp.pointToNextLvl(save.playerLvl) - pointToPrevLvl;
        progressBar.currentValue = save.playerExp - pointToPrevLvl;
        //progressBar.alwaysUpdate = true;
        SkillPointTextUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SkillPointTextUpdate()
    {
        if (save.skillPoint > 0)
        {
            textCountSkillPoint.text = save.skillPoint.ToString();
            textCountSkillPoint.color = new Color(0.83f, 0.83f, 0.025f);
        }
        else
        {
            textCountSkillPoint.text = "0";
            textCountSkillPoint.color = new Color(0.39f, 0.39f, 0.31f);
        }
    }

    public void button_exit()
    {
        SaveLoadGame.SaveGame(save);
        Advertisement.Banner.Hide();
        SceneManager.LoadScene(0);
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

    public void UpdateAllSkillButton()
    {
        if(skillButtons!=null)
            foreach (var item in skillButtons)
                item.SkillButtonUpdate();

        SkillPointTextUpdate();
    }

    public void StarBanner()
    {
        StartCoroutine(ShowBannerWhenReady());
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady("banner"))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.Show("banner");
    }
}