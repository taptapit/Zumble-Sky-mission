using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ZLibrary;

public class SphereRespawn : MonoBehaviour
{
    BallCreator ballCreator = new BallCreator();

    //поля що встановлюються в редакторі
    public List<Transform> pathPoints;      //Точки руху для шару що генерує даний респавн
    public List<BallBehaviour> ballsBh;     //Лист BallBehaviour для цього потоку(з цього респавна) //на даний момент не має практичного застосування
    public Transform ballTransform;         // Шар, який буде створено
    private int ballCount;                  // 
    private GameObject frontBall;           // передній шар
    public int RespID;                      //ID респауну. Виставляється в редакторі від 0 до 9.
  //  private bool isStartVelocity;           //Чи запускати кулі з початковою швидкістю (на старті рівня кулі певний час виїжджають швидше, що б прискорити геймплей)
  //  private bool isFirstRun = true;

    private int countColor;                 //Кількість кольорів у куль. Має бути налаштованим з GameControl

    BallBehaviour ballBehaviour;        //клас поточної кулі, яка запускається з даного респавна

    public bool IsInfiniteLaunch;           //чи запускати кулі безкінечно

    public AccTrigger accTrigger;       //трігер зупинки прискорення, встановлюється в редакторі
    public StopTrigger stopTrigger;

    public int CountColor                   //кількість колькорів, від 1 до 4. Отримується з GameControl
    {
        get { return countColor; }
        set { countColor = value; }
    }

   /* public bool IsStartVelocity
    { 
        get {return isStartVelocity;}
        set
        {
            isStartVelocity = value;
            if (value)
                isFirstRun = true;
        }
    }*/

    private float baseSpeed;                 //базова швидкість
    private float speed;                     //швидкість
    public float Speed
    {
        get { return speed; }
        set 
        { 
            speed = value;
            ballBehaviour?.ChangeSpeedForwardBalls(value, true);
        }
    }

    public float BaseSpeed
    {
        get 
        { 
            return baseSpeed; 
        }
        set 
        {
            Speed = value;
            baseSpeed = value; 
        }
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
        ballCreator = new BallCreator();

        accTrigger.triggerMessage += OnAccTriggerEvent;
        //stopTrigger.triggerMessage += OnStopTriggerEvent;

        Speed = 5.0f;
        //isStartVelocity = true;

        //BallController.AddBallsList(balls, RespID);
        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
        GameObject ball = ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject;
        ballBehaviour = ball.GetComponent<BallBehaviour>();
        ballsBh.Add(ballBehaviour);
        SetBallPropertis(); // перша сфера
        ballCount++;
    }

    //Генерує сферу, якщо попередня виходить за межі колайдера спавна (респавн повинен мати статичний колайдер!)
    void OnTriggerExit2D(Collider2D previos)
    {
        if (countOfBalls == ballCount && !IsInfiniteLaunch)
        {
            previos.GetComponent<BallBehaviour>().IsLastBallInResp = true;
            previos.tag = "ball";
            return;
        }

        if (previos.tag != "newBall")
            return;
        
        TypesSphere typeSphere = ballCreator.randomType(true, CountColor);
        GameObject ball = ballCreator.getBall(ballTransform, transform.position, typeSphere).gameObject;
        ballBehaviour = ball.GetComponent<BallBehaviour>();
        ballsBh.Add(ballBehaviour);
        SetBallPropertis(); // перша сфера
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

    void SetBallPropertis()                          //Передача параметрів кулі
    {
       // balls.Add(ball);
       // BallBehaviour sb = ball.GetComponent<BallBehaviour>();

        if(pathPoints==null)
            Debug.Log("null");
        ballBehaviour.Move(pathPoints, 1);                                 //шлях, по якому рухатись

        ballBehaviour.RespIndex = RespID;                                      //Індекс для звернення до масиву зі списком куль відповідає айді респауна
        // sb.TypeSphere = typeSphere;
        // sb.Health = 1;
        // sb.Health = 1;
        
        if (frontBall != null)
        {
            ballBehaviour.FrontBall = frontBall;
            frontBall.GetComponent<BallBehaviour>().BackBall = ballBehaviour.gameObject;

            //   Debug.Log(frontBall.GetComponent<BallBehaviour>().TypeSphere);
        }

        frontBall = ballBehaviour.gameObject;

        // SetBallVelocity();
        ballBehaviour.Speed = Speed;
        ballBehaviour.BaseSpeed = baseSpeed;

        //Debug.Log(sb.TypeSphere);
    }

    /// int timestopSkill;

    /* private void SetBallVelocity()
     {
        // ballBehaviour.Speed = Speed;
          ballBehaviour.baseSpead = Speed;
         if (IsStartVelocity)
             ballBehaviour.Speed = 5.0f;
         else
         {
             if (isFirstRun)
             {
                 //Debug.Log("isFirstRun. Speed="+ Speed + " balls="+balls.Count);
                 ballBehaviour.ChangeSpeedForwardBalls(Speed);
                 isFirstRun = false;
             }
             else
                 ballBehaviour.Speed = Speed;
         }
     }*/

    private void OnAccTriggerEvent()
    {
        SetAllRespSpeed(BaseSpeed);
        StartCoroutine(CheckCountBallsCoroutine());
        BallController.redyToRunNewPlayerBall = true;
    }
    /*private void OnStopTriggerEvent(float speed)
    {
        accTrigger.isFirstTriggerEnter = true;
        SetAllRespSpeed(speed);
    }*/

    private void SetAllRespSpeed(float speed)
    {
        Speed = speed;
        Debug.Log("SetAllRespVelocity=" + Speed);
        ballBehaviour.ChangeSpeedForwardBalls(Speed, true);
    }

    public IEnumerator TimestopBallsCoroutine(int timestopSkill)
    {
        //Debug.Log("AlignBallsCoroutine");
        while (true)
        {
            SetAllRespSpeed(0.2f);
            Debug.Log(" timestopSkill="+ timestopSkill);
            yield return new WaitForSecondsRealtime(8.0f * (timestopSkill+1));
            Debug.Log(" yield break;");
            SetAllRespSpeed(BaseSpeed);
            yield break;
        }
    }

    public IEnumerator CheckCountBallsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(4.0f);

            List<BallBehaviour> bufer = new List<BallBehaviour>();
            if (ballsBh != null)
            {
                foreach (var ballBh in ballsBh)
                    if (ballBh != null)
                        bufer.Add(ballBh);

                ballsBh.Clear();
                ballsBh.AddRange(bufer);

                if (bufer.Count==1 && accTrigger!=null)
                {
                    BallController.redyToRunNewPlayerBall = false;
                    accTrigger.enableTrigger = true;
                    SetAllRespSpeed(5.0f);
                    bufer.Clear();
                }
            }
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