using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RectTransform textRectTransform;
    public Text HighscoreText;
    public Text[] ButtonTexts;

    private float currentLeftValue;
    private float resetValue;
    private float startValue;

    void Start()
    {
        UpdateText();

        startValue = textRectTransform.offsetMin.x;
        currentLeftValue = startValue;
        resetValue = - textRectTransform.rect.width - 1000;
    }

    void Update()
    {
        currentLeftValue -= 0.5f;
        textRectTransform.anchoredPosition = new Vector2(currentLeftValue, textRectTransform.anchoredPosition.y);

        if (currentLeftValue < resetValue) currentLeftValue = startValue;
    }

    private void UpdateText()
    {
        int[] highscores = {
            PlayerPrefs.GetInt("Highscore0"),
            PlayerPrefs.GetInt("Highscore1"),
            PlayerPrefs.GetInt("Highscore2"),
            PlayerPrefs.GetInt("Highscore3")
        };

        string text = "Highscore:\t\t";

        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    text += "Easy-";
                    break;
                case 1:
                    text += "Medium-";
                    break;
                case 2:
                    text += "Hard-";
                    break;
                case 3:
                    text += "Extreme-";
                    break;
            }

            if (highscores[i] == 0) text += "??? (--:-- --.--.--)\t\t";
            else text += highscores[i] + " (" + PlayerPrefs.GetString("HighscoreTimestamp" + i) + ")\t\t";
        }

        HighscoreText.text = text;
    }

    public void PointerEnterButton(int modeNr)
    {
        ButtonTexts[modeNr].color = new Color(200f/255, 107f/255, 27f/255);
    }

    public void PointerOutButton(int modeNr)
    {
        ButtonTexts[modeNr].color = new Color(1f, 1f, 1f);
    }
}
