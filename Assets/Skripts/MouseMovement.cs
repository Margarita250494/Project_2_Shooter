using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 500f;

    float xRotation = 0f;
    float yRotation = 0f;

    [SerializeField]float topClamp = -90f;
    [SerializeField]float bottomClamp = 90f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation,topClamp,bottomClamp);

        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation,yRotation,0f);
    }
}
