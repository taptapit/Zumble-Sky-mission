using UnityEngine;

public class Clouds : MonoBehaviour
{
    private float lengthX, startPositionX;
    public float speed;                                         // швидкість руху вліво

    void Start()
    {
        startPositionX = transform.position.x;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);

        if (startPositionX - transform.position.x >= lengthX)
            transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
    }
}
