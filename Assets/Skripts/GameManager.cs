using System;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;


public class Gun
{
    public string Name;
    public int AmmoCapacity;
    public int LoadingTimeFullAmmo;
    public int BulletLeft;

    public Gun(string Name, int AmmoCapacity, int LoadingTimeFullAmmo)
    {
        this.Name = Name;
        this.AmmoCapacity = AmmoCapacity;
        BulletLeft = AmmoCapacity;
        this.LoadingTimeFullAmmo = LoadingTimeFullAmmo; //milisecond
    }

    public bool Fire()
    {
        if (BulletLeft > 0)
        {
            BulletLeft--;
            Debug.Log("Fired. BulletLeft: " + BulletLeft);
        }

        return BulletLeft == 0;
    }

    public void Reload()
    {
        BulletLeft = AmmoCapacity;
        Debug.Log("Reloaded. BulletLeft: " + BulletLeft);
    }

    public bool IsAmmoFull()
    {
        return BulletLeft == AmmoCapacity;
    }

    public int CalcLoadingTime()
    {
        if (BulletLeft == 0) return LoadingTimeFullAmmo;
        else return (int) ((float) (AmmoCapacity - BulletLeft) / AmmoCapacity * LoadingTimeFullAmmo);
    }
}

public class GameManager : MonoBehaviour
{
    /*
     *  Information from Weapon from Vlad
     */

    public static GameManager Instance;

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

    private Gun[] Guns = new Gun[] {
        new Gun("Pistol", 6, 2500),
        new Gun("Rifle", 30, 5000),
        new Gun("Shotgun", 1, 1000)
    };

    private bool IsReloading = false;
    private int BulletLeft;
    private Gun BeingUsedGun;
    private int CurrentGun; //Pistol = 0, Rifle = 1, Shotgun = 2
    public bool TimerOn { get; private set; } // TimerOn is public, but only modifiable within this class
    private int Score;

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

    void Start()
    {
        Debug.Log(ModeManager.Instance.ModeNr);
        TimerOn = true;
        TimeLeft = MaxTime;

        BeingUsedGun = Guns[0];
        BulletLeft = BeingUsedGun.AmmoCapacity;
        BulletLeftTexts[0].text = "6 / 6";
        GunBackgrounds[0].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(200, 150);
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

    // Reload logic
    if (!IsReloading)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _ = ReloadAsync();
        }

        // Gun switching logic
        if (Input.GetKeyDown(KeyCode.Alpha1) && !Object.ReferenceEquals(BeingUsedGun, Guns[0]))
        {
            BeingUsedGun = Guns[0];
            UpdateUIAfterChangingGun(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && !Object.ReferenceEquals(BeingUsedGun, Guns[1]))
        {
            BeingUsedGun = Guns[1];
            UpdateUIAfterChangingGun(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && !Object.ReferenceEquals(BeingUsedGun, Guns[2]))
        {
            BeingUsedGun = Guns[2];
            UpdateUIAfterChangingGun(2);
        }

        // Firing logic
        if (Input.GetMouseButtonDown(0)) // Left mouse button for shooting
        {
            Fire();
        }
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

    private void UpdateUIAfterChangingGun(int NextGun)
    {
        BulletLeftTexts[CurrentGun].text = "";
        BulletLeftTexts[NextGun].text = BeingUsedGun.BulletLeft + " / " + BeingUsedGun.AmmoCapacity;

        GunBackgrounds[CurrentGun].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(200, 80);
        GunBackgrounds[NextGun].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(200, 150);

        CurrentGun = NextGun;
    }

    public void Fire()
    {
        if (IsReloading) return; // Prevent firing while reloading

        if (BeingUsedGun.Fire())
        {
            // If the gun is empty after firing, start reloading
            BulletLeftTexts[CurrentGun].text = "Reloading...";
            _ = ReloadAsync();
        }
        else
        {
            // Update the UI with the remaining ammo
            BulletLeftTexts[CurrentGun].text = BeingUsedGun.BulletLeft + " / " + BeingUsedGun.AmmoCapacity;
        }
    }


    private async Task ReloadAsync()
    {
        if (BeingUsedGun.IsAmmoFull()) return; // No need to reload if ammo is already full

        BulletLeftTexts[CurrentGun].text = "Reloading...";
        IsReloading = true;

        await Task.Delay(BeingUsedGun.CalcLoadingTime()); // Simulate reload time

        BeingUsedGun.Reload(); // Reload the gun
        BulletLeftTexts[CurrentGun].text = BeingUsedGun.AmmoCapacity + " / " + BeingUsedGun.AmmoCapacity;
        IsReloading = false;
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


       int storedHighscore = PlayerPrefs.GetInt(ModeManager.Instance.HighscoreKey);

        if (Score > storedHighscore)
        {
            PlayerPrefs.SetInt(ModeManager.Instance.HighscoreKey, Score);
            PlayerPrefs.SetString(ModeManager.Instance.HighscoreTimestampKey, DateTime.Now.ToString("HH:mm dd.MM.yyyy"));
            FinishCanvasHighscoreText.text = "Congratulation! New Highscore!";
        }
        else FinishCanvasHighscoreText.text = "Highscore: " + storedHighscore;
    }

    public void BackHome()
    {
        SceneManager.LoadScene("Home");
    }
}
