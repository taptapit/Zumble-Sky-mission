using UnityEngine;
using System.Collections.Generic;
using ZLibrary;

public class DestroyerMain : MonoBehaviour
{
    public float lifetime = 0.2f;
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "ball")
        {
            var bufer = (GameObject)coll.gameObject;
            /*List<GameObject> balls = BallController.GetBalls(coll.gameObject.GetComponent<SphereBehaviour>().RespIndex);
            balls.Remove(coll.gameObject);*/
            Destroy(bufer, lifetime);
        }
    }
}