using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZLibrary
{
    //ПОВЕДІНКА ОБ'ЄКТА ЩО РУХАЄТЬСЯ
    //Кожному об'єкту може бути заданий рух. Рух задається за переданою множиною точок, або в напрямку
    //Швідкість може бути задана окремо
    //Об'єкт може бути зупинений
    //Має текстову властивість "тип сфери", що зберігає задані кольори, так і вказує на інші варіанти подібних об'єктів, наприклад "мультиколірна сфера", "сфера вибухівка", "ракета")
    //Куля має дві сусідні кулі - "передню" та "задню". Має властивості для цього значення (сусідів може не бути).
    public interface IMovingObject
    {
        public Vector3 DestVector();                              //поточна точка призначення
        public void Move(Transform direction);                      //рух по напрямку
        public void Move(List<Transform> path, bool isRevers);      //рух по списку точок
        public void Stop();                                         //стоп
        public GameObject FrontBall { get; set; }                   //куля попереду
        public GameObject BackBall { get; set; }                    //куля позаду
        public float Speed { get; set; }                            //швидкість
        public short TypeSphere { get; set; }                       //тип
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
        //Зміна кольору кулі
        public void ChengeColor(GameObject ball, TypesSphere typeBall)
        {
            ball.transform.GetComponent<Renderer>().material.color = GetColorByType(typeBall);
        }

        //Повертає кулю випадкового кольору з базових кольорів
        public Transform getBall(Transform ball,           //який об'єкт створювати
                                  Vector3 placeToCreate)    //в якій позиції
        {
            return getBall(ball, placeToCreate, randomType(true));
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
        static private List<List<GameObject>> ballsLists = new List<List<GameObject>>();      //списки з потоками куль всіх створених респавнів

        static public void AddBallsList(List<GameObject> balls)                                //створити новий список з потоком куль
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

        static public List<GameObject> GetBalls(int ballsListIndex)
        {
            return ballsLists[ballsListIndex];
        }

        static public List<GameObject> GetForwardBalls(int ballsListIndex, GameObject target)
        {
            int thisBallIndex = GetBalls(ballsListIndex).IndexOf(target);
            //Debug.Log($"ballIndex={thisBallIndex}");

            if (thisBallIndex <= 0)
            {
                return null;
            }
            

            return GetBalls(ballsListIndex).GetRange(0, thisBallIndex);
        }

        /*  static public List<GameObject> GetTowarddBalls(int ballsListIndex, GameObject target)
          {
              int thisBallIndex = ballsLists[ballsListIndex].IndexOf(target);

              if (thisBallIndex == ballsLists[ballsListIndex].Count-1)
              {
                  return null;
              }

              return ballsLists[ballsListIndex].GetRange(ballsLists[ballsListIndex], ballsLists[ballsListIndex].Count-1);
          }*/

        //логіка зіткнення кулі гравця з потоком куль
        /*  static public void CheckCollision(GameObject atacker, GameObject target, int ballsListIndex)
          {
              if (ballsLists == null)
                  return;

              //сфери, попереду поточної
              List<GameObject> forwardBalls = ballsLists[ballsListIndex].GetRange(ballsLists[ballsListIndex].IndexOf(target), ballsLists[ballsListIndex].Count-1);
              foreach (GameObject ball in forwardBalls)
              {

              }
          }*/
    }
}