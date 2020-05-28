using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ZLibrary;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public List<GameObject> mapsList;                     //префаби можливих мап, встановити в редакторі
    private MapInfo map;                                  //поточна мапа

    public GameObject player;                           //об'єкт гравця, отримується з мапи

    // private List<SphereRespawn> sphereRespawnsList;     //точки генерації куль, береться з мапи 
    //public List<AccTrigger> accTriggers;     //тригери прискорення

    //Кулі гравця. Встановлюється в редакторі
    public Transform ballTransform;   
    public Transform explosiveBallTransform;
    public Transform multicolorBallTransform;
    public Transform beaverBallTransform;

    private GameObject newSphere;
    public Transform moveTo;                 //змінити (костиль в якості пустого об'єкта до якого рухатись. ставиться в редакторі)
                                             // SphereRespawn[] respawns;
    SaveInfo save;
    SaveLoadGame saveLoadManager;

    public Text textCountExplosive;     //текстові поля куль, встановлюються в редакторі
    public Text textCountMulticolor;
    public Text textCountBeaver;
    public Text textCountTimestop;
    public ProgressBar scoreProgressBar;    //прогрес бар набраних балів. Встановити в редакторі
   
    private int score;                                  //поточні зароблені бали
    private int scoreToFinal;                           //бали які необхідно заробити до перемоги в рівні         
    private int countColor;                             //Кількість кольорів у куль
    public int CountColor                               //кількість колькорів, від 1 до 4.
    {
        get { return countColor; }
        set
        {
            if (value > 4)
                countColor = 4;
            else if (value < 1)
                countColor = 1;
            else
                countColor = value;
        }
    }

    // private List<GameObject>[] destroyLists;
    public float speed;
    [SerializeField]
    public float Speed
    {
        get 
        {
            return speed + (float)save.skill[(int)PlayerSkill.SK_BONUS_SPEED] * 2.0f; 
        }
        set 
        {
            speed = value;
        }
    }

    BallCreator ballCreator = new BallCreator();

    void Awake()
    {
       // BallController.redyToRunNewPlayerBall=true;     //костиль, виклик конструктора
        ballCreator = new BallCreator();         
    }

    void Start()
    {
        BallController.redyToRunNewPlayerBall = false;

        if (explosiveBallTransform==null)
        {
            Debug.LogError("explosiveBallTransform==null");
        }
        //Завантажити збереження
        saveLoadManager = new SaveLoadGame();
        save = saveLoadManager.LoadSave();
        Debug.Log(save.curLvl);

        // створити мапу, відповідно до рівня та отримати її скрипт 
        map = createMapGameobject(save.curLvl).GetComponent<MapInfo>();
        player = map.player;

        TextUpdate();
        LevelPreference(save.curLvl);

        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);    
        newSphere = ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject;
       // newSphere.GetComponent<BallBehaviour>().TypeSphere = typeSphere;
        //  destroyLists = BallController.BallsLists;
    }

    //BallController.BallsLists - списки з кулями.
    // 1-4 списки (індекса 0-3) зарезервовані під респауни з тими ж індексами
    // 5-тий список (індекс 4) - зарезервований під взаємодію куль (крайніх куль в локальних послідовностях)
    // 6-тий список (індекс 5) - для безумовного знищення куль.
    void Update()
    {
      /*  if (map.CountDestroyBalls>4)
            GameOverProcess();*/

        if (score > scoreToFinal)
            GameWinProcess();
        
        TextUpdate();

        //Очистка шостого списка
        if (BallController.BallsLists[5] != null)
        {
            foreach (GameObject ball in BallController.BallsLists[5])
                DestroyProcess(ball);

            BallController.BallsLists[5].Clear();
        }

        if (BallController.BallsLists == null)
            return;

        if (!BallController.readyToDestroy)
        {
            // SafeBallDestroy();
            return;
        }

        //return;

        BallController.readyToDestroy = false;      //закрити доступ до перевірки на знищення, до наступного відкриття в коді

        //Очистка списка один-п'ять
        for (int i = 0; i < 4; i++)
        {
            if (BallController.BallsLists[i] == null)
                continue;

            // Debug.Log("3_destroyList.count=" + destroyList.Count);

            if (BallController.BallsLists[i].Count < 3)
            {
                BallController.BallsLists[i].Clear();
                continue;
            }
            //Debug.Log(BallController.lastForwardBallIndex);
            //Destroy(balls[BallController.lastForwardBallIndex], 0.2f);
            // GameObject localLastBall = BallController.BallsLists[i][BallController.lastForwardBallIndex].GetComponent<BallBehaviour>().FrontBall;

            for (int j = 0; j < BallController.BallsLists[i].Count; j++)
            {
                //  if(destroyLists[i][j].GetComponent<BallBehaviour>().Health<1)
                if (BallController.BallsLists[i][j] == null)
                    continue;

                DestroyProcess(BallController.BallsLists[i][j]);
                /*int AdvencedBallIndex = BallController.BallsLists[i][j].GetComponent<BallBehaviour>().AdvencedBallIndex;
                if (AdvencedBallIndex>-1)
                {
                    switch (AdvencedBallIndex)
                    {
                        case 0:
                            save.curCountMulticolor += save.curCountMulticolor < (save.maxCountMulticolor + save.skill[(int)PlayerSkill.SK_BONUS_MAX_MULTICOLOR]) ? 1 : 0;
                            break;
                        case 1:
                            save.curCountBeaver += save.curCountBeaver < (save.maxCountBeaver + save.skill[(int)PlayerSkill.SK_BONUS_MAX_BEAVER]) ? 1 : 0;
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
                }
                Destroy(BallController.BallsLists[i][j]);*/
                score++;
            }
            BallController.BallsLists[i].Clear();
        }
        BallController.redyToRunNewPlayerBall = true;
        //  SafeBallDestroy();
    }

    private void DestroyProcess(GameObject ball)
    {
        int AdvencedBallIndex = ball.GetComponent<BallBehaviour>().AdvencedBallIndex;
        if (AdvencedBallIndex > -1)
        {
            switch (AdvencedBallIndex)
            {
                case 0:
                    save.curCountMulticolor += save.curCountMulticolor < (save.maxCountMulticolor + save.skill[(int)PlayerSkill.SK_BONUS_MAX_MULTICOLOR]) ? 1 : 0;
                    break;
                case 1:
                    save.curCountBeaver += save.curCountBeaver < (save.maxCountBeaver + save.skill[(int)PlayerSkill.SK_BONUS_MAX_BEAVER]) ? 1 : 0;
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
        }
        Destroy(ball);
    }

    void OnMouseDown()
    {
        if(!BallController.redyToRunNewPlayerBall)
             return;

        BallController.redyToRunNewPlayerBall = false;

        if (moveTo==null)
        {
            Debug.Log("не вказаний moveTo об'єкт в редакторі");
            return;
        }

        moveTo.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));

        Vector3 relativePos = moveTo.position - player.transform.position;
        transform.rotation = Quaternion.LookRotation(player.transform.forward, relativePos);

        //newSphere.tag = "player";
        SphereBehaviour sb  =  newSphere.GetComponent<SphereBehaviour>();
        sb.Speed = Speed;
        sb.Move(moveTo);

        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
        newSphere = ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject;
        sb = newSphere.GetComponent<SphereBehaviour>();
        //sb.TypeSphere = typeSphere;
        //Debug.Log(sb.TypeSphere);
        // newSphere = null;                   //прибрати
    }

    /*void SafeBallDestroy()
    {
        if (destroyLists[5].Count > 0)                             //безпечне знищення куль, що підійшли до точки знищення
        {
            for (int i = 0; i < destroyLists[5].Count; i++)
                if (destroyLists[5][i] != null)
                    Destroy(destroyLists[5][i]);
            destroyLists[5].Clear();
        }
    }*/

    //повертає мапу, відповідно до рівня
    private GameObject createMapGameobject(int curMapLvl)
    {
        int randomMapIndex = Random.Range(0, mapsList.Count - 1);
        return GameObject.Instantiate(mapsList[randomMapIndex]);
    }

    private void TextUpdate()       //оновлення текстових полів головного екрану
    {
        textCountExplosive.text = save.curCountExplosive.ToString();// + "/" + (save.maxCountExplosive + save.skill[(int)PlayerSkill.SK_BONUS_MAX_EXPLOSIVE]);
        textCountMulticolor.text = save.curCountMulticolor.ToString() /*+ "/" + (save.maxCountMulticolor + save.skill[(int)PlayerSkill.SK_BONUS_MAX_MULTICOLOR])*/;
        textCountBeaver.text = save.curCountBeaver.ToString() /*+ "/" + (save.maxCountBeaver + save.skill[(int)PlayerSkill.SK_BONUS_MAX_BEAVER])*/;
        textCountTimestop.text = save.curCountTimestop.ToString() /*+ "/" + (save.maxCountTimestop + save.skill[(int)PlayerSkill.SK_BONUS_MAX_TIMESTOP])*/;

        scoreProgressBar.currentValue = score;
        //Debug.Log("scoreProgressBar.currentValue = "+scoreProgressBar.currentValue);
        //Debug.Log("score = " + score);
    }

    private void LevelPreference(int curLvl)
    {
        if (curLvl < 11)
            CountColor = 3;
        else if (curLvl < 21)
            CountColor = 4;
        else
            countColor = 5;

        foreach (var resp in map.sphereRespawnsList)
            resp.CountColor = CountColor;

        scoreToFinal = 200 + (curLvl * curLvl);                           //бали які необхідно заробити до перемоги в рівні         

        scoreProgressBar.currentValue = score;
        scoreProgressBar.fullValue = scoreToFinal;

        if (map.sphereRespawnsList != null)
            foreach (var resp in map.sphereRespawnsList)
            {
                resp.BaseSpeed = 0.4f + (float)curLvl / 50.0f;
            }
    }

    public void AddScore(int count)
    {
        score += count > 0 ? count : 0;
        Debug.Log("Score=" + score);
    }

    private void GameOverProcess()
    {
        SaveLoadGame.SaveGame(save);
        SceneManager.LoadScene(0);
    }

    private void GameWinProcess()
    {
        if ((float)save.skill[(int)PlayerSkill.SK_BONUS_EXP] > 0)
            score = (score*110)/100;

        save.playerExp += (score * 3);

        save.maxOpenLvl++;
        SaveLoadGame.SaveGame(save);
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

    public void OnButtonBeaver()
    {
        if (save.curCountBeaver < 1)
            return;

        Destroy(newSphere);
        TypesSphere typeSphere = TypesSphere.BEAVER;
        newSphere = ballCreator.getBall(beaverBallTransform, transform.position, typeSphere).gameObject;

        BallBeaver advencedBall = newSphere.GetComponent<BallBeaver>();
        advencedBall.PowerBeaverSkill = save.skill[(int)PlayerSkill.SK_BEAVER_POWER];
        advencedBall.scoreMessage += AddScore;

        save.curCountBeaver--;
    }

    public void OnButtonMulticolor()
    {
        if (save.curCountMulticolor < 1)
            return;

        Destroy(newSphere);
        TypesSphere typeSphere = TypesSphere.MULCOLOR;
        newSphere = ballCreator.getBall(multicolorBallTransform, transform.position, typeSphere).gameObject;
        BallMulticolor advencedBall = newSphere.GetComponent<BallMulticolor>();
        advencedBall.powerMulticolorSkill = save.skill[(int)PlayerSkill.SK_BONUS_MAX_MULTICOLOR];

        Debug.Log("newSphere type="+ newSphere.GetComponent<BallBehaviour>().TypeSphere);
        save.curCountMulticolor--;
    }
    public void OnButtonTimestop()
    {
        if (save.curCountTimestop < 1)
            return;

        foreach (var resp in map.sphereRespawnsList)
            resp.StartCoroutine(resp.TimestopBallsCoroutine(save.skill[(int)PlayerSkill.SK_BONUS_MAX_TIMESTOP]));

        save.curCountTimestop--;
    }

    public void OnButtonExplosive()
    {
        if (save.curCountExplosive < 1)
            return;

        Destroy(newSphere);
        TypesSphere typeSphere = TypesSphere.EXPLOSIVE;
        newSphere = ballCreator.getBall(explosiveBallTransform, transform.position, typeSphere).gameObject;

        BallExplosive advencedBall = newSphere.GetComponent<BallExplosive>();
        advencedBall.rangExplosiveSkill = save.skill[(int)PlayerSkill.SK_BONUS_EXP];
        advencedBall.scoreMessage += AddScore;

        save.curCountExplosive--;
    }
}