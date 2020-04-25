using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSphereBehaviour : SphereBehaviour
{
    private void Start()
    {
        /*SphereRespawn sr = GetComponent<SphereRespawn>();
        Debug.Log($"rID={sr.respID}");*/
    }
    void OnTriggerEnter2D(Collider2D collBall)
    {
        if (!isDirection || collBall.gameObject.tag != "ball")
            return;

        BallBehavior collSb = collBall.GetComponent<BallBehavior>();
        gameObject.tag = "ball";
        isDirection = false;
        destPointIndex = collSb.destPointIndex;
        Move(collSb.pathPoints, false);

        Debug.Log(collSb.RespIndex);
    }
}
