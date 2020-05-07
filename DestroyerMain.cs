using UnityEngine;
using System.Collections;
using ZLibrary;

public class DestroyerMain : MonoBehaviour
{
    public float lifetime = 0.3f;
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "ball")
        {

            // BallController.BallsLists[5].Add(coll.gameObject);
            var bufer = (GameObject)coll.gameObject;
            /*List<GameObject> balls = BallController.GetBalls(coll.gameObject.GetComponent<SphereBehaviour>().RespIndex);
            balls.Remove(coll.gameObject);*/
            Destroy(bufer);
        }
    }

    public IEnumerator DestroyCoroutine(GameObject ball)
    {

        while (true)
        {
            yield return null;

            if (true)
            {

                yield break;
            }
        }
    }
    //додати знищення на протязі 1-2секунди, якщо інший предмет завис в зіткненні з цим
}