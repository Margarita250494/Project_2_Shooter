using System.Collections;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private float recoilVertical = 0f; // Tracks current vertical recoil
    private float recoilHorizontal = 0f; // Tracks current horizontal recoil
    private float recoilRecoverySpeed = 5f; // Speed of recoil recovery (adjust as needed)

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX + recoilHorizontal; // Include horizontal recoil
        xRotation -= mouseY + recoilVertical; // Include vertical recoil

        // Clamp vertical rotation to avoid unnatural behavior
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera and the player's orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        // Gradually recover recoil to 0
        recoilVertical = Mathf.Lerp(recoilVertical, 0, Time.deltaTime * recoilRecoverySpeed);
        recoilHorizontal = Mathf.Lerp(recoilHorizontal, 0, Time.deltaTime * recoilRecoverySpeed);
    }

    public void ApplyRecoil(float verticalRecoil, float horizontalRecoil)
    {
        // Add recoil values to the current recoil variables
        recoilVertical += verticalRecoil;
        recoilHorizontal += horizontalRecoil;
    }
}
