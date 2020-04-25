using UnityEngine;
using System.Collections.Generic;

public class Destroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        Destroy(coll.gameObject);
    }
}
