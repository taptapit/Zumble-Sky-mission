using UnityEngine;

public class BallExplosive : SphereBehaviour
{
    //Підписка для визначення кількості вибитих балів
    public delegate void ScoreHandler(int score);
    public event ScoreHandler scoreMessage;            

    public int rangExplosiveSkill;                 // призначити з геймконтрола (що б лишній раз не читати або передавати збереження гри)
    int score;

    //Ефекти
    public GameObject explosivePs;
    public AudioSource explosiveAudio;


    void OnTriggerEnter2D(Collider2D collBall)
    {
        if (collBall.gameObject.tag != "ball" && collBall.gameObject.tag != "newBall")      //перервати, якщо колізія не з кулею
            return;

        BallController.RedyToRunNewPlayerBall = true;

        //        if (rangExplosiveSkill==0)
        DestroyBall(collBall.gameObject.GetComponent<BallBehaviour>().FrontBall, collBall.gameObject.GetComponent<BallBehaviour>().BackBall, rangExplosiveSkill+1);
        BallController.BallsLists[5].Add(collBall.gameObject);
        //Destroy(collBall.gameObject);
        score += 2;

        //Звук
        //explosiveAudio?.Play();

        //вибух
        if (explosivePs != null)
        {
            Vector3 particlePos = new Vector3(transform.position.x, transform.position.y, -2);
            GameObject explodeParticleSystem = Transform.Instantiate(explosivePs, particlePos, Quaternion.identity);
            Destroy(explodeParticleSystem.gameObject, 0.5f);
        }

        Destroy(gameObject);
    }

    //Рекурсивне знищення певної кількості передніх/задніх куль
    private void DestroyBall(GameObject frontBall, GameObject backBall, int count)
    {
        GameObject nextFrontBall = null;
        GameObject nextBackBall = null;

        if (frontBall!=null)
        {
            nextFrontBall = frontBall.GetComponent<BallBehaviour>().FrontBall;
            score+=2;
            BallController.BallsLists[5].Add(frontBall);
            //Destroy(frontBall);
        }

        if (backBall!=null)
        {
            nextBackBall = backBall.GetComponent<BallBehaviour>().BackBall;
            score+=2;
            BallController.BallsLists[5].Add(backBall);
            //Destroy(backBall);
        }

        count--;
        if (count < 1)
        {
            scoreMessage?.Invoke(score);
            return;
        }

        DestroyBall(nextFrontBall, nextBackBall, count);
    }
}