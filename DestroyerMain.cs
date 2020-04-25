using UnityEngine;

public class DestroyerMain : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "ball")
            Destroy(coll.gameObject);
    }
}
