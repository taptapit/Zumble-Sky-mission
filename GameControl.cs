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
    private GameObject launchSphere;
    public Transform moveTo;                 //змінити (костиль в якості пустого об'єкта до якого рухатись. ставиться в редакторі)
                                             // SphereRespawn[] respawns;
    SaveInfo save;
    SaveLoadGame saveLoadManager;

    //Ефекти
    public GameObject explode;
    public AudioSource destroyBallSound;
    public AudioSource launchBallSound;
    public AudioSource salutSound;
    public GameObject winSalut;

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
            return speed + (float)save.skill[(int)PlayerSkill.SK_BONUS_SPEED] * 1.1f; 
        }
        set 
        {
            speed = value;
        }
    }

    BallCreator ballCreator = new BallCreator();

    void Awake()
    {
        // BallController.RedyToRunNewPlayerBall=true;     //костиль, виклик конструктора
        ballCreator = new BallCreator();         
    }

    void Start()
    {
       // Debug.Log("-=3=-");
      //  BallController.RedyToRunNewPlayerBall = false;

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
        newSphere = ballCreator.getBall(ballTransform, map.pointToRespawnPlayersBall.position, typeSphere).gameObject;
       // newSphere.GetComponent<BallBehaviour>().TypeSphere = typeSphere;
        //  destroyLists = BallController.BallsLists;
    }

    //BallController.BallsLists - списки з кулями.
    // 1-4 списки (індекса 0-3) зарезервовані під респауни з тими ж індексами
    // 5-тий список (індекс 4) - зарезервований під взаємодію куль (крайніх куль в локальних послідовностях)
    // 6-тий список (індекс 5) - для безумовного знищення куль.
    void Update()
    {
        if (map.CountDestroyBalls>4)
              GameOverProcess();

        if (score >= scoreToFinal)
        {
            scoreToFinal += 1000;
            GameWinProcess();
        }
        
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

        if (launchSphere == null)
        {
            BallController.RedyToRunNewPlayerBall = true;
        }
        else if(launchSphere.tag!="player")
            BallController.RedyToRunNewPlayerBall = true;

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

        //Звук
        if (destroyBallSound != null)
            destroyBallSound.Play();

        //Анімація
        if (explode != null)
        {
            Vector3 particlePos = new Vector3(ball.transform.position.x, ball.transform.position.y, -2);
            GameObject explodeParticleSystem = Transform.Instantiate(explode, particlePos, Quaternion.identity);
            Destroy(explodeParticleSystem.gameObject, 0.5f);
        }
        Destroy(ball);
    }

    void OnMouseDown()
    {
        /*if (launchSphere == null)
        {
            BallController.RedyToRunNewPlayerBall = true;
        }
        else if (launchSphere.tag != "player")
            BallController.RedyToRunNewPlayerBall = true;*/

        if (!BallController.RedyToRunNewPlayerBall)
             return;

        //Debug.Log("-=2=-");
        BallController.RedyToRunNewPlayerBall = false;

        if (moveTo==null)
        {
            Debug.Log("не вказаний moveTo об'єкт в редакторі");
            return;
        }

//      moveToPos.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z)); 
        moveTo.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        moveTo.position = new Vector3(moveTo.position.x-map.player.transform.position.x, moveTo.position.y- map.player.transform.position.y, 0);

        //Debug.Log("coord X=" + moveTo.position.x);
        //Debug.Log("coord Y=" + moveTo.position.y);
        //Vector3 relativePos = moveTo.position - player.transform.position;
        // transform.rotation = Quaternion.LookRotation(player.transform.forward, relativePos);
        // player.transform.LookAt((new Vector3(moveTo.position.x, moveTo.position.y, player.transform.position.z ));
        player.transform.rotation = Quaternion.LookRotation(transform.forward, new Vector3(moveTo.position.x, moveTo.position.y, player.transform.position.z));
        newSphere.transform.position = map.pointToRespawnPlayersBall.position;
        //newSphere.tag = "player";
        SphereBehaviour sb  =  newSphere.GetComponent<SphereBehaviour>();
        sb.Speed = Speed;
        sb.Move(moveTo);
        launchSphere = newSphere;

        //Звук
        if (launchBallSound!=null)
            launchBallSound.Play();

        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
            newSphere = ballCreator.getBall(ballTransform, map.pointToRespawnPlayersBall.position, typeSphere).gameObject;
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

    //private Vector3 BallPosicion() => new Vector3(player.transform.position.x-0.2f, player.transform.position.y+3.1f, -2.0f);

    //повертає мапу, відповідно до рівня
    private GameObject createMapGameobject(int curMapLvl)
    {
        int count = mapsList.Count;

        Debug.Log("curMapLvl=" + curMapLvl);
        Debug.Log("mapsList.Count=" + mapsList.Count);
        if (curMapLvl < 10)
            count = mapsList.Count > 7 ? 7 : mapsList.Count;

        Debug.Log("count=" + count);

        int randomMapIndex = Random.Range(0, count);

        Debug.Log("randomMapIndex=" + randomMapIndex);
        //randomMapIndex = 0;
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
        if (curLvl < 15)
            CountColor = 4;
        else
            countColor = 5;

        foreach (var resp in map.sphereRespawnsList)
            resp.CountColor = CountColor;

        scoreToFinal = 100 + ((curLvl * curLvl)/3);                           //бали які необхідно заробити до перемоги в рівні         

        scoreProgressBar.currentValue = score;
        scoreProgressBar.fullValue = scoreToFinal;

        if (map.sphereRespawnsList != null)
            foreach (var resp in map.sphereRespawnsList)
            {
                resp.BaseSpeed = 0.8f + (float)curLvl / 125.0f;
                foreach (var triger in resp.accTrigger)
                {
                    triger.currentMapLvl = curLvl;
                }
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
            score = (score * 110) / 100;

        save.playerExp += (score * 8);

        save.maxOpenLvl++;
        SaveLoadGame.SaveGame(save);
        StartCoroutine(GameWinProcessCoroutine());

        //Debug.Log("-=1=-");
        BallController.RedyToRunNewPlayerBall = false;

        if (winSalut != null)
            winSalut.SetActive(true);

        //Звук
        if (salutSound != null)
            salutSound.Play();

        if (map.sphereRespawnsList != null)
            foreach (var resp in map.sphereRespawnsList)
                resp.Speed = 0;

        //Очистка 5-го списка. Знищення наявних на екрані куль
        if (BallController.BallsLists[4] != null)
        {
            for (int i = 0; i < BallController.BallsLists[4].Count; i++)
            {
                if (BallController.BallsLists[4][i] != null)
                    DestroyProcess(BallController.BallsLists[4][i]);
            }
        }
    }

    public IEnumerator GameWinProcessCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(7.0f);
            SceneManager.LoadScene(0);
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

    public void OnButtonBeaver()
    {
        if (save.curCountBeaver < 1)
            return;

        Destroy(newSphere);
        TypesSphere typeSphere = TypesSphere.BEAVER;
        newSphere = ballCreator.getBall(beaverBallTransform, map.pointToRespawnPlayersBall.position, typeSphere).gameObject;

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
        newSphere = ballCreator.getBall(multicolorBallTransform, map.pointToRespawnPlayersBall.position, typeSphere).gameObject;
        BallMulticolor advencedBall = newSphere.GetComponent<BallMulticolor>();
        advencedBall.powerMulticolorSkill = save.skill[(int)PlayerSkill.SK_MULTICOLOR_POWER];

        Debug.Log("newSphere type="+ newSphere.GetComponent<BallBehaviour>().TypeSphere);
        save.curCountMulticolor--;
    }
    public void OnButtonTimestop()
    {
        if (save.curCountTimestop < 1)
            return;

        foreach (var resp in map.sphereRespawnsList)
        {
            if (resp.Speed <= resp.BaseSpeed)
                resp.StartCoroutine(resp.TimestopBallsCoroutine(save.skill[(int)PlayerSkill.SK_BONUS_MAX_TIMESTOP]));
            else
                return;
        }
        save.curCountTimestop--;
    }

    public void OnButtonExplosive()
    {
        if (save.curCountExplosive < 1)
            return;

        Destroy(newSphere);
        TypesSphere typeSphere = TypesSphere.EXPLOSIVE;
        newSphere = ballCreator.getBall(explosiveBallTransform, map.pointToRespawnPlayersBall.position, typeSphere).gameObject;

        BallExplosive advencedBall = newSphere.GetComponent<BallExplosive>();
        advencedBall.rangExplosiveSkill = save.skill[(int)PlayerSkill.SK_BONUS_EXP];
        advencedBall.scoreMessage += AddScore;

        save.curCountExplosive--;
    }
}