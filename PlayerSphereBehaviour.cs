using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class PlayerSphereBehaviour : SphereBehaviour
{
    private bool isCollidet = false;
    private void Start()
    {
        /*SphereRespawn sr = GetComponent<SphereRespawn>();
        Debug.Log($"rID={sr.respID}");*/

    }


    void OnTriggerEnter2D(Collider2D collBall)
    {

        if (collBall.gameObject.tag != "ball" || isCollidet)
            return;

        isCollidet = true;      //костиль від спрацювання тригера на двох сферах
        List<GameObject> balls = BallController.GetBalls(collBall.GetComponent<BallBehavior>().RespIndex);
        List<GameObject> forwardBalls = BallController.GetForwardBalls(collBall.GetComponent<BallBehavior>().RespIndex, collBall.gameObject);
        List<GameObject> ballsToDestroy = new List<GameObject>();

       /* if (forwardBalls != null)
        {
             Debug.Log("forwardBalls="+forwardBalls.Count);
            for (int i = 0; i < forwardBalls.Count-1; i++)
            {
                forwardBalls[i].GetComponent<BallBehavior>().TypeSphere = forwardBalls[i+1].GetComponent<BallBehavior>().TypeSphere;
                
            }
        }*/

          if (forwardBalls != null)
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
        }

          Debug.Log("To Destroy: "+ballsToDestroy.Count);
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


        Destroy(gameObject);
    }
}
