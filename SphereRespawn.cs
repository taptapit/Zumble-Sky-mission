using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class SphereRespawn : MonoBehaviour
{
    //поля що встановлюються в редакторі
    public List<Transform> pathPoints;      // Точки руху для шару що генерує даний респавн
    public List<GameObject> balls;          //Лист шарів для цього потоку(з цього респавна)
    public Transform ballTransform;         // Шар, який буде створено
    private int ballID;                     // ID шара
    private SphereBehaviour frontBall;      // передній шар
    public int respID;                    // ID респавна     
    public int countOfBalls;                 // кількість шарів з респавна

    //швидкість
    public float speed;
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    BallCreator ballControl = new BallCreator();
    
    [SerializeField]
    public int CountOfBalls
    {
        get { return countOfBalls; }
        set { countOfBalls = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetBallMembers(ballControl.getBall(ballTransform, transform.position, TypesSphere.EMPTY).gameObject); // перша сфера
    }

    //Генерує сферу, якщо попередня виходить за межі колайдера спавна (респавн повинен мати статичний колайдер!)
    void OnTriggerExit2D(Collider2D previos)
    {
        if (countOfBalls <= ballID)
            return;

        SetBallMembers(ballControl.getBall(ballTransform, transform.position).gameObject);
    }
    void SetBallMembers(GameObject ball)
    {
        //Передача параметрів шару
        SphereBehaviour sb = ball.GetComponent<SphereBehaviour>();
        sb.Move(pathPoints, false);                                 //шлях, по якому рухатись
        sb.ID = ballID;
        sb.Speed = Speed;

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
