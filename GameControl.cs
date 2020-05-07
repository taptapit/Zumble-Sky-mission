using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class GameControl : MonoBehaviour
{
    public Transform ballTransform;         //Яку кулю створити. Встановлюється в редакторі                
    private GameObject newSphere;
    public Transform moveTo;                //змінити (костиль в якості пустого об'єкта до якого рухатись. ставиться в редакторі)
                                            // SphereRespawn[] respawns;

    private List<GameObject>[] destroyLists;

    public float speed;
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    BallCreator ballCreator = new BallCreator();

   private void Start()
   {
        TypesSphere typeSphere = ballCreator.randomType(true);
        newSphere = ballCreator.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0), typeSphere).gameObject;
        newSphere.GetComponent<BallBehaviour>().TypeSphere = typeSphere;

        destroyLists = BallController.BallsLists;
    }

    void OnMouseDown()
    {
      /*  if(!BallController.redyToRunNewPlayerBall)
             return;*/

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

        TypesSphere typeSphere = ballCreator.randomType(true);
        newSphere = ballCreator.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0), typeSphere).gameObject;
        sb = newSphere.GetComponent<PlayerSphereBehaviour>();
        sb.TypeSphere = typeSphere;
        //Debug.Log(sb.TypeSphere);
        // newSphere = null;                   //прибрати

       // BallController.redyToRunNewPlayerBall = false;
    }

    //BallController.BallsLists - списки з кулями.
    // 1-4 списки (індекса 0-3) зарезервовані під респауни з тими ж індексами
    // 5-тий список (індекс 4) - зарезервований під взаємодію куль (крайніх куль в локальних послідовностях)
    // 6-тий список (індекс 5) - для безпечного знищення куль. (безпосереднє знищення по тригеру може викликати "The object of type 'GameObject' has been destroyed but you are still trying to access it.") 
    void FixedUpdate()
    {
        if (BallController.BallsLists == null)
            return;

        if (!BallController.readyToDestroy)
        {
           // SafeBallDestroy();
            return;
        }

        BallController.readyToDestroy = false;      //закрити доступ до перевірки на знищення, до наступного відкриття в коді

        for (int i = 0; i < 4; i++)
        {
            if (destroyLists[i] == null)
                continue;

            // Debug.Log("3_destroyList.count=" + destroyList.Count);

            if (destroyLists[i].Count < 3)
            {
                destroyLists[i].Clear();
                continue;
            }
            //Debug.Log(BallController.lastForwardBallIndex);
            //Destroy(balls[BallController.lastForwardBallIndex], 0.2f);
            GameObject localLastBall = destroyLists[i][BallController.lastForwardBallIndex].GetComponent<BallBehaviour>().FrontBall;

            for (int j = 0; j < destroyLists[i].Count; j++)
            {
               // Debug.Log(destroyList[i].GetComponent<BallBehaviour>().TypeSphere);
                Destroy(destroyLists[i][j]);
            }

            /* if (localLastBall != null)
             {
                 localLastBall.GetComponent<BallBehaviour>().IsLocalLastBall = true;
             }*/

            destroyLists[i].Clear();
        }

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