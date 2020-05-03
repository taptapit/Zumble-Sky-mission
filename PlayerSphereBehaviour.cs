using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class PlayerSphereBehaviour : SphereBehaviour
{
    private bool isCollidet = false;
    private bool isRotableBall = true;
    private bool isClockwise = false;                   //в яку сторону обертається куля при ударі(залежить від кута удару)
    private SphereBehaviour collBallSb;

    private float r = 1.6f;
    private float angle=0.0f;
    private float angleSpeed => (Speed / 8) * Time.deltaTime;

    private void Start()
    {
     //   forwardBalls = new List<GameObject>();
    }
    public override void Muving()
    {
        if(collBallSb == null || !isRotableBall)
        { 
            base.Muving();
            return;
        }

        var x = Mathf.Cos(angle) * r;                           // ДО перевірки на кут відносно напрямку руху, інакше не стане на дистанцію, якщо ударить в уже потрібний кут
        var y = Mathf.Sin(angle) * r;
        transform.position = new Vector3(x, y) + collBallSb.gameObject.transform.position;

        // зупиняю обертання, якщо куля вирівнялась по напрямку руху кулі, в яку відбувся удар (вектор Z)
        float pathAngle = PathAngle(collBallSb.gameObject);                          //Кут між центром куля що обертається, та напрямком кулі в яку відбувся удар 

        //Debug.Log("path "+pathAngle);

        if (isClockwise?(pathAngle < 15):(pathAngle > 165.0f))         //15 градусів - допустима похибка
        {
            //destPointIndex = SetDestPointIndex(backBallSb);           //визначити точку, до якої рухатись
            isRotableBall = false;                                      //обертатись більше не потрібно

            PlayerCheckToDestroy();
            GetPrefFromBackBall();
            Move(pathPoints, false, Speed);                  //рухатись в напрямку потоку, зі швидкістю потоку 
            return;
        }

        angle += isClockwise ? -angleSpeed:angleSpeed;                  // зміна значення кута
    }

    void OnTriggerEnter2D(Collider2D collBall)
    {

        if (collBall.gameObject.tag != "ball" || isCollidet)                                //чи вдарились в ряд куль
            return;

        /*if (PathAngle(collBall.gameObject) < 90)
            isClockwise = true;*/

        RespIndex = collBall.GetComponent<SphereBehaviour>().RespIndex;                     //отримує індекс списку куль

        isCollidet = true;                                                                  //костиль від спрацювання тригера на двох кулях

        angle = Mathf.Atan2((transform.position.y - collBall.transform.position.y), (transform.position.x - collBall.transform.position.x));

        gameObject.tag = "ball";

        if (!isClockwise)
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
        else
        {
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
       /* else 
            Debug.Log("Front/back NULL");*/
    }

    void GetPrefFromBackBall()                                        //налаштування, які необхідно зробити з новою кулею, якщо вона вбудовуються в ряд
    {
        //BackBall = from;                                            //призначити кулю позаду
        // backBallSb = BackBall.GetComponent<SphereBehaviour>();     //отримати скрипт кулі позаду
        //gameObject.tag = "ball";
        RespIndex = collBallSb.RespIndex;
        pathPoints.AddRange(collBallSb.pathPoints);
        Debug.Log("count points_0 = " + collBallSb.pathPoints.Count);
        Debug.Log("count points_1 = " + pathPoints.Count);

        destPointIndex = SetDestPointIndex(collBallSb);
        Debug.Log("SET destPointIndex 1 =" + destPointIndex);
        if (BackBall!=null)
        {
            Speed = BackBall.GetComponent<SphereBehaviour>().Speed;
        }
        else
            Speed = collBallSb.Speed;
    }

    private int SetDestPointIndex(SphereBehaviour collBallSb)              //визначення точки, до якої рухатись
    {
        if (collBallSb.gameObject==FrontBall)
        {
            return collBallSb.destPointIndex;
          //  Debug.Log("collBallSb.gameObject==FrontBall");
        }
        //Debug.Log("pathPoints.Count ="+ pathPoints.Count);
        if (collBallSb.pathPoints.Count > collBallSb.destPointIndex+1)         //якщо точка не остання
        {

            //пошук найближчою точки для руху на відстані не менше визначених трешхолдів 
            //(радіус задньої сфери плюс діаметер поточної - гарантія що не поїде проти руху)
            float distance = Vector3.Distance(collBallSb.pathPoints[collBallSb.destPointIndex].position, collBallSb.transform.position);                  //дистанція до точки для руху попередньої сфери                                                
            for (int i = collBallSb.destPointIndex; i < collBallSb.pathPoints.Count-1; i++)      
            {
                distance += Vector3.Distance(collBallSb.pathPoints[i].position, collBallSb.pathPoints[i+1].position);                  //дистанція сумується для наступних точок, на випадок якщо шлях різко повертає і наступна точка шляху може бути ближче ніж попередня
                if (distance > distanceTreshold * 3.5)                 //якщо дистанція достатня, то рухатись до даної точки
                {
                    Debug.Log("i="+i);
                    //Debug.Log("distance="+ distance);
                    Debug.Log("collBallSb.destPointIndex=" + collBallSb.destPointIndex);
                    Debug.Log("count points_result = "+ pathPoints.Count);
                    return i;
                }
            }
        }
      //  Debug.Log("RETURN " + backBallSb.destPointIndex);
        return collBallSb.destPointIndex;             //якщо точка була останьою, то взяти її з кулі позаду
    }

    private void PlayerCheckToDestroy()
    {
        List<GameObject> destroyList = new List<GameObject>();
        destroyList = BallController.GetBalls(RespIndex);
        destroyList.Add(gameObject);
        //Debug.Log(TypeSphere);

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
}