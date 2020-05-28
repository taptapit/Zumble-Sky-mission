using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMulticolor : PlayerSphereBehaviour
{
    public int powerMulticolorSkill;                //встановити при виклику з геймконтрола

    override internal void PlayerCheckToDestroy()
    {
        Debug.Log("BallMulticolor Type=" + TypeSphere);

        List<GameObject> destroyList = new List<GameObject>();
        destroyList = BallController.GetBalls(RespIndex);

        if (powerMulticolorSkill < 1)
            destroyList.Add(gameObject);
        else
            destroyList.Add(null);

        powerMulticolorSkill--;
        BallController.lastForwardBallIndex = 0;

        if (FrontBall != null)
            FrontBall.GetComponent<BallBehaviour>().CheckToDestroy(true, this);
        else
            BallController.readyToDestroy = true;       //якщо передніх куль немає (і їх не потрібно зсувати) - то розблокувати процес знищення куль

        if (BackBall != null)
            BackBall.GetComponent<BallBehaviour>().CheckToDestroy(false, this);
        else
            BallController.readyToDestroy = true;       //якщо задніх куль не має і зсуву передніх робити також не потрібно - то розблокувати процес знищення куль

    }
}
