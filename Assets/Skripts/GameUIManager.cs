using System;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;


public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    public Text[] BulletLeftTexts; //Pistol, Rifle, Shotgun
    public GameObject[] GunBackgrounds; //Pistol, Rifle, Shotgun

    public float MaxTime;
    public Text TimerText;

    public GameObject InGameCanvas;
    public GameObject PauseCanvas;
    public GameObject FinishCanvas;

    public Text InGameScoreText;
    public Text PauseScoreText;
    public Text FinishScoreText;
    public Text FinishCanvasHighscoreText;

    private float TimeLeft;

    private int CurrentGun = 0; //Pistol = 0, Rifle = 1, Shotgun = 2
    public bool TimerOn { get; private set; } // TimerOn is public, but only modifiable within this class
    private int Score;

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

    void Start()
    {
        Debug.Log(ModeManager.Instance.ModeNr);
        TimerOn = true;
        TimeLeft = MaxTime;
    }

    void Update()
    {
        if (!TimerOn) return;

        // Timer logic
        if (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            UpdateTimer(TimeLeft);
        }
        else
        {
            FinishGame();
        }

        // Pause logic
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateUIAfterChangingGun(int NextGun, Weapon wp)
    {
        BulletLeftTexts[CurrentGun].text = "";
        BulletLeftTexts[NextGun-1].text = wp.GetBulletLeftForUI() + " / " + wp.GetAmmoCapacityForUI();

        GunBackgrounds[CurrentGun].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(200, 80);
        GunBackgrounds[NextGun-1].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(200, 150);

        CurrentGun = NextGun-1;
    }

    public void UpdateUIOfBulletStatusForCurrentWp(Weapon wp)
    {
        BulletLeftTexts[CurrentGun].text = wp.GetBulletLeftForUI() + " / " + wp.GetAmmoCapacityForUI();
    }

    public void UpdateUIForReloading()
    {
        BulletLeftTexts[CurrentGun].text = "Reloading...";
    }


    public void GainScore(int Addition)
    { 
        Score += Addition;
        InGameScoreText.text = Score.ToString();
    }

    public void TogglePause()
    {
        if (!TimerOn)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        PauseScoreText.text = Score.ToString();
        InGameCanvas.SetActive(false);
        PauseCanvas.SetActive(true);
        Time.timeScale = 0f;
        TimerOn = false;
    }

    public void ResumeGame()
    {
        InGameCanvas.SetActive(true);
        PauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        TimerOn = true;
    }

    public void FinishGame()
    {
        TimeLeft = MaxTime;

        FinishScoreText.text = Score.ToString();
        InGameCanvas.SetActive(false);
        FinishCanvas.SetActive(true);
        Time.timeScale = 0f;
        TimerOn = false;


       int storedHighscore = PlayerPrefs.GetInt(ModeManager.Instance.HighscoreKey, 0);

        if (Score > storedHighscore)
        {
            PlayerPrefs.SetInt(ModeManager.Instance.HighscoreKey, Score);
            PlayerPrefs.SetString(ModeManager.Instance.HighscoreTimestampKey, DateTime.Now.ToString("HH:mm dd.MM.yyyy"));

            PlayerPrefs.Save();

            FinishCanvasHighscoreText.text = "Congratulation! New Highscore!";
        }
        else FinishCanvasHighscoreText.text = "Highscore: " + storedHighscore;
    }

    public void BackHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
