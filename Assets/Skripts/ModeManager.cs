using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeManager : MonoBehaviour
{
    public static ModeManager Instance;

    public int Difficulty;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(int difficulty)
    {
        Difficulty = difficulty;
        SceneManager.LoadScene("InGame");
    }
}
