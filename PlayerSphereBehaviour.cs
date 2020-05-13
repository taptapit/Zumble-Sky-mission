using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class PlayerSphereBehaviour : BallBehaviour
{
    public bool isCollidet = false;                      //зіткнення вже відбулося (костиль від повторного зіткнення з іншою кулею)
    internal bool isRotableBall = true;                  //чи ще крутиться куля навколо іншої
    private bool isClockwise = false;                    //в яку сторону обертається куля при ударі(залежить від кута удару)
    public bool isRealDistanceMeasurement;               //вимірювання дистанції для розрахунку точки руху. false: рахувати куля - точка, true: рахувати повний шлях по сумі елементів шляху 

    private BallBehaviour collBallSb;                   //Куля, в яку вдарилась куля гравця

    private float angle;                                //Кут удару
    private float angleSpeed => (Speed / 3) * Time.deltaTime;   //Кутова швидкість обертання кулі після удару

    private const float TPi = Mathf.PI * 2;

    public override void Muving()
    {
        if (collBallSb == null || !isRotableBall)
        {
            base.Muving();
            return;
        }
        transform.position = MuvingOnCircle(angle, radius, false, false) + collBallSb.gameObject.transform.position;

        float pathAngle = PathAngle(collBallSb.gameObject);                          //Кут між центром куля що обертається, та напрямком кулі в яку відбувся удар 

        if (isClockwise ? (pathAngle <= 20) : (pathAngle >= 160))
        {
            isRotableBall = false;                                      //обертатись більше не потрібно

            gameObject.tag = "ball";                        //Помітити як звичайну кулю

            GetPrefFromBackBall();

            if (!isClockwise)
                angle = PointToPointAngle(BackBall.transform.position, pathPoints[destPointIndex].position);
            else
                angle = PointToPointAngle(FrontBall.transform.position, pathPoints[destPointIndex].position);

            transform.position = MuvingOnCircle(angle, radius, isClockwise, true) + collBallSb.gameObject.transform.position;

            if (!isClockwise)
            {
                pathAngle = PathAngle(collBallSb.gameObject);

                angle = angle + ((((180 - pathAngle) / 2) * Mathf.PI) / 180.0f);

                transform.position = MuvingOnCircle(angle, radius+0.01f, isClockwise, true) + collBallSb.gameObject.transform.position;
            }

            PlayerCheckToDestroy();                          // провести перевірку чи не потрібно знищити кулі
            Move(pathPoints, 1 , Speed);                   //рухатись в напрямку потоку, зі швидкістю потоку 
          
            return;
        }

        angle += isClockwise ? -angleSpeed : angleSpeed;                  // зміна значення кута
    }

    private Vector3 MuvingOnCircle(float angle, float radius, bool isClockwise, bool isSpeedCorection)      //Отримання координат для руху по колу
    {
        float x = Mathf.Cos(angle) * (radius * 2 + (isSpeedCorection ? Speed * Time.deltaTime : 0)) * (isClockwise ? -1 : 1);
        float y = Mathf.Sin(angle) * (radius * 2 + (isSpeedCorection ? Speed * Time.deltaTime : 0)) * (isClockwise ? -1 : 1);
        return new Vector3(x,y,0);
    }

    public override void CollisionOfBalls(GameObject collBall)
    {
        if (collBall.tag != "ball")                      //Тільки при ударі в кулю
            return;

        if(isCollidet)
        {
            base.CollisionOfBalls(collBall);           //Якщо зіткнення вже відбувалось, то опрацьовувати як звичайну кулю
            return;
        }

        isCollidet = true;                             //Помітити що зіткнення відбулось, та викликати базову реалізацію

        angle = Mathf.Atan2((transform.position.y - collBall.transform.position.y), (transform.position.x - collBall.transform.position.x));
        if (angle < 0)
            angle += TPi;

        GameObject collidetBall = collBall;
        collBallSb = collidetBall.GetComponent<BallBehaviour>();

        //Три основні варіанти зіткнення - з заходом в перед кулі, в зад кулі та в перед кулі але з перенаправленням на задню кулю (для обрахунку більшості ударів одноманітним чином - з закотом проти часової стрілки)
        if (PathAngle(collBall) >= 90)                          //1. Удар в передню частину кулі
        {
            BackBall = collBall;
            collBallSb = BackBall.GetComponent<BallBehaviour>();

            if (collBallSb.FrontBall != null)
            {
                FrontBall = collBallSb.FrontBall;               //призначити передню кулю, якщо вона є
                FrontBall.GetComponent<BallBehaviour>().BackBall = gameObject;
            }
            collBallSb.FrontBall = gameObject;                  //призначити поточну кулю як нову передню для задньої
        }
        else if (collBallSb.BackBall != null)                   //2. Псевдо удар в задню частину кулі, який рахується як удар в передню частину задньої кулі
        {
            //Debug.Log("point 0");
            FrontBall = collBallSb.gameObject;
            BackBall = collBallSb.BackBall;

            float beta = Mathf.Atan2((BackBall.transform.position.y - collBall.transform.position.y), (BackBall.transform.position.x - collBall.transform.position.x));
            if (beta < 0)
                beta += TPi;

            if (beta < Mathf.PI)
                angle = beta - (Mathf.PI * 4.0f / 3.0f);
            else
                angle = beta + Mathf.PI - (Mathf.PI / 3.0f);

            collBallSb.BackBall = gameObject;
            collBallSb = BackBall.GetComponent<BallBehaviour>();

            collBallSb.FrontBall = gameObject;
        }
        else                                                    //3. Дійсний удар в задню частину кулі, якщо інших варіантів не лишилось(удар в хвіст потоку, або по одиночній кулі)
        {
            isClockwise = true;
            //Debug.Log("REAL BACK");
            FrontBall = collBall;
            collBallSb = FrontBall.GetComponent<BallBehaviour>();
            if (collBallSb.BackBall != null)
            {
                BackBall = collBallSb.BackBall;
                BackBall.GetComponent<BallBehaviour>().FrontBall = gameObject;
            }
            collBallSb.BackBall = gameObject;
        }

        if (FrontBall != null && BackBall != null)      //Якщо потрібно від'їхати вперед, та звільнити місце
        {
            FrontBall.GetComponent<BallBehaviour>().MoveOneStepForward(this);
        }

        pathPoints = new List<Transform>(collBallSb.pathPoints);

        RespIndex = collBall.GetComponent<BallBehaviour>().RespIndex;                     //отримує індекс списку куль
    }
     
    void GetPrefFromBackBall()                                        //налаштування, які необхідно зробити з новою кулею, якщо вона вбудовуються в ряд
    {
        RespIndex = collBallSb.RespIndex;

        if (BackBall != null)
        {
            BallBehaviour BackBallSb = BackBall.GetComponent<BallBehaviour>();
            Speed = BackBallSb.Speed;
            destPointIndex = SetFirstDestPointIndex(BackBallSb);
            Debug.Log("i_2=" + destPointIndex);

        }
        else                                                        //якщо куля стає останньою в послідовності
        {
            destPointIndex = SetFirstDestPointIndex(collBallSb);
            Speed = collBallSb.Speed;
           // IsLastBallInResp = true;
           /* if (collBallSb.IsLocalLastBall)                         //якщо куля перед нею була позначена як остання в послідовності і нерухома
            {
                collBallSb.IsLocalLastBall = false;
                IsLocalLastBall = true;
            }*/

        }
        baseSpead = collBallSb.baseSpead;
    }

    private int SetFirstDestPointIndex(BallBehaviour collBallSb)              //визначення точки, до якої рухатись
    {
        if (BackBall == null)
            return collBallSb.destPointIndex;

        if (collBallSb.pathPoints.Count > collBallSb.destPointIndex+1)   //якщо точка не остання
        {
            //пошук найближчою точки для руху на відстані не менше визначених трешхолдів 
            //(радіус задньої сфери плюс діаметер поточної - гарантія що не поїде проти руху)
            float distance = Vector3.Distance(collBallSb.gameObject.transform.position, collBallSb.pathPoints[collBallSb.destPointIndex].position);          //дистанція до точки для руху попередньої сфери                                                
            for (int i = collBallSb.destPointIndex; i < collBallSb.pathPoints.Count-1; i++)      
            {
                if (isRealDistanceMeasurement)
                    distance += Vector3.Distance(collBallSb.pathPoints[i].position, collBallSb.pathPoints[i+1].position);                  //дистанція сумується для наступних точок, на випадок якщо шлях різко повертає і наступна точка шляху може бути ближче ніж попередня
                else
                    distance = Vector3.Distance(collBallSb.gameObject.transform.position, collBallSb.pathPoints[i].position);

                if (distance > distanceTreshold * (isRealDistanceMeasurement ? 4.1f : 3.2f))                 //якщо дистанція достатня, то рухатись до даної точки
                {
                    Debug.Log("i="+i);
                    return i;
                }
            }
        }
        return collBallSb.destPointIndex;             //якщо точка була останьою, то взяти її з кулі позаду
    }

    override internal void PlayerCheckToDestroy()
    {
        List<GameObject> destroyList = new List<GameObject>();
        destroyList = BallController.GetBalls(RespIndex);
        destroyList.Add(gameObject);
        //Debug.Log("0_destroyList.count=" + destroyList.Count);
        BallController.lastForwardBallIndex = 0;

        if (FrontBall != null)
            FrontBall.GetComponent<BallBehaviour>().CheckToDestroy(true, this);
        else
            BallController.readyToDestroy = true;       //якщо передніх куль немає (і їх не потрібно зсувати) - то розблокувати процес знищення куль

        if (BackBall != null)
            BackBall.GetComponent<BallBehaviour>().CheckToDestroy(false, this);
        else
            BallController.readyToDestroy = true;       //якщо задніх куль не має і зсуву передніх робити також не потрібно - то розблокувати процес знищення куль
    }

    private float PathAngle(GameObject ball)
    {
            Vector3 targetDir = ball.transform.position - transform.position;                           //Вектор на кулю в яку відбувся удар
            Vector3 toDir = ball.transform.up;                                                          // Напрямок по задній кулі
            return Vector3.Angle(targetDir, toDir);                                                     //Кут між центром куля що обертається, та напрямком кулі в яку відбувся удар 
    }

    private float PointToPointAngle(Vector3 ball, Vector3 point)
    {
        return Mathf.Atan2(( point.y - ball.y), (point.x - ball.x));        
    }

    void OnDestroy()
    {
        BallController.redyToRunNewPlayerBall = true;
    }
}