using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public Text[] BulletLeftTexts; //Pistol, Rifle, Shotgun
    public GameObject[] GunBackgrounds; //Pistol, Rifle, Shotgun
    public Text InGameScoreText;

    public GameObject InGameCanvas;
    public GameObject PauseCanvas;
    public Text PauseScoreText;

    private Gun[] Guns = new Gun[] {
        new Gun("Pistol", 6, 2500),
        new Gun("Rifle", 30, 5000),
        new Gun("Shotgun", 1, 1000)
    };

    private bool IsReloading = false;
    private int BulletLeft;
    private Gun BeingUsedGun;
    private int CurrentGun; //Pistol = 0, Rifle = 1, Shotgun = 2

    private bool IsPaused = false;
    private int Score;


    void Start()
    {
        Debug.Log(ModeManager.Instance.Difficulty);

        BeingUsedGun = Guns[0];
        BulletLeft = BeingUsedGun.AmmoCapacity;
        BulletLeftTexts[0].text = "6 / 6";
        GunBackgrounds[0].GetComponent<Image>().rectTransform.sizeDelta = new Vector2(200, 150);
    }

    void Update()
    {
        if (!IsReloading && !IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _ = ReloadAsync();
            }
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
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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
        if (BeingUsedGun.Fire()) _ = ReloadAsync();
        else BulletLeftTexts[CurrentGun].text = BeingUsedGun.BulletLeft + " / " + BeingUsedGun.AmmoCapacity;
    }

    private async Task ReloadAsync()
    {
        if (BeingUsedGun.IsAmmoFull()) return;

        BulletLeftTexts[CurrentGun].text = "Reloading...";
        IsReloading = true;

        await Task.Delay(BeingUsedGun.CalcLoadingTime());
        BeingUsedGun.Reload();
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
        if (IsPaused)
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
        IsPaused = true;
    }

    public void ResumeGame()
    {
        InGameCanvas.SetActive(true);
        PauseCanvas.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void BackHome()
    {
        SceneManager.LoadScene("Home");
    }
}
