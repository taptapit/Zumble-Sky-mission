using UnityEngine;

public class Destroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        /*if (coll.gameObject.tag != "player")
            return;*/

        Destroy(coll.gameObject);
        BallController.RedyToRunNewPlayerBall = true;
    }
}
