using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    //встановити в редакторі
    public List<SphereRespawn> sphereRespawnsList; 
    public List<DestroyerMain> destroyerMainsList;
    public GameObject player;                           //об'єкт гравця
    public Transform pointToRespawnPlayersBall;                  //точка з об'єкта гравця, в якій створювати кулі

    private int countDestroyBalls;           //кількість знищених куль

    public int CountDestroyBalls 
    {
        get
        {
            return countDestroyBalls;
        }
    }

    private void Update()
    {
        if (destroyerMainsList != null)
        {
            countDestroyBalls = 0;
            foreach (var destroyer in destroyerMainsList)
                countDestroyBalls += destroyer.countDestroyBalls;
        }
    }
}
