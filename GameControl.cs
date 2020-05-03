using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class GameControl : MonoBehaviour
{
    public Transform ballTransform;         //Яку кулю створити. Встановлюється в редакторі                
    private GameObject newSphere;
    public Transform moveTo;                //змінити (костиль в якості пустого об'єкта до якого рухатись. ставиться в редакторі)
   // SphereRespawn[] respawns;

    public float speed;
    [SerializeField]
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    BallCreator ballCreator = new BallCreator();

   private void Start()
    {
        TypesSphere typeSphere = ballCreator.randomType(true);
        newSphere = ballCreator.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0), typeSphere).gameObject;
        newSphere.GetComponent<SphereBehaviour>().TypeSphere = typeSphere;
    }

    void OnMouseDown()
    {
      /*  if(newSphere!=null)
            if (newSphere.tag !="ball")
                return;*/

        if (moveTo==null)
        {
            Debug.Log("не вказаний moveTo об'єкт в редакторі");
            return;
        }

        moveTo.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
       
        //newSphere.tag = "player";
        PlayerSphereBehaviour sb  =  newSphere.GetComponent<PlayerSphereBehaviour>();
        sb.Speed = Speed;
        sb.Move(moveTo);

        TypesSphere typeSphere = ballCreator.randomType(true);
        newSphere = ballCreator.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0), typeSphere).gameObject;
        sb = newSphere.GetComponent<PlayerSphereBehaviour>();
        sb.TypeSphere = typeSphere;
        //Debug.Log(sb.TypeSphere);
        // newSphere = null;                   //прибрати
    }

    void Update()
    {
        if (BallController.BallsLists == null)
            return;

        foreach (List<GameObject> balls in BallController.BallsLists)
        {
            if (balls == null)
                continue;

            if (balls.Count < 3)
            {
                balls.Clear();
                continue;
            }

            foreach (GameObject ball in balls)
                Destroy(ball, 0.2f);

            balls.Clear();
        }
    }
}
