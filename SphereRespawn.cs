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
    private int ballID;                     // ID шара
    private BallBehavior frontBall;         // передній шар
    public int countOfBalls;                // кількість шарів з респавна

    //швидкість
    public float speed;
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    [SerializeField]
    public int CountOfBalls
    {
        get { return countOfBalls; }
        set { countOfBalls = value; }
    }

    public int RespIndex { get; set; }                   // Індекс респавна в листі

    // Start is called before the first frame update
    void Start()
    {
        RespIndex = BallController.BallsListIndex;

        SetBallMembers(ballCreator.getBall(ballTransform, transform.position, TypesSphere.EMPTY).gameObject); // перша сфера
    }

    //Генерує сферу, якщо попередня виходить за межі колайдера спавна (респавн повинен мати статичний колайдер!)
    void OnTriggerExit2D(Collider2D previos)
    {
        if (countOfBalls <= ballID)
            return;

        SetBallMembers(ballCreator.getBall(ballTransform, transform.position).gameObject);
    }
    void SetBallMembers(GameObject ball)
    {
        //Передача параметрів шару
        BallBehavior sb = ball.GetComponent<BallBehavior>();
        sb.Move(pathPoints, false);                                 //шлях, по якому рухатись
        sb.ID = ballID;
        sb.Speed = Speed;
        sb.RespIndex = RespIndex;

        if (frontBall != null)
        {
            sb.FrontBall = frontBall.gameObject;
            sb.FrontBallID = frontBall.ID;
            frontBall.BackBall = ball;
            frontBall.BackBallID = ballID;
        }
        frontBall = sb;

        ballID++;

        balls.Add(ball);
        BallController.setBallList(balls);
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