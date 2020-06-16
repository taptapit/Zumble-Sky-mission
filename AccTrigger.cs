using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccTrigger : MonoBehaviour
{
    public delegate void TriggerHandler();
    public event TriggerHandler triggerMessage;

    public int minMapLvl = -1;           //мінімальний рівень карти, на якій спрацює цей тригер. Встановити в редакторі
    public int currentMapLvl;           //поточний рівень карти. Передати з геймконтрола

   // public SphereRespawn resp;                      //встановлюється в редакторі
    public bool enableTrigger;

    // Start is called before the first frame update
    void Start()
    {
        enableTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D collBall)
    {
        if (!enableTrigger)
            return;

        if ((minMapLvl < currentMapLvl) || minMapLvl == -1)
        {
            if (collBall.gameObject.tag != "ball")
                return;

            triggerMessage?.Invoke();
            // resp.Speed = resp.BaseSpeed;
            enableTrigger = false;
        }
    }
}