using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public GameObject cameraHolder;

    public float mouseRotationX;
    public float mouseRotationY;

    public float mouseSensitivity = 1f;

    public GameObject playerOrientation;

    float CameraMoveEdits;

    Camera cam;

    private CanvasGroup ads;

    private RaycastHit InteractRay;

    TMP_Text interactText;

    [SerializeField] Transform interactPos;

    GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();

        cam = GetComponent<Camera>();
        ads = GameObject.Find("ads").GetComponent<CanvasGroup>();
        ads.alpha = 0;

        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);
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

        CameraMoveEdits = Mathf.LerpAngle (currentCamRot, targetCamRot, Time.deltaTime * 10);

        transform.localEulerAngles = new Vector3(-mouseRotationY, mouseRotationX, CameraMoveEdits);

        playerOrientation.transform.localEulerAngles = new Vector3(transform.rotation.x, mouseRotationX, transform.rotation.z);

        if(Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 20, Time.deltaTime * 10);
            ads.alpha = Mathf.Lerp(ads.alpha, 1, Time.deltaTime * 5);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, Time.deltaTime * 10);
            ads.alpha = Mathf.Lerp(ads.alpha, 0, Time.deltaTime * 5);
        }

        if (Physics.Raycast(interactPos.position, interactPos.transform.forward, out InteractRay, 5f, 1 << LayerMask.NameToLayer("Interactible")))
        {
            Debug.DrawRay(interactPos.position, interactPos.transform.forward * 5f, Color.green);
            Debug.Log("Interacting");
            interactText.text = "Press [E] to interact";

            if(Input.GetKeyDown(KeyCode.E))
            {
                Destroy(InteractRay.transform.gameObject);
                gameController.SetHeldItem(InteractRay.transform.gameObject.tag);

                if(InteractRay.transform.gameObject.tag == "rifle" || InteractRay.transform.gameObject.tag == "shotgun")
                {
                    gameController.magazineAmmo = InteractRay.transform.gameObject.GetComponent<GunPrefab>().magazineAmmunition;
                }
                
            }
        }
        else
        {
            Debug.DrawRay(interactPos.position, interactPos.transform.forward * 5f, Color.red);
            interactText.text = "";
        }
    }
}
