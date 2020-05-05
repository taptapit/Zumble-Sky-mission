using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class SphereBehaviour : MonoBehaviour, IMovingObject
{
    public TypesSphere TypeSphere { get; set; }                     //тип
    public int ID { get; set; }                                     //ID
    public GameObject FrontBall { get; set; }                       //сусід попереду
    public GameObject BackBall { get; set; }                        //сусід позаду
    public int RespIndex { get; set; }                              //індекс респавна шара, за замовчуванням 0

    public List<Transform> pathPoints;      //точки маршруту
    public int destPointIndex;                                      //індекс поточної точки, до якої здійснювати рух
    private bool isRun;                                             //Чи почато рух(також для завдання руху та зупинки)
    internal bool isDirection;                                      //тип руху

    internal float distanceTreshold = 0.9f;                           //допуск, при якому сфера переключиться на рух до наступної точки.
    private float stopDistanceTreshold = 0.1f;

    public float radius = 0.8f;

    public bool isLerp = false;                                     //зглажування траекторії. За замовчуванням вимикаю.
    private float distancePointToPoint;                             //дистанція між точками маршруту, викаростовується в зглажуванні траекторії

    //швидкість
    private float speed;
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    void Update()
    {
        Muving();
    }

    float a = 1.0f;

    virtual public void Muving()
    {
        if (pathPoints == null || !isRun)                             //шлях відсутній або об'єкт не повинен рухатись
            return;

        Quaternion rotY = Quaternion.AngleAxis(a * 4, new Vector3(0, 1, 0));
        Quaternion rotZ = Quaternion.AngleAxis(a, new Vector3(0, 0, 1));
        Vector3 destVector = DestVector();
        if (!isDirection)
        {
            Vector3 relativePos = destVector - transform.position;
            transform.rotation = Quaternion.LookRotation(transform.forward, relativePos);
            transform.position = Vector3.MoveTowards(transform.position, destVector, Speed * Time.deltaTime); //рух до точки
            /* if (BackBall!=null)
            {
                if (Vector3.Distance(BackBall.transform.position, transform.position)> radius*2+ stopDistanceTreshold)
                {
                    Stop();
                }
            } */
        }
        else
        {
            transform.Translate(((DestVector()).normalized) * Speed * Time.deltaTime);  //рух в напрямку
        }
    }

    private Vector3 oldPos;
    private float distanceTraveled = 0.0f;                                       //пройдена дистанція, використовується для обрахунку згладжування
    public Vector3 DestVector()                                                //Поточний вектор призначення
    {
         float distance = this.distance;

        /* if(!isLerp || destPointIndex == pathPoints.Count - 1 || destPointIndex == 0)
         {
             if ((distance < (distanceTreshold + (Speed * Time.deltaTime)) || distance == 0) && destPointIndex < pathPoints.Count - 1)
                 destPointIndex++;
             return pathPoints[destPointIndex].position;
         }
         else if (isNextPointIfLerp && destPointIndex < pathPoints.Count - 1)
         {
             destPointIndex++;
             distance = this.distance;
             distancePointToPoint = Vector3.Distance(pathPoints[destPointIndex].position, transform.position);   //дистанція до наступної точки
             return Vector3.Lerp(pathPoints[destPointIndex].position, pathPoints[destPointIndex + 1].position,
                     (distancePointToPoint - distance) / (distancePointToPoint));
         }

       return pathPoints[destPointIndex].position;*/

        if(destPointIndex < pathPoints.Count - 1)
        {
            if (!isLerp && (distance < (distanceTreshold + (Speed * Time.deltaTime)) || distance == 0))
            {
                destPointIndex++;   //дистанція до наступної точки
            }
            else if (isNextPointIfLerp())
            {
                destPointIndex++;
                distance = this.distance;
                distancePointToPoint = Vector3.Distance(pathPoints[destPointIndex].position, transform.position);   //дистанція до наступної точки
            }
        }


        /*if (destPointIndex < pathPoints.Count - 1)
        {
            if (isLerp && destPointIndex != 0)
            {
                if (isNextPointIfLerp)
                {
                    destPointIndex++;
                    distance = this.distance;
                    distancePointToPoint = Vector3.Distance(pathPoints[destPointIndex].position, transform.position);   //дистанція до наступної точки
                    return Vector3.Lerp(pathPoints[destPointIndex-1].position, pathPoints[destPointIndex].position,
                                 (distancePointToPoint - distance) / (distancePointToPoint));
                }
            }
            else if (distance < (distanceTreshold + (Speed * Time.deltaTime)) || distance == 0)
                destPointIndex++;
        }*/

         if (!isLerp || destPointIndex == pathPoints.Count - 1 || destPointIndex == 0)
              return pathPoints[destPointIndex].position;                    //наступна точка призначення

        // Debug.Log("distance=" + distance);
        // Debug.Log("1_lerp=" + (distancePointToPoint - distance) / (distancePointToPoint));
        //Lerp
        //(distancePointToPoint-distance)/(distancePointToPoint)
         return Vector3.Lerp(pathPoints[destPointIndex].position, pathPoints[destPointIndex + 1].position,
                            (distancePointToPoint - distance) / (distancePointToPoint));
    }

    private bool isNextPointIfLerp()
    {
        if (destPointIndex == 0) return true;
        else return Vector3.Distance(pathPoints[destPointIndex - 1].position, pathPoints[destPointIndex].position) > distance;
    }

    private float distance => Vector3.Distance(pathPoints[destPointIndex].position, transform.position);

    //наказ рухатись по напрямку 
    public void Move(Transform direction)
    {
        isDirection = true;
        isRun = true;

        if (direction == null)
            return;

        pathPoints.Clear();
        pathPoints.Add(direction);
    }

    //наказ рухатись по шляху
    public void Move(List<Transform> path, bool isRevers)
    {
        isDirection = false;
        isRun = true;
        pathPoints = path;
    }

    public void Move(List<Transform> path, bool isRevers, float speed)
    {
        Speed = speed;
        Move(path, isRevers);
    }

    //наказ зупинитись
    public void Stop()
    {
        isRun = false;
       // Debug.Log("stoped");
    }
    //додати тимчасовий об'єкт, який буде зберігати задню кулю. Це потрібно що б відміряти дистанцію відносно фіксованої кулі
    public void MoveOneStep()
    {
        if (FrontBall!=null)
        {
            FrontBall.GetComponent<SphereBehaviour>().MoveOneStep();
        }
        StartCoroutine(TestCoroutine(true));
    }

    public IEnumerator TestCoroutine(bool isFirst)
    {
        while (true)
        {
            yield return null;
            Speed = 5.0f;

          /*  if (FrontBall != null)
            {
                FrontBall.GetComponent<SphereBehaviour>().StartCoroutine(FrontBall.GetComponent<SphereBehaviour>().TestCoroutine(true));
            }*/

          /* if (Vector3.Distance(BackBall.transform.position, transform.position)>=(3.6f-Speed*Time.deltaTime) && isFirst)
           {
               Speed = 1.0f;// BackBall.GetComponent<SphereBehaviour>().Speed;
               yield break;
           }
           else if(!IsMovedForwardOneBall)
                yield break;*/
        }
    }

    /*void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "ball")
            IsMovedForwardOneBall = false;
    }*/

    internal void CheckToDestroy(bool isForward, SphereBehaviour sb)        //виклик по ланцюгу перевірки на один колір
    {
        if (sb.TypeSphere != TypeSphere)                        //перервати виконання, в разі якщо кулі не однакові
            return;

        BallController.GetBalls(RespIndex).Add(gameObject);

        if (isForward && FrontBall != null)
            FrontBall.GetComponent<SphereBehaviour>().CheckToDestroy(true, this);
        else if (BackBall != null)
            BackBall.GetComponent<SphereBehaviour>().CheckToDestroy(false, this);
    }

    /*void OnCollisionExit2D(Collision2D collBall)
    {
        if (FrontBall == null)
            return;

        Debug.Log("STOP");
        if (collBall.gameObject == FrontBall)
            Stop();
    }
    */
   /* void OnCollisionEnter2D(Collision2D collBall)
    {
        if (BackBall == null)
            return;

        Debug.Log("START");
        if (collBall.gameObject == BackBall)
            Move(pathPoints, false);
    }*/
    
   /* void OnCollisionStay2D(Collision2D collBall)
    {
        if (BackBall == null)
            return;

        Debug.Log("START");
        if (collBall.gameObject == BackBall)
            Move(pathPoints, false);
    }*/
}