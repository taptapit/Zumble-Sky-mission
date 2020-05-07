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

    public List<Transform> pathPoints;                              //точки маршруту
    public int destPointIndex;                                      //індекс поточної точки, до якої здійснювати рух
    private bool isRun;                                             //Чи почато рух(також для завдання руху та зупинки)
    internal bool isDirection;                                      //тип руху

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

    void Update()
    {
        Muving();   
    }

    virtual public void Muving()                                    //процес руху об'єкта
    {
        if (pathPoints == null || !isRun)                           //шлях відсутній або об'єкт не повинен рухатись
            return;

        Vector3 destVector = DestVector();
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
         float distance = this.distance;

        if(destPointIndex < pathPoints.Count - 1)
        {
            if (!isLerp && (distance < (distanceTreshold + (Speed * Time.deltaTime)) || distance == 0))
                destPointIndex++; 
            else if (isNextPointIfLerp())
            {
                destPointIndex++;
                distance = this.distance;
                distancePointToPoint = Vector3.Distance(pathPoints[destPointIndex].position, transform.position);   //дистанція до наступної точки
            }
        }

         if (!isLerp || destPointIndex == pathPoints.Count - 1 || destPointIndex == 0)
              return pathPoints[destPointIndex].position;                    //наступна точка призначення

         return Vector3.Lerp(pathPoints[destPointIndex].position, pathPoints[destPointIndex + 1].position,
                            (distancePointToPoint - distance) / (distancePointToPoint));
    }

    private bool isNextPointIfLerp()            //Чи переключатись на наступну точку руху? Використовується при згладжуванні маршруту
    {
        if (destPointIndex == 0) return true;
        else return Vector3.Distance(pathPoints[destPointIndex - 1].position, pathPoints[destPointIndex].position) > distance;
    }

    private float distance => Vector3.Distance(pathPoints[destPointIndex].position, transform.position);

    public void Move(Transform direction)                   //наказ рухатись по напрямку 
    {
        isDirection = true;
        isRun = true;

        if (direction == null)
            return;

        pathPoints.Clear();
        pathPoints.Add(direction);
    }

    public void Move(List<Transform> path, bool isRevers)     //наказ рухатись по шляху
    {
        isDirection = false;
        isRun = true;
        pathPoints = path;
    }

    public void Move(bool isRevers) => Move(pathPoints, isRevers);
    public void Move() => Move(pathPoints, false);
    public void Move(float speed) => Move(pathPoints, false, speed);
    public void Move(List<Transform> path, bool isRevers, float speed)
    {
        Speed = speed;
        Move(path, isRevers);
    }

    //наказ зупинитись
    public void Stop()                                      //зупинка з збереженням значення швидкості
    {
        isRun = false;
    }

    public void Stop(bool isFullStop)                       //зупинка з можливістю виставити швидкість в нуль
    {
        if (isFullStop)
            Speed = 0;
        isRun = false;
    }
}