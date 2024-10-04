using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandsLook : MonoBehaviour
{
    public GameObject cameraHolder;

    public float mouseRotationX;
    public float mouseRotationY;

    public float mouseSensitivity = 1f;

    public GameObject playerOrientation;

    float CameraMoveEdits;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraFollow = cameraHolder.transform.position;
        transform.position = cameraFollow;

        mouseRotationX += (Input.GetAxisRaw("Mouse X") * mouseSensitivity) / 2;
        mouseRotationY += (Input.GetAxisRaw("Mouse Y") * mouseSensitivity) / 2;

        mouseRotationY = Mathf.Clamp(mouseRotationY, -90f, 90f);

        float currentCamRot = transform.eulerAngles.z;
        float targetCamRot = -Input.GetAxisRaw("Horizontal");

        CameraMoveEdits = Mathf.LerpAngle(currentCamRot, targetCamRot, Time.deltaTime * 10);

        transform.localEulerAngles = new Vector3(-mouseRotationY, mouseRotationX, CameraMoveEdits);

        playerOrientation.transform.localEulerAngles = new Vector3(transform.rotation.x, mouseRotationX, transform.rotation.z);

        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 20, Time.deltaTime * 10);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, Time.deltaTime * 10);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            mouseSensitivity += 0.1f;
            Debug.Log($"Mousesens: {mouseSensitivity} ");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            mouseSensitivity -= 0.1f;
            Debug.Log($"Mousesens: {mouseSensitivity} ");
        }
    }
}
