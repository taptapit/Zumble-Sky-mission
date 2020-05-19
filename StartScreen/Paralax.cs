using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    private float lengthX, lengthY, startPositionX, startPositionY;
    public GameObject camera;
    public float parallaxEffect;
    public float speed;                                         // швидкість руху вліво

    void Start()
    {
        startPositionX = transform.position.x;
        startPositionY = transform.position.y;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float temp = (camera.transform.position.y) * (1-parallaxEffect);
        float dist = camera.transform.position.y * parallaxEffect;

        transform.position = new Vector3(transform.position.x-speed*Time.deltaTime, startPositionY + dist, transform.position.z);

       // Debug.Log("temp="+ temp);
      //  Debug.Log("startPosition + length=" + startPositionY + lengthY);
        //Debug.Log("startPosition - length=" + startPositionY + lengthY);

        if (temp > startPositionY + lengthY) startPositionY += lengthY;
        else if (temp <= startPositionY - lengthY) startPositionY -= lengthY;

        if (startPositionX - transform.position.x >= lengthX)
            transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
    }
}
