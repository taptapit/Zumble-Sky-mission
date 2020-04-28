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

    BallCreator ballControl = new BallCreator();

   private void Start()
    {
        newSphere = ballControl.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0)).gameObject;
        /*//Встановити індекси в 
        respawns = GetComponents<SphereRespawn>();
        for (int i = 0; i < respawns.Length; i++)
        {
            respawns[i].RespIndex = i;
            BallController.setBallList(respawns[i].balls);
        }*/
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

        newSphere = ballControl.getBall(ballTransform, new Vector3(transform.position.x, transform.position.y, 0)).gameObject;

        // newSphere = null;                   //прибрати
    }
}
