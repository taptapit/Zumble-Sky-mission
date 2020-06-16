using UnityEngine;

public class Salut : MonoBehaviour
{
    public GameObject pss;                    //з редактора
    GameObject newSalut;

    public float xRange;
    public float yRange;

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0.0f, 1.0f) > 0.7f)
        {
            Vector3 particlePos = new Vector3(Random.Range(-xRange, xRange), Random.Range(-yRange, yRange), 0);
            newSalut = GameObject.Instantiate(pss, particlePos, Quaternion.identity);
            Destroy(newSalut, 3.0f);
            newSalut = null;
        }
    }
}