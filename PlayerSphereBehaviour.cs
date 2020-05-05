using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class PlayerSphereBehaviour : SphereBehaviour
{
    private bool isCollidet = false;                    //зіткнення вже відбулося (костиль від повторного зіткнення з іншою кулею)
    private bool isRotableBall = true;                  //чи ще крутиться куля навколо іншої
    private bool isClockwise = false;                   //в яку сторону обертається куля при ударі(залежить від кута удару)
    public bool isRealDistanceMeasurement;              //вимірювання дистанції для розрахунку точки руху. false: рахувати куля - точка, true: рахувати повний шлях по сумі елементів шляху 

    private SphereBehaviour collBallSb;

    private float angle;
    private float angleSpeed => (Speed / 4) * Time.deltaTime;

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

            PlayerCheckToDestroy();
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

                transform.position = MuvingOnCircle(angle, radius, isClockwise, true) + collBallSb.gameObject.transform.position;
            }

            Move(pathPoints, false, Speed);                  //рухатись в напрямку потоку, зі швидкістю потоку 
            return;
        }

        angle += isClockwise ? -angleSpeed : angleSpeed;                  // зміна значення кута
    }

    private Vector3 MuvingOnCircle(float angle, float radius, bool isClockwise, bool isSpeedCorection)
    {
        float x = Mathf.Cos(angle) * (radius * 2 + (isSpeedCorection ? Speed * Time.deltaTime : 0)) * (isClockwise ? -1 : 1);
        float y = Mathf.Sin(angle) * (radius * 2 + (isSpeedCorection ? Speed * Time.deltaTime : 0)) * (isClockwise ? -1 : 1);
        return new Vector3(x,y,0);
    }

     void OnTriggerEnter2D(Collider2D collBall)
     {
         if (collBall.gameObject.tag != "ball" || isCollidet)                                //чи вдарились в ряд куль
             return;

        gameObject.tag = "ball";

        angle = Mathf.Atan2((transform.position.y - collBall.transform.position.y), (transform.position.x - collBall.transform.position.x));
        if (angle < 0)
            angle += TPi;

        GameObject collidetBall = collBall.gameObject;
        collBallSb = collidetBall.GetComponent<SphereBehaviour>();
        if (PathAngle(collBall.gameObject) >= 90)
        {
            BackBall = collBall.gameObject;
            collBallSb = BackBall.GetComponent<SphereBehaviour>();

            if (collBallSb.FrontBall != null)
            {
                FrontBall = collBallSb.FrontBall;                       //призначити передню кулю, якщо вона є
                FrontBall.GetComponent<SphereBehaviour>().BackBall = gameObject;
            }
            collBallSb.FrontBall = gameObject;                          //призначити поточну кулю як нову передню для задньої
        }
        else if (collBallSb.BackBall != null)
        {
            FrontBall = collBallSb.gameObject;
            BackBall = collBallSb.BackBall;

            float beta = Mathf.Atan2((BackBall.transform.position.y - collBall.transform.position.y), (BackBall.transform.position.x - collBall.transform.position.x));
            if (beta < 0)
                beta += TPi;

            if (beta < Mathf.PI)
                angle = beta - (Mathf.PI * 4.0f / 3.0f);
            else
                angle = beta  + Mathf.PI - (Mathf.PI / 3.0f);

            collBallSb.BackBall = gameObject;
            collBallSb = BackBall.GetComponent<SphereBehaviour>();

            collBallSb.FrontBall = gameObject;
        }
        else
        {
            isClockwise = true;

            FrontBall = collBall.gameObject;
            collBallSb = FrontBall.GetComponent<SphereBehaviour>();
            if (collBallSb.BackBall != null)
            {
                BackBall = collBallSb.BackBall;
                BackBall.GetComponent<SphereBehaviour>().FrontBall = gameObject;
            }
            collBallSb.BackBall = gameObject;
        }

        if (FrontBall != null && BackBall!=null)
            FrontBall.GetComponent<SphereBehaviour>().MoveOneStep();

        pathPoints = new List<Transform>(collBallSb.pathPoints);

        RespIndex = collBall.GetComponent<SphereBehaviour>().RespIndex;                     //отримує індекс списку куль

        isCollidet = true;                                                                  //костиль від спрацювання тригера на двох кулях
    }

    void GetPrefFromBackBall()                                        //налаштування, які необхідно зробити з новою кулею, якщо вона вбудовуються в ряд
    {
        RespIndex = collBallSb.RespIndex;

        if (BackBall != null)
        {
            SphereBehaviour BackBallSb = BackBall.GetComponent<SphereBehaviour>();
            Speed = BackBallSb.Speed;
            destPointIndex = SetDestPointIndex(BackBallSb);
        }
        else
        {
            destPointIndex = SetDestPointIndex(collBallSb);
            Speed = collBallSb.Speed;
        }
    }

    private int SetDestPointIndex(SphereBehaviour collBallSb)              //визначення точки, до якої рухатись
    {
        if (BackBall == null)
            return collBallSb.destPointIndex;

        if (collBallSb.pathPoints.Count > collBallSb.destPointIndex+1)         //якщо точка не остання
        {

            //пошук найближчою точки для руху на відстані не менше визначених трешхолдів 
            //(радіус задньої сфери плюс діаметер поточної - гарантія що не поїде проти руху)
            float distance = Vector3.Distance(collBallSb.pathPoints[collBallSb.destPointIndex].position, collBallSb.transform.position);                  //дистанція до точки для руху попередньої сфери                                                
            for (int i = collBallSb.destPointIndex; i < collBallSb.pathPoints.Count-1; i++)      
            {
                if(isRealDistanceMeasurement)
                    distance += Vector3.Distance(collBallSb.pathPoints[i].position, collBallSb.pathPoints[i+1].position);                  //дистанція сумується для наступних точок, на випадок якщо шлях різко повертає і наступна точка шляху може бути ближче ніж попередня
                else
                    distance = Vector3.Distance(collBallSb.gameObject.transform.position, collBallSb.pathPoints[i].position);

                if (distance > distanceTreshold * (isRealDistanceMeasurement?4.2f:3.5f))                 //якщо дистанція достатня, то рухатись до даної точки
                    return i;
            }
        }
        return collBallSb.destPointIndex;             //якщо точка була останьою, то взяти її з кулі позаду
    }

    private void PlayerCheckToDestroy()
    {
        List<GameObject> destroyList = new List<GameObject>();
        destroyList = BallController.GetBalls(RespIndex);
        destroyList.Add(gameObject);

        if (FrontBall != null)
            FrontBall.GetComponent<SphereBehaviour>().CheckToDestroy(true, this);

        if (BackBall != null)
            BackBall.GetComponent<SphereBehaviour>().CheckToDestroy(false, this);
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
}