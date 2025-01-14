using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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
    public Text[] BulletLeftTexts; //Pistol, Shotgun, Rifle
    public GameObject[] GunBackgrounds; //Pistol, Shotgun, Rifle
    public Text ScoreNumber;

    private Gun[] Guns = new Gun[] {
        new Gun("Pistol", 6, 2500),
        new Gun("Shotgun", 1, 1000),
        new Gun("Rifle", 30, 5000)
    };

    private bool IsReloading = false;
    private int BulletLeft;
    private Gun BeingUsedGun;
    private int CurrentGun; //Pistol = 0, Shotgun = 1, Rifle = 2

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
        if (!IsReloading)
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
            if (Input.GetMouseButtonDown(0))  // left mouse
            {
                Fire();
            }
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
        ScoreNumber.text = Score.ToString();
    }
}
