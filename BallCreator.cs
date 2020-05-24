using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ПОВЕДІНКА ОБ'ЄКТА ЩО РУХАЄТЬСЯ
//Кожному об'єкту може бути заданий рух. Рух задається за переданою множиною точок, або в напрямку
//Швідкість може бути задана окремо
//Об'єкт може бути зупинений
//Має текстову властивість "тип сфери", що зберігає задані кольори, так і вказує на інші варіанти подібних об'єктів, наприклад "мультиколірна сфера", "сфера вибухівка", "ракета")
//Куля має дві сусідні кулі - "передню" та "задню". Має властивості для цього значення (сусідів може не бути).
public interface IMovingObject
{
     Vector3 DestVector();                              //поточна точка призначення
     void Move(Transform direction);                      //рух по напрямку
     void Move(List<Transform> path, int Step);      //рух по списку точок
     void Stop();                                         //стоп
     GameObject FrontBall { get; set; }                   //куля попереду
     GameObject BackBall { get; set; }                    //куля позаду
     float Speed { get; set; }                            //швидкість
     TypesSphere TypeSphere { get; set; }                       //тип
}

public enum TypesSphere
{
    RED,        //0
    GREEN,      //1
    BLUE,       //2
    YELLOW,     //3
    PURPLE,     //4
    MULCOLOR,   //5
    EXPLOSIVE,  //6
    BEAVER,     //7
    EMPTY       //8
}

public class BallCreator
{
    //Зміна кольору кулі
    public void ChengeColor(GameObject ball, TypesSphere typeBall)
    {
        ball.transform.GetComponent<Renderer>().material.color = GetColorByType(typeBall);
    }

    //Повертає кулю випадкового кольору з базових кольорів
    public Transform getBall(Transform ball,           //який об'єкт створювати
                              Vector3 placeToCreate)    //в якій позиції
    {
        return getBall(ball, placeToCreate, randomType(true, 4));
    }

    //Повертає шар певного кольору
    public Transform getBall(Transform ball,           //який об'єкт створювати
                      Vector3 placeToCreate,            //в якій позиції
                      TypesSphere typeBall)             //якого кольору(чи іншого типу для некольорового)
    {
        Transform result = Transform.Instantiate(ball, placeToCreate, Quaternion.identity);

        result.GetComponent<Renderer>().material.color = GetColorByType(typeBall);
        return result;
    }

    public TypesSphere randomType(bool isColorBall, int countColor)
    {
        int min = 0;
        int max = 0;
        if (isColorBall)
        {
            min = 0;
            max = countColor;
        }

        System.Random ran = new System.Random();
        return (TypesSphere)ran.Next(min, max);
    }

    private Vector4 GetColorByType(TypesSphere typeBall)
    {
        Vector4 color;
        switch (typeBall)
        {
            case TypesSphere.RED:
                {
                    color = Color.red;
                    break;
                }
            case TypesSphere.GREEN:
                {
                    color = Color.green;
                    break;
                }
            case TypesSphere.BLUE:
                {
                    color = Color.blue;
                    break;
                }
            case TypesSphere.YELLOW:
                {
                    color = Color.yellow;
                    break;
                }
            case TypesSphere.PURPLE:
                {
                    color = new Vector4(1, 0, 1, 1);
                    break;
                }
            case TypesSphere.EXPLOSIVE:
                {
                    Debug.Log("TypesSphere.EXPLOSIVE");
                    color = Color.black;
                    break;
                }
            case TypesSphere.BEAVER:
                {
                    color = Color.gray;
                    break;
                }
            case TypesSphere.EMPTY:
                {
                    color = new Vector4(1, 1, 1, 0);
                    break;
                }
            default:
                {
                    color = Color.white; // за замовчуванням білий, непрозорий
                    break;
                }
        }
        return color;
    }
}

static public class BallController
{
    static private List<GameObject>[] ballsLists = new List<GameObject>[10];      //масив з списками потоків куль всіх створених респавнів(до десяти)
    static public int lastForwardBallIndex;
    static public bool readyToDestroy;           //використовується для визначення моменту, в який можна звертатись до списку, для недопущення знищення куль, до яких ще буде потрібен доступ
    static public bool redyToRunNewPlayerBall = true;   //використовується для визначення, чи можна запускати нову кулю.
    static BallController()
    {
        for (int i = 0; i < ballsLists.Length; i++)
        {
            ballsLists[i] = new List<GameObject>();                                 //ініціалізація списків в конструкторі
        }
    }
    static public List<GameObject>[] BallsLists
    {
        get
        {
            return ballsLists;
        }
    }

    static public void AddBallsList(List<GameObject> balls, int index)                                //створити новий список з потоком куль
    {
        ballsLists[index] = balls;
    }

    static public List<GameObject> GetBalls(int ballsListIndex)
    {
        return ballsLists[ballsListIndex];
    }

    //використовувалась для отримання передніх куль в списку, на данний момент не використовую
    static public List<GameObject> GetForwardBalls(int ballsListIndex, GameObject target)
    {
        int thisBallIndex = GetBalls(ballsListIndex).IndexOf(target);

        if (thisBallIndex <= 0)
            return null;

        CleanBallList(ballsListIndex);

        return GetBalls(ballsListIndex).GetRange(0, thisBallIndex);
    }

    static void CleanBallList(int ballsListIndex)            //костиль. В одному кадрі видаляється куля та відбувається звертання до списку, до її видалення з нього? Це додаткова перевірка (по нормальному зайва)
    {
        for (int i = 0; i < ballsLists[ballsListIndex].Count; i++)
            if (ballsLists[ballsListIndex][i] == null)
            {
                Debug.LogError("Куля відсутня. індекс=" + i);
                ballsLists[ballsListIndex].RemoveAt(i);
            }
    }

    //Використовувалась для вбудовування кулі в середину списку. На данний момент не використовую
    static public void InsertBallToList(List<GameObject> forward, GameObject ball, int respIndex)
    {
        List<GameObject> bufer = new List<GameObject>();

        //Debug.Log("ballsLists[respIndex].Count="+ ballsLists[respIndex].Count);
        if (forward != null)
            bufer = ballsLists[respIndex].GetRange(forward.Count, ballsLists[respIndex].Count - forward.Count);
        else
        {
            forward = new List<GameObject>();
            bufer = ballsLists[respIndex];
        }

        if (ball == null)
        {
            Debug.Log("ball == null");
        }
        if (forward == null)
        {
            Debug.Log("forward == null");
        }
        forward.Add(ball);
        Debug.Log("ballsLists[respIndex]_1=" + ballsLists[respIndex].Count);
        ballsLists[respIndex].Clear();
        ballsLists[respIndex].AddRange(forward);
        ballsLists[respIndex].AddRange(bufer);
        Debug.Log("ballsLists[respIndex]_2=" + ballsLists[respIndex].Count);
    }

    static public void DelBallFromList(GameObject ball, int respIndex)
    {
        ballsLists[respIndex].Remove(ball);
    }
}