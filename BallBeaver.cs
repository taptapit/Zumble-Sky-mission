using UnityEngine;

public class BallBeaver : SphereBehaviour
{
    //Підписка для визначення кількості вибитих балів
    public delegate void ScoreHandler(int score);
    public event ScoreHandler scoreMessage;

    int score;
    int countToDestroy;                 
    bool isFirstColl = true;

    public int PowerBeaverSkill             //Призначити з геймконтрола (що б лишній раз не читати або передавати збереження гри).         
    {
        set
        {
            if (value==0)
            {
                countToDestroy = 4;
                return;
            }
            countToDestroy = 12;            
        }
    }

    private void OnTriggerEnter2D (Collider2D collBall)
    {
        if (collBall.gameObject.tag != "ball" && collBall.gameObject.tag != "newBall")      //перервати, якщо колізія не з кулею
            return;
        
        if (countToDestroy<1)
        {
            scoreMessage?.Invoke(score);
            BallController.redyToRunNewPlayerBall = true;
            //BallController.BallsLists[5].Add(gameObject);
            Destroy(gameObject);
            return;
        }

        if (isFirstColl)
        {
            SphereBehaviour collBallBehaviour = collBall.gameObject.GetComponent<SphereBehaviour>();

            pathPoints = collBallBehaviour.pathPoints;
            destPointIndex = collBallBehaviour.destPointIndex;
            Speed = 20.0f;
            Move(pathPoints, 1);
            isFirstColl = false;
            Debug.Log("isFirstColl");
        }

        Debug.Log("countToDestroy="+ countToDestroy);
        countToDestroy--;
        BallController.BallsLists[5].Add(collBall.gameObject);
        //Destroy(collBall.gameObject);
        score += 2;
    }

    public override void Muving()
    {
        if (pathPoints.Count > 1)
            if (destPointIndex >= pathPoints.Count - 1)
            {
                BallController.redyToRunNewPlayerBall = true;
                Destroy(gameObject);
            }

        base.Muving();  
    }
}