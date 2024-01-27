using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour
{
    public float mouseSensitivity = 500;
    float Xrot = 0f;
    float Yrot = 0f;
    Vector2 mousePos;
    Vector2 mousePosStart = new Vector2(0, 0);
    Camera camera;
    Controls playerInput;
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Transform camPos;

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponent<Camera>();
        playerInput = new Controls();
        playerInput.OnFoot.Enable();

        mousePosStart = GetMousePos();
    }

    public Vector2 GetMousePos()
    {
        mousePos = playerInput.OnFoot.Mouse.ReadValue<Vector2>();
        return mousePos;
    }

    void Update()
    {

        mousePos = GetMousePos();


        float tick = Time.realtimeSinceStartup;

        if (tick > 5)
        {
            float mouseX = Time.deltaTime * mousePos.x * mouseSensitivity;
            float mouseY = Time.deltaTime * mousePos.y * mouseSensitivity;

            Xrot -= mouseY;
            Xrot = Mathf.Clamp(Xrot, -270f, 270f);
            Yrot += mouseX;

            transform.localRotation = Quaternion.Euler(Xrot * 360 / camera.pixelWidth, Yrot * 360 / camera.pixelHeight, 0f);

        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }

    private void FixedUpdate()
    {
        Quaternion lookRot = Quaternion.FromToRotation(rb.transform.forward.normalized, new Vector3(transform.forward.x, 0, transform.forward.z).normalized);
        rb.rotation *= lookRot;
        camPos.rotation = camPos.rotation * Quaternion.Inverse(lookRot);
    }
}
