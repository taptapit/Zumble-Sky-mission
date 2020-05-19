using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScreenCamera : MonoBehaviour
{
    private LoadScreenManager loadScreenManger;                  //встановлюється в редакторі
    public SpriteRenderer background;                            //спрайт заднього фона. Встановлюється в редакторі

    private Vector2 startPosition;
    private Camera cam;                                         // скрипт повинен бути компонентом камери
    private float targetPos;                                    // позиція дотику/кліку
    public float speed;                                         // швидкість при плавному скролі
    public float darknesRatio;                                  //швидкість затемнення фону по мірі скролу

    private void Start()
    {
        cam = GetComponent<Camera>();
        targetPos = transform.position.y;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        else if(Input.GetMouseButton(0))
        {
            float curPosition = cam.ScreenToWorldPoint(Input.mousePosition).y - startPosition.y;
            targetPos = Mathf.Clamp(transform.position.y - curPosition,0,12);

          // transform.position = new Vector3(transform.position.x, 
           //                                  targetPos,
           //                                  transform.position.z);

            //зміна відтінку фона на більш темний, по мірі прокрутки вверх
            float changeColor =  Mathf.Clamp(transform.position.y * darknesRatio, 0,210)/255f;
            background.color = new Color(1 - changeColor, 1 - changeColor, 1 - changeColor);
        }
        transform.position = new Vector3(transform.position.x, 
                                       Mathf.Lerp(transform.position.y, targetPos, speed*Time.deltaTime),
                                       transform.position.z);
    }
}