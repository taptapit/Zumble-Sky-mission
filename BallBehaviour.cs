using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ZLibrary;

public class BallBehaviour : SphereBehaviour
{
    //Змінні для керування переміщеннями куль
    public bool isLocalLastBall;            //остання куля в локальній послідовності
    public bool isAlwaysRun;                 //повинна продовжувати рух, навіть якщо остання в послідовності 
    public short Health;                     //чи має куля запас хітпойнтів. За замовчуванням не має, знищується при першій підтвердженій умові  
    internal float baseSpead;                 //базова швидкість, для відновлення руху з базовою швидкістю в разі потреби

    private bool isLastBallInResp = false;       //остання куля з даного респауна

    [SerializeField]
    public bool IsLastBallInResp
    {
        get 
        {
            return isLocalLastBall; 
        }
        set
        {
            if(value)               //якщо остання куля з респауна - заборонити рух назад.
            {
                MoveForwardBall(10.0f, 1, true);
                if (FrontBall != null)
                    FrontBall.GetComponent<BallBehaviour>().IsLastBallInResp = true;
            }
            isLocalLastBall = value;
            Destroy(gameObject, Random.Range(0.5f, 2.5f));
        }
    }
    
    public void Start()
    {
        if(baseSpead==0)
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
            if (isLocalLastBall != value)           
            {
                if (value)                          //Якщо остання куля в послідовності  - рекурсивно зупинити передні кулі
                {
                   // Debug.Log("IsLocalLastBall= " + IsLocalLastBall);
                    BallController.redyToRunNewPlayerBall = false;
                    StopAllCoroutines();
                    MoveForwardBall(10.0f, -1, true);
                }
                else                                //Якщо більше не остання куля - рекурсивно почати рух передніх куль
                {
                    StopAllCoroutines();
                    MoveForwardBall(baseSpead, 1, false);
                    StartCoroutine(AlignBallsCoroutine());
                    PlayerCheckToDestroy();
                    BallController.redyToRunNewPlayerBall = true;
                }
            }
            isLocalLastBall = value;
        }
    }

    public bool alignStart { get; private set; }

    public GameObject SearchFirstBall()                    //Повернути першу кулю в послідовності. Не використовую
    {
         if (FrontBall != null)
            return FrontBall.GetComponent<BallBehaviour>().SearchFirstBall();
         else return gameObject;
    }

     public void MoveReversBackwardBall()                   //Рухатись назад, кулям які позаду. Не використовую
     {
         //Debug.Log("MoveReversBackwardBall: " + TypeSphere);

         Move(5.0f, -1);

         if (BackBall!=null)
         {
             BackBall.GetComponent<BallBehaviour>().MoveReversBackwardBall();
         }
     }

    public void StopForwardBall(bool isSmoothStop)               //рекурсивна зупинка передніх куль
    {
        StopAllCoroutines();

        if (FrontBall != null)                  //зупинити передню кулю
        {
            FrontBall.GetComponent<BallBehaviour>().StopForwardBall(isSmoothStop);
        }

        if (isSmoothStop)
            StartCoroutine(AccelerationCoroutine(Speed, -0.15f, Step));
        else
            Stop(true);                               //зупинитись самій
    }

    public void MoveForwardBall() => MoveForwardBall(Speed, Step, false);    //варіант, який використовує власну швидкість кулі

    public void MoveForwardBall(float speed, int step, bool isSmoothStart)    //рекурсивний початок руху передніх куль
    {
        //StopAllCoroutines();

        //Debug.Log("MoveForwardBall "+TypeSphere);
        if (FrontBall != null)                  //почати рух передньої кулі
        {
            if (FrontBall==BackBall)
            {
                Debug.LogError("задня та передня куля співпадають");
                Destroy(gameObject);
            }
            FrontBall.GetComponent<BallBehaviour>().MoveForwardBall(speed, step, isSmoothStart);
        }

        if (isSmoothStart)
            StartCoroutine(AccelerationCoroutine(speed, 0.25f, step));
        else
            Move(speed, step);                            //продовжити рух самій
    }
    public IEnumerator AccelerationCoroutine(float speed, float acceleration, int step)  //плавний старт/стоп
    {
        float currentSpead = acceleration>0?0:Speed;
        while (true)
        {
            yield return null;

            if (acceleration > 0?currentSpead < speed : currentSpead > 0)
            {
                currentSpead = (currentSpead + acceleration) > 0 ? (currentSpead + acceleration) : 0;
                currentSpead += acceleration;
                acceleration += acceleration>0 ? acceleration : 0;
                Move(currentSpead, step);                        
            }
            else
            {
                //Debug.Log("current stop");
                if (acceleration > 0)
                    Move(speed, step);
                else
                    Stop();
                yield break;
            }
        }
    }

    /* public void MoveAndAlignBalls(float speed, int step, bool isSmoothStart)
     {
         if (FrontBall != null)
             FrontBall.GetComponent<BallBehaviour>().MoveAndAlignBalls(speed, step, isSmoothStart);
         Move(speed, step);

         if (BackBall != null)
         {
             float distance = Vector3.Distance(BackBall.transform.position, transform.position);

             if (distance < ((radius * 2) - 0.2f))
             {
                 MoveForwardBall(4.0f, 1);
                 StartCoroutine(AlignBallsCoroutine());
             }
         }
     }*/
    public void ChangeSpeedForwardBalls(float Speed)    //рекурсивна зміна швидкості без команди на початок руху
    {
       // Debug.Log("Speed "+TypeSphere);
        this.Speed = Speed;                      //Змінити швидкість без запуску руху
        if (FrontBall != null)                 
        {
            FrontBall.GetComponent<BallBehaviour>().ChangeSpeedForwardBalls(Speed);
        }
        //StopAllCoroutines();
    }

    //private bool blockRevers = false;
    public override void Muving()
    {
        //Знову костилі. Запуск з OnDestroy() спрацьовує через раз.. Прийшлося перенести в виклик з апдейт
        if ((BackBall == null && gameObject.tag == "ball" && !IsLastBallInResp && !IsLocalLastBall))  //реверс, якщо немає куль позаду  
        {
              IsLocalLastBall = true;
        }

        /*if (!blockRevers && IsLocalLastBall && destPointIndex == 0 && gameObject.tag == "ball")       //Не дати кулям скочуватись в нульову точку. Вимагає правильної розстановки точок в редакторі.(Можливо є сенс перенести з Update на окремий трігер)
        {
           // Debug.Log("blockRevers = true");
            blockRevers = true;                     // заблокувати повторні виклики запинки куль                
            StopForwardBall();                      // зупинити кулі
        }*/

       /* if(destPointIndex==1)
            gameObject.tag = "ball";*/

        base.Muving();
    }   
    public void MoveOneStepForward(PlayerSphereBehaviour atackBall) //Coroutine запускається з метода, що б не створювати булеву змінну для виконання початкового коду тільки один раз
    {
        //Speed = 5.0f;
        MoveForwardBall(4.0f, 1, false);
        StartCoroutine(MoveOneStepForwardCoroutine(atackBall));
    }
    public IEnumerator MoveOneStepForwardCoroutine(PlayerSphereBehaviour atackBall)
    {
        //  Debug.Log("MoveOneStepForwardCoroutine run");

        while (true)
        {
            yield return null;

            if (atackBall == null)              //Якщо якимось чином куля знищена до break
            {
                EndOneStepCorProc();
                yield break;
            }

            if (( Vector3.Distance(atackBall.gameObject.transform.position, transform.position)>=(radius*1.95f) 
                && !atackBall.isRotableBall))
            {
                EndOneStepCorProc();
                yield break;
            }
        }
    }

    private void EndOneStepCorProc()
    {
        Speed = baseSpead;                      //повернути базову швидкість
        MoveForwardBall(Speed, 1, false);        //дати команду переднім кулям рухатись в звичайному режимі
        StartCoroutine(AlignBallsCoroutine());
    }

   /* public IEnumerator MoveToBackCoroutine()
    {
        while (true)
        {
           // float distance = Vector3.Distance(BackBall.transform.position, transform.position);

            yield return null;

            if (BackBall!=null)
            {
                MoveForwardBall(baseSpead, 1);
                BallController.redyToRunNewPlayerBall = true;
               // Debug.Log("AlignBallsCoroutine "+TypeSphere);
              //  StartCoroutine(AlignBallsCoroutine());
               // if (FrontBall != null)

                yield break;
            }
        }
    }*/

    public IEnumerator AlignBallsCoroutine()
    {
        //Debug.Log("AlignBallsCoroutine");
        while (true)
        {
            yield return null;
            if (BackBall == null)           //повтора перевірка, на випадок якщо задня куля знищена під час виконання
            {
                MoveForwardBall(baseSpead, 1, false);
                BallController.readyToDestroy = true;   //розблокувати перевірку на знищення
                yield break;
            }

            //Правка розсинхрону відстані між кулями
            float distance = Vector3.Distance(BackBall.transform.position, transform.position);

            if (distance < radius * 1.9f)
                    MoveForwardBall(Speed + 2.0f, 1, false);
            else
            {
                //Debug.Log("AlignBallsCoroutine END");
                MoveForwardBall(baseSpead, 1, false);
                BallController.readyToDestroy = true;   //розблокувати перевірку на знищення
                yield break;
            }
        }
    }

    /*  public IEnumerator MoveBackwardCoroutine(PlayerSphereBehaviour atackBall)
     {
         //Debug.Log("TestCoroutine run");

         while (true)
         {
             yield return null;

             if (true)
             {
                 yield break;
             }
         }
     }*/

     // void OnCollisionEnter2D(Collision2D collBall)
    void OnTriggerEnter2D(Collider2D collBall)
    {
        //Debug.Log("OnCollisionEnter2D");
        CollisionOfBalls(collBall.gameObject);
    }

    virtual public void CollisionOfBalls(GameObject collBall)
    {
        if (collBall.tag != "ball" && collBall.tag != "newBall")      //перервати, якщо колізія не з кулею
        {
            //Debug.Log("-=1=-");
            return;
        }

        if (FrontBall == collBall)                  //ігнорувати спрацювання тригера від передньої кулі
            return;

        BallBehaviour collBallSb = collBall.GetComponent<BallBehaviour>();
         //Debug.Log("-=2=-");
        if (!IsLocalLastBall)               //перервати, якщо це була не остання куля в локальній послідовності
        {
          // Debug.Log("не остання локальна куля: "+ IsLocalLastBall +" "+ TypeSphere);
           if(collBallSb.IsLocalLastBall)
           {
                collBallSb.BackBall = gameObject;
                FrontBall = collBall;

                collBallSb.IsLocalLastBall = false;
           }                  
            return;
        }
                      
        if (collBall.GetComponent<BallBehaviour>().FrontBall != null)
        {
            //Debug.LogError("FrontBall != null "+TypeSphere);
            return;
        }
        //Debug.Log("-=4=- "+ TypeSphere);
        
        BackBall = collBall;                        //встановлення посилання на нову задню кулю 
        collBallSb.FrontBall = gameObject;          //передача посилання на цю кулю як передньої, для тої що позаду


        IsLocalLastBall = false;                    //Ця куля - вже не остання

       /* if(collBallSb.IsLocalLastBall)              //перевірка в обох кулях. (тригер не спрацьовує з об'єктом з нульовою швидкціст)
            collBallSb.IsLocalLastBall = false;*/
    }

    internal bool CheckToDestroy(bool isForward, SphereBehaviour sb)        //виклик по ланцюгу перевірки на один колір
    {
        if (sb.TypeSphere != TypeSphere || Health > 0)            //перервати виконання, в разі якщо кулі не однакові
            return false;

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

        return true;
    }

    /* void OnDestroy()
     {
         Debug.Log("OnDestroy()");
         if (FrontBall != null)
         {
             FrontBall.GetComponent<BallBehaviour>().IsLocalLastBall = true;
             Debug.Log("OnDestroy() FRONT="+FrontBall.GetComponent<BallBehaviour>().TypeSphere);
         }
     }*/

    virtual internal void PlayerCheckToDestroy()
    {
        List<GameObject> destroyList = new List<GameObject>();
        destroyList = BallController.GetBalls(RespIndex);
        destroyList.Clear();
        destroyList.Add(gameObject);

        if (FrontBall != null)
            FrontBall.GetComponent<BallBehaviour>().CheckToDestroy(true, this);

        if (BackBall != null)
        {
            if (BackBall.tag != "newBall" && BackBall.GetComponent<BallBehaviour>().CheckToDestroy(false, this))
                BallController.readyToDestroy = true;
            else
                destroyList.Clear();
        }
        else
            destroyList.Clear();
    }
}