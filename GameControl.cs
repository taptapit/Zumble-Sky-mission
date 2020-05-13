using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class GameControl : MonoBehaviour
{
    public List<SphereRespawn> respawns;     //точки генерації куль. Встановлюється в редакторі
    //public List<AccTrigger> accTriggers;     //тригери прискорення

    public Transform ballTransform;         //Яку кулю створити. Встановлюється в редакторі                
    private GameObject newSphere;
    public Transform moveTo;                //змінити (костиль в якості пустого об'єкта до якого рухатись. ставиться в редакторі)
                                            // SphereRespawn[] respawns;

    public int countColor;                 //Кількість кольорів у куль. Має бути налаштованим з GameControl
    [SerializeField]
    public int CountColor                   //кількість колькорів, від 1 до 4.
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
        get { return speed; }
        set { speed = value; }
    }

    BallCreator ballCreator = new BallCreator();

    void Awake()
    {
        BallController.redyToRunNewPlayerBall=true;     //костиль, виклик конструктора
        ballCreator = new BallCreator();                //костиль, повторне створення екземпляра
    }

    void Start()
    {
        foreach (var resp in respawns)
        {
            resp.CountColor = CountColor;
        }
        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
        newSphere = ballCreator.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0), typeSphere).gameObject;
        newSphere.GetComponent<BallBehaviour>().TypeSphere = typeSphere;

      //  destroyLists = BallController.BallsLists;
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
       
        //newSphere.tag = "player";
        PlayerSphereBehaviour sb  =  newSphere.GetComponent<PlayerSphereBehaviour>();
        sb.Speed = Speed;
        sb.Move(moveTo);

        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
        newSphere = ballCreator.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0), typeSphere).gameObject;
        sb = newSphere.GetComponent<PlayerSphereBehaviour>();
        sb.TypeSphere = typeSphere;
        //Debug.Log(sb.TypeSphere);
        // newSphere = null;                   //прибрати
    }

    //BallController.BallsLists - списки з кулями.
    // 1-4 списки (індекса 0-3) зарезервовані під респауни з тими ж індексами
    // 5-тий список (індекс 4) - зарезервований під взаємодію куль (крайніх куль в локальних послідовностях)
    // 6-тий список (індекс 5) - для безпечного знищення куль. (безпосереднє знищення по тригеру може викликати "The object of type 'GameObject' has been destroyed but you are still trying to access it.") 
    void Update()
    {
        if (BallController.BallsLists == null)
            return;

        if (!BallController.readyToDestroy)
        {
           // SafeBallDestroy();
            return;
        }

        //return;

        BallController.readyToDestroy = false;      //закрити доступ до перевірки на знищення, до наступного відкриття в коді

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
                    Destroy(BallController.BallsLists[i][j]);
                // Debug.Log(destroyList[i].GetComponent<BallBehaviour>().TypeSphere);
            }

            BallController.BallsLists[i].Clear();

        }
        BallController.redyToRunNewPlayerBall = true;
        //  SafeBallDestroy();
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
}