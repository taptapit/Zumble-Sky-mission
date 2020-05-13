using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class SphereBehaviour : MonoBehaviour, IMovingObject
{
    // ДЕБАГ
    public TypesSphere forwardType;
    public TypesSphere backType;
    // ДЕБАГ
    public TypesSphere TypeSphere { get; set; }                     //тип
    public int ID { get; set; }                                     //ID
    public GameObject FrontBall { get; set; }                       //сусід попереду
    public GameObject BackBall { get; set; }                        //сусід позаду
    public int RespIndex { get; set; }                              //індекс респавна шара, за замовчуванням 0

    public List<Transform> pathPoints;                              //точки маршруту
    public int destPointIndex;                                      //індекс поточної точки, до якої здійснювати рух
    private bool isRun;                                             //Чи почато рух(також для завдання руху та зупинки)
    internal bool isDirection;                                      //тип руху

    private int step;                                                //крок точки маршруту
    public bool isRevers;                                           //зворотній рух
    private bool oldReversValue = false;                              //Використовується для миттєвої зміни напрямку при реверсі. Зберігає старе значення isRevers

    internal float distanceTreshold = 0.6f;                         //допуск, при якому сфера переключиться на рух до наступної точки.

    public float radius = 0.9f;                                     //радіус сфери для розрахунків

    public bool isLerp = false;                                     //зглажування траекторії. За замовчуванням вимикаю.
    private float distancePointToPoint;                             //дистанція між точками маршруту, викаростовується в зглажуванні траекторії

    //швидкість
    public float speed;
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public int Step
    {
        get { return step; }
        set
        {
            if (step != value && step!=0)
            {

                destPointIndex=SetDestPointIndex(value);
               /* float distance = Vector3.Distance(gameObject.transform.position, pathPoints[destPointIndex].position);
               

                
                if (value < 0)
                {
                    if (destPointIndex > 0)
                        destPointIndex -= 1;

                    Debug.Log("Revers");
                    
                    for (int i = destPointIndex; i > 0; i--)
                    {
                        distance += Vector3.Distance(pathPoints[i].position, pathPoints[i - 1].position);

                        if (distance > distanceTreshold * 2.0f)                 //якщо дистанція достатня, то рухатись до даної точки
                        {
                            destPointIndex = i;
                            Debug.Log(" destPointIndex = "+ destPointIndex);
                            break;
                        }
                    }
                }
                else
                {
                    if (destPointIndex < pathPoints.Count - 1 )
                        destPointIndex += 1;

                    for (int i = destPointIndex; i < pathPoints.Count - 1; i++)
                    {
                        distance += Vector3.Distance(pathPoints[i].position, pathPoints[i + 1].position);

                        if (distance > distanceTreshold * 2.0f)                 //якщо дистанція достатня, то рухатись до даної точки
                        {
                            destPointIndex = i;
                            break;
                        }
                    }
                }*/
            }
            step = value;
           // Debug.Log("i_3=" + destPointIndex);
        }
    }

    private int SetDestPointIndex(int step)
    {
        float distance = Vector3.Distance(gameObject.transform.position, pathPoints[destPointIndex].position);
        if (destPointIndex > 0 || (destPointIndex < pathPoints.Count - 1))
            destPointIndex += step;

        for (int i = destPointIndex; step<0?(i > 0):(i < pathPoints.Count - 1); i+=step)
        {
            distance += Vector3.Distance(pathPoints[i].position, pathPoints[i + step].position);

            if (distance > distanceTreshold * 2.0f)                 //якщо дистанція достатня, то рухатись до даної точки
            {
              //  Debug.Log(" destPointIndex = " + i);
                return i;
            }
        }
        return destPointIndex;
    }

    void Start()
    {
        Step = 1;
    }

    void Update()
    {
        //ДЕБАГ
        if (BackBall != null)
            backType = BackBall.GetComponent<SphereBehaviour>().TypeSphere;
        else
            backType = TypesSphere.EMPTY;

        if (FrontBall != null)
            forwardType = FrontBall.GetComponent<SphereBehaviour>().TypeSphere;
        else
            forwardType = TypesSphere.EMPTY;
        if (FrontBall != null && BackBall != null)
            if (FrontBall == BackBall)
            {
                Debug.LogError("FrontBall == BackBall: " + FrontBall.GetComponent<SphereBehaviour>().TypeSphere);
                Destroy(gameObject);
            }
        //ДЕБАГ

        Muving();
    }

    virtual public void Muving()                                    //процес руху об'єкта
    {
        if (pathPoints == null || !isRun)                           //шлях відсутній або об'єкт не повинен рухатись
            return;

        Vector3 destVector = !isLerp ? DestVector() : DestVectorLerp();
        if (!isDirection)                                           //рух по точках
        {
            Vector3 relativePos = destVector - transform.position;
            transform.rotation = Quaternion.LookRotation(transform.forward, relativePos);
            transform.position = Vector3.MoveTowards(transform.position, destVector, Speed * Time.deltaTime); //рух до поточної точки
        }
        else
        {
            transform.Translate(((destVector).normalized) * Speed * Time.deltaTime);  //рух в напрямку
        }
    }

    public Vector3 DestVector()                                                //Поточний вектор призначення
    {
        //  Debug.Log("P1");
        if ((destPointIndex == pathPoints.Count - 1 && Step > 0) ||
            (destPointIndex == 0 && Step < 0))
            return pathPoints[destPointIndex].position;                 //не міняти точку

        float distance = this.distance();
        // Debug.Log("P2");
        if (distance < (distanceTreshold + (Speed * Time.deltaTime)))
        {
            // Debug.Log(Step);
            destPointIndex += Step;
        }

        return pathPoints[destPointIndex].position;                    //наступна точка призначення
    }

    public Vector3 DestVectorLerp()
    {
        float distance = this.distance();

        if ((destPointIndex == pathPoints.Count - 1 && Step > 0) ||
            (destPointIndex == 0 && Step < 0))
            return pathPoints[destPointIndex].position;                 //не міняти точку

        if (destPointIndex < pathPoints.Count - 1)
            {
                if (isNextPointIfLerp())
                {
                    destPointIndex+=Step;

                    distance = this.distance();
                    distancePointToPoint = Vector3.Distance(pathPoints[destPointIndex].position, transform.position);   //дистанція до наступної точки
                }
            }
        Debug.Log(destPointIndex);
            return Vector3.Lerp(pathPoints[destPointIndex].position, pathPoints[destPointIndex + Step].position,
                               (distancePointToPoint - distance) / (distancePointToPoint));
    }

    private bool isNextPointIfLerp()            //Чи переключатись на наступну точку руху? Використовується при згладжуванні маршруту
    {
        if (destPointIndex == 0) 
            return true;

        return Vector3.Distance(pathPoints[destPointIndex -1].position, pathPoints[destPointIndex].position) > distance();
    }

    private float distance() => Vector3.Distance(pathPoints[destPointIndex].position, transform.position);
    
    public void Move(Transform direction)                   //наказ рухатись по напрямку 
    {
        isDirection = true;
        isRun = true;

        if (direction == null)
            return;

        pathPoints.Clear();
        pathPoints.Add(direction);
    }
    public void Move(List<Transform> path, bool IsRevers)
    {

    }
    public void Move(List<Transform> path, int step)     //наказ рухатись по шляху
    {
        isDirection = false;
        isRun = true;
        pathPoints = path;
        Step = step;
    }
    public void Move(List<Transform> path, int step, float speed)
    {
        Speed = speed;
        Move(path, step);
    }
    public void Move(int step) => Move(pathPoints, step);
    public void Move() => Move(pathPoints, 1);
    public void Move(float speed) => Move(pathPoints, 1, speed);
    public void Move(float speed, int step) => Move(pathPoints, step, speed);

    //наказ зупинитись
    public void Stop()                                      //зупинка з збереженням значення швидкості
    {
        isRun = false;
    }

    public void Stop(bool isFullStop)                       //зупинка з можливістю виставити швидкість в нуль
    {
        Debug.Log("STOP " + TypeSphere);
        if (isFullStop)
            Speed = 0;
        isRun = false;
    }
}