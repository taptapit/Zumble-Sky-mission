using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collBall)
    {
      //  Debug.Log("1_OnTriggerEnter2D");
        if (collBall.gameObject.tag != "ball")
            return;

       // Debug.Log("2_OnTriggerEnter2D");

        if(collBall.gameObject.GetComponent<BallBehaviour>().isLocalLastBall)
             collBall.gameObject.GetComponent<BallBehaviour>().StopForwardBall(false);
    }
}
