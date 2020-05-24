using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    void Start()
    {
        //Стартовий колір
        float randomColor = Random.Range(0, 1.0f);
        if (randomColor < 0.75f)
            return;

        SpriteRenderer backgroundSpriteRender = gameObject.GetComponent<SpriteRenderer>();

        if (randomColor < 0.8f)
            backgroundSpriteRender.color = new Color(0.48f, 0.13f, 0.64f);
        else if (randomColor < 0.85f)
            backgroundSpriteRender.color = new Color(0.396f, 0.545f, 0.878f);
        else if (randomColor < 0.9f)
            backgroundSpriteRender.color = new Color(0.65f, 0, 0.729f);
        else if (randomColor < 0.95f)
            backgroundSpriteRender.color = new Color(0.729f, 0.4f, 0);
        else
            backgroundSpriteRender.color = new Color(0, 0.537f, 0.729f);
    }
}
