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
    private int ballCount;                   // ID шара
    private BallBehavior frontBall;         // передній шар

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
        SetBallPropertis(ballCreator.getBall(ballTransform, transform.position, TypesSphere.EMPTY).gameObject); // перша сфера
    }

    //Генерує сферу, якщо попередня виходить за межі колайдера спавна (респавн повинен мати статичний колайдер!)
    void OnTriggerExit2D(Collider2D previos)
    {
        if (countOfBalls <= ballCount || previos.tag!="ball")
            return;

        SetBallPropertis(ballCreator.getBall(ballTransform, transform.position).gameObject);
        ballCount++;
    }

    void SetBallPropertis(GameObject ball)                          //Передача параметрів кулі
    {
        BallBehavior sb = ball.GetComponent<BallBehavior>();
        sb.Move(pathPoints, false);                                 //шлях, по якому рухатись
        //sb.ID = ballID;
        sb.Speed = Speed;
        sb.RespIndex = BallController.BallsListIndex;               //

        if (frontBall != null)
        {
            sb.FrontBall = frontBall.gameObject;
            frontBall.BackBall = ball;
        }
        frontBall = sb;

        balls.Add(ball);
        BallController.AddBallsList(balls);
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