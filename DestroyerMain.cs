using UnityEngine;
using System.Collections;
using ZLibrary;

public class DestroyerMain : MonoBehaviour
{
    public float lifetime = 0.3f;
    void OnTriggerEnter2D(Collider2D collBall)
    {
        DestroyProcess(collBall.gameObject);
    }

    void OnTriggerStay2D(Collider2D collBall)
    {
        DestroyProcess(collBall.gameObject);
    }

    void DestroyProcess(GameObject ball)
    {
        if (ball.tag == "ball")
        {
            // BallController.BallsLists[5].Add(coll.gameObject);
            //var bufer = (GameObject)coll.gameObject;
            /*List<GameObject> balls = BallController.GetBalls(coll.gameObject.GetComponent<SphereBehaviour>().RespIndex);
            balls.Remove(coll.gameObject);*/
            Destroy(ball);
        }
        else if (ball.tag == "player")
            if (ball.GetComponent<PlayerSphereBehaviour>().isCollidet)
                Destroy(ball);
    }

    /* public IEnumerator DestroyCoroutine(GameObject ball)
     {

         while (true)
         {
             yield return null;

             if (true)
             {

                 yield break;
             }
         }
     }*/
    //додати знищення на протязі 1-2секунди, якщо інший предмет завис в зіткненні з цим
}