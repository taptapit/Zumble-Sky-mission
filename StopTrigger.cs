using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
   // public delegate void TriggerHandler(float Speed);
   // public event TriggerHandler triggerMessage;

    void OnTriggerEnter2D(Collider2D collBall)
    {
      //  Debug.Log("1_OnTriggerEnter2D");
        if (collBall.gameObject.tag != "ball")
            return;

        // Debug.Log("2_OnTriggerEnter2D");

        if (collBall.gameObject.GetComponent<BallBehaviour>().isLocalLastBall)
        {
            //triggerMessage?.Invoke(5.0f);
            collBall.gameObject.GetComponent<BallBehaviour>().StopForwardBall(false);
        }
    }
}
