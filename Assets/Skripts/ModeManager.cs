using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class ModeManager : MonoBehaviour
{
    public static ModeManager Instance;

    public int ModeNr; //Easy = 0, Medium = 1, Hard = 2, Extreme = 3
    public string HighscoreKey;
    public string HighscoreTimestampKey;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetModeNr(int modeNr)
    {
        ModeNr = modeNr;

        switch (modeNr)
        {
            case 0:
                HighscoreKey = "Highscore0";
                HighscoreTimestampKey = "HighscoreTimestamp0";
                break;
            case 1:
                HighscoreKey = "Highscore1";
                HighscoreTimestampKey = "HighscoreTimestamp1";
                break;
            case 2:
                HighscoreKey = "Highscore2";
                HighscoreTimestampKey = "HighscoreTimestamp2";
                break;
            case 3:
                HighscoreKey = "Highscore3";
                HighscoreTimestampKey = "HighscoreTimestamp3";
                break;
        }

        SceneManager.LoadScene("InGame");
    }
}
