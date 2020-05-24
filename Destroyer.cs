using UnityEngine;
using System.Collections.Generic;
//using ZLibrary;

public class Destroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        Destroy(coll.gameObject);
        BallController.redyToRunNewPlayerBall = true;
    }
}
