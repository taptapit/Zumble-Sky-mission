using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : MonoBehaviour
{
    public SpriteRenderer moonSpriteRender;
    SaveInfo save;
    SaveLoadGame saveLoadManager;
    // Start is called before the first frame update
    void Start()
    {
        saveLoadManager = new SaveLoadGame();
        save = saveLoadManager.LoadSave();
        float scaleValue = 1.0f;

        if (save != null)
            scaleValue = Mathf.Clamp((save.maxOpenLvl / 10.0f), 1, 8);

        moonSpriteRender.transform.localScale = new Vector3(scaleValue, scaleValue, 1);
    }
}
