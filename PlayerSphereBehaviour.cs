using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class PlayerSphereBehaviour : SphereBehaviour
{
    private bool isCollidet = false;
    private bool isRotableBall = true;
    GameObject collidetBall;
    /*private void Start()
    {
        SphereRespawn sr = GetComponent<SphereRespawn>();
        Debug.Log($"rID={sr.respID}");
    }*/
    float r = 2.0f;
    public float angle=0.0f;
    float angle2;

    public override void Muving()
    {
        if(collidetBall == null || !isRotableBall)
        { 
            base.Muving();
            return;
        }

        if (Vector3.Angle((collidetBall.transform.position - transform.position), collidetBall.transform.forward)>178.0f)
        {
            isRotableBall = false;

            BallBehavior collSb = collidetBall.GetComponent<BallBehavior>();

            destPointIndex = (collSb.pathPoints.Count > collSb.destPointIndex+1)?(collSb.destPointIndex + 1): collSb.destPointIndex;


            gameObject.tag = "ball";
            Speed = collSb.Speed;
            Move(collSb.pathPoints, false);
        }

        angle += Time.deltaTime; // меняется плавно значение угла

        var x = Mathf.Cos(angle) * r;
        var y = Mathf.Sin(angle) * r;
        transform.position = new Vector3(x, y) +collidetBall.transform.position;

        //transform.RotateAround(collidetBall.transform.position, Vector3.forward, 2 * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collBall)
    {
        if (collBall.gameObject.tag != "ball" || isCollidet)                        //чи вдарились в ряд куль
            return;

        collidetBall = collBall.gameObject;
        RespIndex = collBall.GetComponent<BallBehavior>().RespIndex;                //отримує індекс списку куль

        isCollidet = true;                                                          //костиль від спрацювання тригера на двох кулях
         List<GameObject> balls = BallController.GetBalls(RespIndex);
         List<GameObject> forwardBalls = BallController.GetForwardBalls(RespIndex, collBall.gameObject);     //передні сфери
         
        angle = Mathf.Atan2((transform.position.y - collBall.transform.position.y), (transform.position.x- collBall.transform.position.x));

        if (forwardBalls != null)
        {
            if (forwardBalls.Count>1)
                for (int i = 0; i < forwardBalls.Count-1; i++)
                {
                    forwardBalls[i].GetComponent<SphereBehaviour>().StartCoroutine(forwardBalls[i].GetComponent<SphereBehaviour>().TestCoroutine(false));
                }
            forwardBalls[forwardBalls.Count-1].GetComponent<SphereBehaviour>().StartCoroutine(forwardBalls[forwardBalls.Count - 1].GetComponent<SphereBehaviour>().TestCoroutine(true));
        }

        /*gameObject.tag = "ball";
        destPointIndex = collBall.GetComponent<SphereBehaviour>().destPointIndex;
        Move(collBall.GetComponent<SphereBehaviour>().pathPoints, false);*/

        //List<GameObject> ballsToDestroy = new List<GameObject>();

        /* if (forwardBalls != null)
         {
              Debug.Log("forwardBalls="+forwardBalls.Count);
             for (int i = 0; i < forwardBalls.Count-1; i++)
             {
                 forwardBalls[i].GetComponent<BallBehavior>().TypeSphere = forwardBalls[i+1].GetComponent<BallBehavior>().TypeSphere;

             }
         }*/

        /* if (forwardBalls != null)
         {
             Debug.Log("forwardBalls="+forwardBalls.Count);
           GameObject prevBall = null;
             foreach (GameObject ball in forwardBalls)
             {
                ballsToDestroy.Add(ball);
               if(prevBall!=null)
                   prevBall.transform.GetComponent<Renderer>().material.color = ball.transform.GetComponent<Renderer>().material.color;
               // balls.Remove(ball);
               //forwardBalls.Remove(ball);
               //Destroy(ball);
               //forwardBalls.Clear();
               prevBall = ball;
             }
          prevBall.gameObject.transform.GetComponent<Renderer>().material.color = gameObject.transform.GetComponent<Renderer>().material.color;
       }*/

        // Debug.Log("To Destroy: "+ballsToDestroy.Count);
        /*
          for (int i = 0; i < ballsToDestroy.Count; i++)
          {
              balls.Remove(ballsToDestroy[i]);
              Destroy(ballsToDestroy[i]);
          }*/



        //   BallBehavior collSb = collBall.GetComponent<BallBehavior>();
        //   gameObject.tag = "ball";
        //   isDirection = false;
        //   destPointIndex = collSb.destPointIndex;
        //    Move(collSb.pathPoints, false);


        // Destroy(gameObject);
    }
}
