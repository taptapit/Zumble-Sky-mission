using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZLibrary;

public class BallBehavior : SphereBehaviour
{
    public int RespIndex { get; set; }            //індекс респавна шара, за замовчуванням 0

    private TypesSphere typeSphere;
    public TypesSphere TypeSphere 
    {
        get
        {
            return typeSphere;
        }
        set
        {
            typeSphere = value;
            BallCreator bc = new BallCreator();
            bc.ChengeColor(gameObject, typeSphere);

        }
    } 
}
