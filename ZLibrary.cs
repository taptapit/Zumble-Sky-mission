using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZLibrary
{
    //ПОВЕДІНКА ОБ'ЄКТА ЩО РУХАЄТЬСЯ
    //Кожному об'єкту може бути заданий рух. Рух задається за переданою множиною точок, або в напрямку
    //Рух може бути заданий реверсно, з останньої точки переданого масиву до першої.
    //Швідкість може бути задана окремо
    //Об'єкт може бути зупинений
    //Має текстову властивість "тип сфери", що зберігає задані кольори, так і вказує на інші варіанти подібних об'єктів, наприклад "мультиколірна сфера", "сфера вибухівка", "ракета")
    //Об'єкт має ID
    public interface IMovingObject
    {
        public void Move(Transform direction);                      //рух по напрямку
        public void Move(List<Transform> path, bool isRevers);     //рух по списку точок
        public void Stop();                                       //стоп
        public float Speed { get; set; }                         //швидкість
        public short TypeSphere { get; set; }                    //тип
        public int ID { get; set; }                              //ID
    }

    //ПОВЕДІНКА СФЕР
    //Сфера - об'єкт, якому може бути заданий рух
    //Сфера має дві сусідні сфери "передню" та "задню". Має властивості для цього значення (сусідів може не бути).
    public interface IBall : IMovingObject
    {
        public GameObject FrontBall { get; set; }
        public GameObject BackBall { get; set; }
        public int FrontBallID { get; set; }
        public int BackBallID { get; set; }
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
        ROCKET,     //7
        EMPTY       //8
    }

    public class BallCreator
    {
        //
        //Повертає шар випадкового кольору з базових кольорів
        public Transform getBall(Transform ball,           //який об'єкт створювати
                                  Vector3 placeToCreate)    //в якій позиції
        {
            return getBall(ball, placeToCreate, randomType(true));
        }
        
        //Повертає шар певного кольору
        public Transform getBall(Transform ball,           //який об'єкт створювати
                          Vector3 placeToCreate,            //в якій позиції
                          TypesSphere typeBall)              //якого кольору(чи іншого типу для некольорового)
        {
            Vector4 color;
            Transform result = Transform.Instantiate(ball, placeToCreate, Quaternion.identity);
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
                case TypesSphere.EMPTY:
                    {
                        color = new Vector4(1,1,1,0);
                        break;
                    }
                default:
                    {
                        color = Color.white; // за замовчуванням білий, непрозорий
                        break;
                    }
            }

            result.GetComponent<Renderer>().material.color = color;
            return result;
        }

        TypesSphere randomType(bool isColorBall)
        {
            int min = 0;
            int max = 0;
            if (isColorBall)
            {
                min = 0;
                max = 5;
            }

            System.Random ran = new System.Random();
            return (TypesSphere)ran.Next(min, max);
        }

    }

    static public class BallController
    {
        static private List<List<GameObject>> ballsLists = new List<List<GameObject>>();      //списки з шарами всіх створених респавнів
        static private List<GameObject> test;

        static public void setBallList(List<GameObject> balls)
        {
            ballsLists.Add(balls);
        }

        static public int BallsListIndex
        {
            get
            {
                if (ballsLists == null)
                    return 0;
                else
                    return ballsLists.Count;
            }
        }
    }
}