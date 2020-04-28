using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class SphereBehaviour : MonoBehaviour, IMovingObject
{
    public short TypeSphere { get; set; }                           //тип
    public int ID { get; set; }                                     //ID
    public GameObject FrontBall { get; set; }                       //сусід попереду
    public GameObject BackBall { get; set; }                        //сусід позаду
    public int RespIndex { get; set; }                              //індекс респавна шара, за замовчуванням 0

    public List<Transform> pathPoints;                              //точки маршруту
    internal int destPointIndex;                                    //індекс поточної точки, до якої здійснювати рух
    private bool isRun;                                             //Чи почато рух(також для завдання руху та зупинки)
    internal bool isDirection;                                      //тип руху

    //швидкість
    private float speed;
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    void FixedUpdate()
    {
        Muving();
    }

    float a=1.0f;
    Quaternion or;
    void Start()
    {
        or = transform.rotation;
    }
    virtual public void Muving()
    {
        if (pathPoints == null || !isRun)                             //шлях відсутній або об'єкт не повинен рухатись
            return;

        Quaternion rotY = Quaternion.AngleAxis(a*4, new Vector3(0, 1, 0));
        Quaternion rotZ = Quaternion.AngleAxis(a, new Vector3(0, 0, 1));
        if (!isDirection)
        {
            Vector3 relativePos = pathPoints[destPointIndex].position - transform.position;
            transform.rotation = Quaternion.LookRotation(relativePos);
            transform.position = Vector3.MoveTowards(transform.position, DestVector(), Speed * Time.deltaTime); //рух до точки
            //transform.LookAt(pathPoints[destPointIndex]);*/
            //transform.rotation *=rotY ;
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, pathPoints[destPointIndex].rotation, Time.deltaTime*2.0f);

            /*  Vector3 targetDir = pathPoints[destPointIndex].position - transform.position;
                float step = 2.0f * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
                Debug.DrawRay(transform.position, newDir, Color.red);
                transform.rotation = rotY * Quaternion.LookRotation(newDir);*/
            // a += 1.0f;
        }
        else
        {
            transform.Translate(((DestVector()).normalized) * Speed * Time.deltaTime);  //рух в напрямку

        }
    }

    public Vector3 DestVector()                                                //Поточний вектор призначення
    {
        if ((Vector3.Distance(pathPoints[destPointIndex].position, transform.position)<Speed*Time.deltaTime ) && destPointIndex < pathPoints.Count - 1)
            destPointIndex++;

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

    //наказ зупинитись
    public void Stop()
    {
        isRun = false;
    }

    bool IsMoveForwardOneBall = false;

    public void MoveForwardOneBall()
    {
        IsMoveForwardOneBall = true;
    }

    public IEnumerator TestCoroutine(bool isFirst)
    {
        while (true)
        {
            yield return null;
            Speed = 5.0f;

            //if (Vector3.Distance(BackBall.transform.position, transform.position)>=(isFirst?4.0f:2.0f)/*gameObject.GetComponent<CircleCollider2D>().radius*(isFirst?4:2)*/)
            //{
              // Speed = 1.0f;// BackBall.GetComponent<SphereBehaviour>().Speed;
             //}
        }
    }
}