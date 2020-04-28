using UnityEngine;
using System.Collections.Generic;
using ZLibrary;

public class DestroyerMain : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "ball")
        {
            List<GameObject> balls = BallController.GetBalls(coll.gameObject.GetComponent<SphereBehaviour>().RespIndex);
            balls.Remove(coll.gameObject);
            Destroy(coll.gameObject);
        }
    }
}