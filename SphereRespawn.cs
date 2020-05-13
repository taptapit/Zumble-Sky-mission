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
    private bool isStartVelocity;           //Чи запускати кулі з початковою швидкістю (на старті рівня кулі певний час виїжджають швидше, що б прискорити геймплей)

    private int countColor;                 //Кількість кольорів у куль. Має бути налаштованим з GameControl

    public int CountColor                   //кількість колькорів, від 1 до 4. Отримується з GameControl
    {
        get { return countColor; }
        set { countColor = value; }
    }

    public bool IsStartVelocity
    { 
        get {return isStartVelocity;}
        set {isStartVelocity = value;}
    }

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
        isStartVelocity = true;
        //BallController.AddBallsList(balls, RespID);
        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
        GameObject ball = ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject;
        SetBallPropertis(ball, typeSphere); // перша сфера
        ballCount++;
    }

    //Генерує сферу, якщо попередня виходить за межі колайдера спавна (респавн повинен мати статичний колайдер!)
    void OnTriggerExit2D(Collider2D previos)
    {
        if (countOfBalls == ballCount)
        {
            previos.GetComponent<BallBehaviour>().IsLastBallInResp = true;
            previos.tag = "ball";
            return;
        }

        if (previos.tag != "newBall")
            return;
        
        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
        GameObject ball = ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject;
        SetBallPropertis(ball, typeSphere);
        ballCount++;
        previos.tag = "ball";
        //previos.gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
       // previos.GetComponent<BallBehaviour>().Health = 0;
        // previos.GetComponent<BallBehaviour>().Health = 0;

        // previos.GetComponent<BallBehaviour>().Stop();
    }

    void OnTriggerEnter2D(Collider2D collBall)
    {
       /* if (collBall.gameObject.tag == "ball")
            collBall.gameObject.GetComponent<BallBehaviour>().StopForwardBall();*/
    }

    void SetBallPropertis(GameObject ball, TypesSphere typeSphere)                          //Передача параметрів кулі
    {
        balls.Add(ball);
        BallBehaviour sb = ball.GetComponent<BallBehaviour>();

       if(pathPoints==null)
            Debug.Log("null");
        sb.Move(pathPoints, 1);                                 //шлях, по якому рухатись

       
        sb.RespIndex = RespID;                                      //Індекс для звернення до масиву зі списком куль відповідає айді респауна
        sb.TypeSphere = typeSphere;
       // sb.Health = 1;
       // sb.Health = 1;

        if (frontBall != null)
        {
            sb.FrontBall = frontBall;
            frontBall.GetComponent<BallBehaviour>().BackBall = ball;

            //   Debug.Log(frontBall.GetComponent<BallBehaviour>().TypeSphere);
        }
        frontBall = sb.gameObject;

        SetBallVelocity(sb);

        //Debug.Log(sb.TypeSphere);
        // balls.Add(ball);
    }

    private bool isFirstRun = true;
    private void SetBallVelocity(BallBehaviour sb)
    {

        sb.baseSpead = Speed;
        if (IsStartVelocity)
            sb.Speed = 5.0f;
        else
        {
            if (isFirstRun)
            {
                Debug.Log("isFirstRun. Speed="+ Speed + " balls="+balls.Count);
                sb.ChangeSpeedForwardBalls(Speed);
                isFirstRun = false;
            }
            else
                sb.Speed = Speed;
        }
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