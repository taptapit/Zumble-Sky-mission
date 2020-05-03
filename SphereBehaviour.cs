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

    public float distanceTreshold = 0.8f;                           //допуск, при якому сфера переключиться на рух до наступної точки. Оптимально має дорівнювати радіусу 

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

    float a=1.0f;

    virtual public void Muving()
    {
        if (pathPoints == null || !isRun)                             //шлях відсутній або об'єкт не повинен рухатись
            return;

        Quaternion rotY = Quaternion.AngleAxis(a*4, new Vector3(0, 1, 0));
        Quaternion rotZ = Quaternion.AngleAxis(a, new Vector3(0, 0, 1));
        if (!isDirection)
        {
                Vector3 relativePos = pathPoints[destPointIndex].position - transform.position;
                transform.rotation = Quaternion.LookRotation(transform.forward, relativePos);
                transform.position = Vector3.MoveTowards(transform.position, DestVector(), Speed * Time.deltaTime); //рух до точки
        }
        else
        {
            transform.Translate(((DestVector()).normalized) * Speed * Time.deltaTime);  //рух в напрямку
        }
    }

    public Vector3 DestVector()                                                //Поточний вектор призначення
    {
        if ((Vector3.Distance(pathPoints[destPointIndex].position, transform.position) < Speed*Time.deltaTime) && destPointIndex < pathPoints.Count - 1)
            destPointIndex++;

        Debug.Log("SET destPointIndex 2 =" + destPointIndex+" Type: "+TypeSphere);

        return pathPoints[destPointIndex].position;                    //наступна точка призначення
    }

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
    }

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
            Speed = 5.2f;

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
}