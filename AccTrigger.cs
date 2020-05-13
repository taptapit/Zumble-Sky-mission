using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccTrigger : MonoBehaviour
{

    public SphereRespawn resp;                      //встановлюється в редакторі
    bool isFirstTriggerEnter;

    // Start is called before the first frame update
    void Start()
    {
        isFirstTriggerEnter = true;
    }

    void OnTriggerEnter2D(Collider2D collBall)
    {
        if (!isFirstTriggerEnter)
            return;

        if (collBall.gameObject.tag != "ball")
            return;

         resp.IsStartVelocity = false;
         isFirstTriggerEnter = false;
    }
}
