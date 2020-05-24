using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float fullValue;
    public float currentValue;
    public bool alwaysUpdate;

    private float lengthX;
    private float startX;
    SpriteRenderer barrSpriteRender;

    // Start is called before the first frame update
    void Start()
    {
        barrSpriteRender = GetComponent<SpriteRenderer>();
        
        lengthX = barrSpriteRender.bounds.size.x;
        startX = transform.position.x;

        UpdateProgressBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (!alwaysUpdate)
            return;

        UpdateProgressBar();
    }

    public void UpdateProgressBar()
    {
        transform.localScale = new Vector3(currentValue / fullValue, 1, 1);
        float offsetX = (lengthX - barrSpriteRender.bounds.size.x) / 2;
        transform.position = new Vector3(startX - offsetX, transform.position.y, transform.position.z);
    }
}