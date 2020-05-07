using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class BallBehaviour : SphereBehaviour
{
    //Змінні для керування переміщеннями куль
    public bool isLocalLastBall;            //остання куля в локальній послідовності
    public bool isAlwaysRun;                 //повинна продовжувати рух, навіть якщо остання в послідовності 
    public short Health;                     //чи має куля запас хітпойнтів. За замовчуванням не має, знищується при першій підтвердженій умові  
    internal float baseSpead;                 //базова швидкість, для відновлення руху з базовою швидкістю в разі потреби

    public void Start()
    {
        baseSpead = Speed;    
    }

    [SerializeField]
    public bool IsLocalLastBall             //властивість для встановлення останньої кулі в локальній послідовності
    {
        get
        {
            return isLocalLastBall;
        }
        set
        {
            isLocalLastBall = value;
            if (value)                          //Якщо остання куля в послідовності  - рекурсивно зупинити передні кулі
            {
                StopForwardBall();
            }
            else                                //Якщо більше не остання куля - рекурсивно почати рух передніх куль
            {
                MoveForwardBall(baseSpead);
            }
        }
    }

    public void StopForwardBall()               //рекурсивна зупинка передніх куль
    {
        if (FrontBall != null)                  //зупинити передню кулю
        {
            FrontBall.GetComponent<BallBehaviour>().StopForwardBall();
        }
        Stop(true);                               //зупинитись самій
    }

    public void MoveForwardBall() => MoveForwardBall(Speed);    //варіант, який використовує власну швидкість кулі

    public void MoveForwardBall(float Speed)    //рекурсивний початок руху передніх куль
    {
        if (FrontBall != null)                  //почати рух передньої кулі
        {
            FrontBall.GetComponent<BallBehaviour>().MoveForwardBall(Speed);
        }
        Move(Speed);                            //продовжити рух самій
        //StopAllCoroutines();
    }

    public void ChangeForwardBallsSpeed(float Speed)    //рекурсивна зміна швидкості без команди на початок руху
    {
        if (FrontBall != null)                 
        {
            FrontBall.GetComponent<BallBehaviour>().ChangeForwardBallsSpeed(Speed);
        }
        this.Speed = Speed;                      //Змінити швидкість без запуску руху
        //StopAllCoroutines();
    }

    public void MoveOneStep(PlayerSphereBehaviour atackBall)
    {
        Speed = 5.0f;
        MoveForwardBall(Speed);
        StartCoroutine(TestCoroutine(atackBall));
    }

    public IEnumerator TestCoroutine(PlayerSphereBehaviour atackBall)
    {
        //Debug.Log("TestCoroutine run");

        while (true)
        {
            yield return null;

             if (Vector3.Distance(BackBall.transform.position, transform.position)>=(radius*2) 
                && !atackBall.isRotableBall)
             {
                Speed = atackBall.Speed;                //повернути базову швидкість
                MoveForwardBall(Speed);                 //дати команду переднім кулям рухатись в звичайному режимі
                BallController.readyToDestroy = true;   //розблокувати перевірку на їх знищення

                yield break;
             }
        }
    }

    void OnTriggerEnter2D(Collider2D collBall)
    {
        CollisionOfBalls(collBall.gameObject);
    }

    virtual public void CollisionOfBalls(GameObject collBall)
    {
        if (collBall.tag != "ball")      //перервати, якщо колізія не з кулею
        {
            return;
        }
     /*   Debug.Log("перевірка що остання куля");
        if (IsLocalLastBall == false)               //перервати, якщо це була не остання куля в локальній послідовності
        {
            Debug.Log("не остання локальна куля");
            return;
        }

        IsLocalLastBall = false;                    //Ця куля - вже не остання
        SphereBehaviour collBallSb = collBall.GetComponent<SphereBehaviour>();
       // Speed = collBallSb.Speed;

        if (BackBall != null)                       //задньої кулі у неї бути не повинно, це перевірка
            Debug.LogError("Повторне призначення задньої кулі");

        BackBall = collBall;                        //встановлення посилання на нову задню кулю 
        collBallSb.FrontBall = gameObject;   */       //передача посилання на цю кулю як передньої, для тої що позаду
    }

    internal void CheckToDestroy(bool isForward, SphereBehaviour sb)        //виклик по ланцюгу перевірки на один колір
    {
        if (sb.TypeSphere != TypeSphere)            //перервати виконання, в разі якщо кулі не однакові
            return;

        //Debug.Log("Add. forward="+ isForward +" Type="+TypeSphere);

        BallController.GetBalls(RespIndex).Add(gameObject);
        //Debug.Log("1_destroyList.count=" + BallController.GetBalls(RespIndex).Count);

        if (isForward)
        {
            if (FrontBall != null)
            {
                BallController.lastForwardBallIndex++;
                FrontBall.GetComponent<BallBehaviour>().CheckToDestroy(true, this);
            }
        }
        else if (BackBall != null)
            BackBall.GetComponent<BallBehaviour>().CheckToDestroy(false, this);
    }

    void OnDestroy()
    {
        if (FrontBall != null)
            FrontBall.GetComponent<BallBehaviour>().IsLocalLastBall = true;
    }
}