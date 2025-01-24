using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Add this for UI elements

public class PlayerCam : MonoBehaviour
{
    public float sensitivity = 100f; // Single sensitivity value
    public Transform orientation;

    private float xRotation;
    private float yRotation;

    private float recoilVertical = 0f; // Tracks current vertical recoil
    private float recoilHorizontal = 0f; // Tracks current horizontal recoil
    private float recoilRecoverySpeed = 5f; // Speed of recoil recovery (adjust as needed)

    public Slider sensitivitySlider; // Reference to the sensitivity slider UI

    private void Start()
    {
        // Lock the cursor at the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set the slider's initial value to match the current sensitivity
        sensitivitySlider.value = sensitivity;

        // Add listener to change sensitivity when slider value changes
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

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

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity; // Update sensitivity value from slider
    }
}
