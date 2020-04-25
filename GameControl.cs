﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class GameControl : MonoBehaviour
{
    public Transform ballTransform;                
    private GameObject newSphere;
    public Transform moveTo;                //змінити (костиль в якості пустого об'єкта до якого рухатись. ставиться в редакторі)
    SphereRespawn[] respawns;

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
        respawns = GetComponents<SphereRespawn>();
    }

    void OnMouseDown()
    {
        Debug.Log($"MousePosZ={Input.mousePosition.z}");

        if (newSphere != null)
        {
            return;
        }

        if (moveTo==null)
        {
            Debug.Log("не вказаний moveTo об'єкт в редакторі");
            return;
        }

        moveTo.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10));
        newSphere = ballControl.getBall(ballTransform, transform.position, TypesSphere.RED).gameObject;
        //newSphere.tag = "player";
        SphereBehaviour sb  =  newSphere.GetComponent<SphereBehaviour>();
        sb.Speed = Speed;
        sb.Move(moveTo);
    }
}
