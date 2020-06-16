using UnityEngine;

public class ParticleSystemSalut : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.ForceOverLifetimeModule fo;
    public float speed;
    private float forceY;



    // Start is called before the first frame update
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = new Color(Random.Range(0.3f, 1.0f), Random.Range(0.3f, 1.0f), Random.Range(0.3f, 1.0f), Random.Range(0.3f, 1.0f));

        fo = ps.forceOverLifetime;
        fo.enabled = true;

      //  Destroy(gameObject, 1.0f);
        //speed = 0.1f;

       // AnimationCurve curve = new AnimationCurve();
       // curve.AddKey(0.0f, 0.1f);
       // curve.AddKey(0.75f, 1.0f);
       //  fo.y = 5.0f; //= new ParticleSystem.MinMaxCurve(1.5f, curve);
    }

    // Update is called once per frame
    void Update()
    {
        fo.y = forceY;
        forceY += speed;
    }
}
