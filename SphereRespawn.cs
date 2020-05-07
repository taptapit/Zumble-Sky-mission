using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class SphereRespawn : MonoBehaviour
{
    BallCreator ballCreator = new BallCreator();

    //поля що встановлюються в редакторі
    public List<Transform> pathPoints;      //Точки руху для шару що генерує даний респавн
    public List<GameObject> balls;          //Лист шарів для цього потоку(з цього респавна)
    public Transform ballTransform;         // Шар, який буде створено
    private int ballCount;                  // 
    private GameObject frontBall;           // передній шар
    public int RespID;                      //ID респауну. Виставляється в редакторі від 0 до 9.

    public float speed;                     //швидкість
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public int countOfBalls;                // кількість шарів з респавна
    [SerializeField]
    public int CountOfBalls
    {
        get { return countOfBalls; }
        set { countOfBalls = value; }
    }

    void Start()
    {
        BallController.AddBallsList(balls, RespID);
        TypesSphere typeSphere = ballCreator.randomType(true);
        SetBallPropertis(ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject, typeSphere); // перша сфера
        ballCount++;
    }

    //Генерує сферу, якщо попередня виходить за межі колайдера спавна (респавн повинен мати статичний колайдер!)
    void OnTriggerExit2D(Collider2D previos)
    {
        if (countOfBalls == ballCount)
        {
            previos.tag = "ball";
            return;
        }

        if (previos.tag != "newBall")
            return;

        TypesSphere typeSphere = ballCreator.randomType(true);
        GameObject ball = ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject;
        SetBallPropertis(ball, typeSphere);
        ballCount++;
        previos.tag = "ball";
        // previos.GetComponent<BallBehaviour>().Stop();
    }

    void SetBallPropertis(GameObject ball, TypesSphere typeSphere)                          //Передача параметрів кулі
    {
        BallBehaviour sb = ball.GetComponent<BallBehaviour>();

       if(pathPoints==null)
            Debug.Log("null");
        sb.Move(pathPoints, false);                                 //шлях, по якому рухатись
        
        sb.Speed = Speed;
        sb.RespIndex = RespID;                                      //Індекс для звернення до масиву зі списком куль відповідає айді респауна
        sb.TypeSphere = typeSphere;

        if (frontBall != null)
        {
            sb.FrontBall = frontBall;
            frontBall.GetComponent<BallBehaviour>().BackBall = ball;
            //   Debug.Log(frontBall.GetComponent<BallBehaviour>().TypeSphere);
        }
        frontBall = sb.gameObject;

        //Debug.Log(sb.TypeSphere);
        // balls.Add(ball);
    }

    //гізмо
    public void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Count < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 1; i < pathPoints.Count; i++)
        {
            Gizmos.DrawLine(pathPoints[i - 1].position, pathPoints[i].position);
        }
    }
}