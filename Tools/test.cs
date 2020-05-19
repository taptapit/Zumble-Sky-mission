using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class test : MonoBehaviour
{
   // float speed = 2.0f;
    //float r = 5.0f;
    public float angleX = 0;
    public float angleY = 0;
    public float angleZ = 0;

    public GameObject target1;
    public GameObject target2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = target1.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(transform.forward, relativePos);
        Vector3 targetDir = target1.transform.position - transform.position;
        float angle = Vector3.Angle((target2.transform.position - transform.position), target2.transform.forward);
        angle = Mathf.Atan2((transform.position.y - target2.transform.position.y), (transform.position.x - target2.transform.position.x));
        if (angle < 0) angle+=Mathf.PI*2;
        angle = (angle * 180 / Mathf.PI);


        Debug.Log(angle);

        // angle *= 180 / Mathf.PI;
        /*if (angle<0)
        {
            angle += 180;
        }*/
        // Debug.Log("angle_p3 =" + angle);

        // float angle2 = Mathf.Atan2((target1.transform.position.y- transform.position.y), ( target1.transform.position.x- transform.position.x));
        // Debug.Log($"angle o_1_1={angle}");
        //Debug.Log($"angle o_1_2={angle2}");
        // Debug.Log(Vector3.Distance(target1.transform.position, transform.position));

        // Debug.Log(relativePos);

        /* angle = Vector3.Angle((target2.transform.position - transform.position), transform.right);
         angle2 = Mathf.Atan2((target2.transform.position.y - transform.position.y), (target2.transform.position.x - transform.position.x));

         Debug.Log($"angle o_2_1={angle}");
         Debug.Log($"angle o_2_2={angle2}");

         angle = Vector3.Angle((target2.transform.position - target1.transform.position), target1.transform.right);
         angle2 = Mathf.Atan2((target2.transform.position.y - target1.transform.position.y), (target2.transform.position.x - target1.transform.position.x));

         Debug.Log($"angle t-t_1={angle}");
         Debug.Log($"angle t-t_2={angle2}");*/


    }
}
