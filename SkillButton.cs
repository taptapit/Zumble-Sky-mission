using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SkillButton : MonoBehaviour
{
    SaveLoadGame saveLoadManager;
    SaveInfo save;

    //Поля, які налаштовуються в редакторі, відповідно до графічної і логічної побудови дерева вмінь
    public SpriteRenderer[] linkSprite;          //лінії, що з'єднують навик з попередніми по дереву та інші додаткові графічні елементи
   
    public PlayerSkill[] requirementsSkill;    //навики, що необхідні для відкриття даного
    public int[] requirementsSkillValue;       //значення навика, що необхідне для відкриття даного. Кількість значень повинна відповідати кількості навиків
    public PlayerSkill thisSkill;              //поточний навик
    public int thisSkillValue;                 //значення поточного навика (для багаторівневих однакових навиків)
    Button button;                             //власне кнопка
    public SkillScreenManager skillScreenManager;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();

        UpdateSave();        //Кнопка скіла працює з ігровим збереженням, але сама нічого не зберігає в файл. Зберігає SkillScreenManager

        if (thisSkillValue == 0)
            thisSkillValue = 1;

        SkillButtonUpdate();
    }

    public void SkillButtonUpdate()
    {
        UpdateSave();
        //Debug.Log("-=1=-" + thisSkill);

        if (button == null)
            return;

        //Якщо навик не вивчений, перевірити чи можна його відкрити
        bool isEnable = true;

        //Якщо є одна невідповідність, то навик зачинений
        if (requirementsSkill != null)
            for (int i = 0; i < requirementsSkill.Length; i++)
                if (save.skill[(int)requirementsSkill[i]] < requirementsSkillValue[i])
                {
                    //Debug.Log(thisSkill + " " + requirementsSkill[i] + "=" + save.skill[(int)requirementsSkill[i]]);
                    isEnable = false;
                }

        // Перевірка, чи даний навик вже вивчений
        if (save.skill[(int)thisSkill] >= thisSkillValue && isEnable)
        {
            button.interactable = false;
            button.colors = colorBlock(1);

            //Якщо вивчений то зробити кнопку не активною, підсвітити зеленим, підсвітити лінію з'єднання
            if (linkSprite != null)
                foreach (var item in linkSprite)
                    item.color = Color.green;


          //  Debug.Log("-=2=-" + thisSkill +" "+ save.skill[(int)thisSkill] +">="+ thisSkillValue);

            return;
        }
        
       // Debug.Log("-=3=-" + thisSkill);

        //Ввімкнути кнопку
        if (isEnable)
        {
            //Debug.Log("-=4=-" + thisSkill);
            button.interactable = true;
            button.colors = colorBlock(0);
            if (linkSprite != null)
                foreach (var item in linkSprite)
                    item.color = Color.white;

            return;
        }

        // Наступні речі можна повиставляти для кнопок в редакторі, але так не потрібно індивідуально вникати в налаштування за замовчуванням
        button.interactable = false;
        button.colors = colorBlock(-1);
        if (linkSprite != null)
            foreach (var item in linkSprite)
                item.color = Color.gray;
    }

    public void OnButtonDown()
    {
        if (save.skillPoint < 1)
            return;

        save.skillPoint--;

        //Повторна перевірка на відповідність вимогам
        //Якщо є одна невідповідність, то навик зачинений
        if (requirementsSkill != null)
            for (int i = 0; i < requirementsSkill.Length; i++)
                if (save.skill[(int)requirementsSkill[i]] < requirementsSkillValue[i])
                    return;

        // Вивчити навик
        save.skill[(int)thisSkill] +=1;
        //Debug.Log("OnButtonDown "+thisSkill + "="+ save.skill[(int)thisSkill]);

        button.interactable = false;

        button.colors = colorBlock(1);

        //Якщо вивчений то зробити кнопку не активною, підсвітити зеленим, підсвітити лінію з'єднання
        if (linkSprite != null)
            foreach (var item in linkSprite)
                item.color = Color.green;
        // Після даного метода повинно викликатись оновлення всіх кнопок (роблю це в SkillScreenManager). Альтернатива - розмістити це оновлення в Update
    }

    private void UpdateSave()
    {
        save = skillScreenManager.save;           //Кнопка скіла працює з ігровим збереженням, але сама нічого не зберігає в файл. Зберігає SkillScreenManager
        if (save == null)
        {
            saveLoadManager = new SaveLoadGame();
            save = saveLoadManager.LoadSave();
            //Debug.LogError("Не завантажене збереження з skillScreenManager");
        }
    }

    //status "-1" - не активна "0" - активна "1" - позначена зеленим 
    ColorBlock colorBlock(int status)
    {
        ColorBlock cb = new ColorBlock();
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.white;
        cb.pressedColor = Color.gray;
        cb.selectedColor = Color.white;
        cb.colorMultiplier = 1;

        if (status == -1)
            cb.disabledColor = Color.gray;
        else if (status == 1)
            cb.disabledColor = Color.green;
        else
            cb.disabledColor = Color.white;

        return cb;
    }
}