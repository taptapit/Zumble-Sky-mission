using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class SphereBehaviour : MonoBehaviour, IBall
{
    public short TypeSphere { get; set; }                           //тип
    public int ID { get; set; }                                     //ID
    public GameObject FrontBall { get; set; }                       //сусід попереду
    public GameObject BackBall { get; set; }                        //сусід позаду
    public int FrontBallID { get; set; }                            //ID сусіда позаду
    public int BackBallID { get; set; }                             //ID сусіда попереду

    public List<Transform> pathPoints;                              //точки маршруту
    //public List<Transform> oldPathPoints;                         //пройдені точки. Використовуються для реверса
    private int destPointIndex;                                     //індекс поточної точки, до якої здійснювати рух
    private bool isRun;                                             //Чи почато рух(також для завдання руху та зупинки)
    private int movDirection=1;                                     //Напрямок руху   
    bool isDirection;                                               //тип руху
   // private IEnumerator<Transform> point;

    //швидкість
    private float speed;                                            
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; } 
    }

    //private bool isLastPoint = false;

    void Update()
    {
        if (pathPoints == null || !isRun)
             return;

        if(!isDirection)
            transform.position = Vector3.MoveTowards(transform.position, destVector(), Speed * Time.deltaTime);
        else
                        //dir = (m_pos - (Vector2)transform.position).normalized;
            transform.Translate(((destVector()).normalized) * Speed * Time.deltaTime);
    }

    //Поточний вектор призначення
    Vector3 destVector()        
     {
         if (pathPoints[destPointIndex].position == transform.position && destPointIndex < pathPoints.Count-1)
             destPointIndex++;

         return pathPoints[destPointIndex].position;
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
        if (isRevers)
            movDirection = -1;
    }

    //наказ зупинитись
    public void Stop()
    {
        isRun = false;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!isDirection || coll.gameObject.tag != "ball")
            return;

        SphereBehaviour collSb = coll.GetComponent<SphereBehaviour>();
        gameObject.tag = "ball";
        isDirection = false;
        destPointIndex = collSb.destPointIndex;
        Move(collSb.pathPoints, false);
    }

    // if


    /* public IEnumerator<Transform> GetNextPathPoint()                //отримати положення наступної точки
     {
         if (pathPoints == null || pathPoints.Count < 1) //
         {
             Debug.Log("1:pathPoints == null");
             yield break;
         }

         while (true)
         {
             Debug.Log("2:while start");
             yield return pathPoints[destPointIndex];

             if (pathPoints.Count == 1)
             {
                 Debug.Log("3:pathPoints.Count == 1");
                 continue;
             }

             if (destPointIndex <= 0)
             {
                 Debug.Log("4:destPointIndex <= 0");
                 movDirection = 1;
             }
             else if (destPointIndex >= pathPoints.Count - 1)
             {
                 Debug.Log("5:destPointIndex >= pathPoints.Count - 1");
                 movDirection = -1;
             }

             destPointIndex += movDirection;
             Debug.Log($"6:destPointIndex += movDirection = {destPointIndex}");
         }
     }*/
}

//інтерполяція, допрацювати по необхідності. Варіант не робочий
/* public static Vector3 Mov(List<Transform> vertices, float startTime, float duration)
 {
     if (Time.time - startTime < duration)
     {
         float posit = (Time.time - startTime) * vertices.Count / duration;

         int lposit = (int)System.Math.Floor(posit);
         int rposit = (int)System.Math.Ceiling(posit);

         Vector3 start = vertices[lposit].position;
         Vector3 end = vertices[rposit].position;

         float factor = posit - lposit;

         return Vector3.Lerp(start, end, factor);
     }
     else
         return vertices[vertices.Count - 1].position;
 }*/
