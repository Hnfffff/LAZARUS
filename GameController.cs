using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private int shotGunAmmo;
    [SerializeField] private int rifleAmmo;

    [SerializeField] private string heldItem;

    [SerializeField] public GameObject rifle;
    [SerializeField] public GameObject shotgun;

    [SerializeField] public GameObject mainUI;
    [SerializeField] public GameObject pauseUI;
    [SerializeField] public GameObject settingsUI;

    public int magazineAmmo;

    [SerializeField] public GameObject mainCamera;
    [SerializeField] public GameObject handCamera;

    public bool isPaused;

    [SerializeField] private AudioSource[] Audiosources;

    [SerializeField] public AudioSource pauseMusic;

    PlayerLook playerLook;


    // Start is called before the first frame update
    void Start()
    {
        shotGunAmmo = 32;
        rifleAmmo = 32;

        rifle = GameObject.Find("Main Camera").transform.GetChild(2).gameObject;
        shotgun = GameObject.Find("Main Camera").transform.GetChild(1).gameObject;
        playerLook = GameObject.Find("Main Camera").GetComponent<PlayerLook>();


        heldItem = null;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (heldItem)
        {
            case "rifle":
                rifle.SetActive(true);
                break;

            case "shotgun":
                shotgun.SetActive(true);
                break;

            case "none":
                break;

            default:

                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(mainUI.activeSelf == true)
            {
                mainUI.SetActive(false);
                pauseUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                mainCamera.GetComponent<PlayerLook>().enabled = false;
                handCamera.GetComponent<HandsLook>().enabled = false;
                Time.timeScale = 0f;
                PauseAllAudio();
                isPaused = true;
                pauseMusic.Play();
            }

            else if(pauseUI.activeSelf == true)
            {
                if (pauseUI != null)
                {
                    pauseUI.SetActive(false);
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mainCamera.GetComponent<PlayerLook>().enabled = true;
                handCamera.GetComponent<HandsLook>().enabled = true;

                mainUI.SetActive(true);
                Time.timeScale = 1.0f;
                isPaused = false;
                pauseMusic.Stop();
                ResumeAllAudio();
            }

            else if(settingsUI.activeSelf == true)
            {
                if (settingsUI != null)
                {
                    settingsUI.SetActive(false);
                }

                if (mainUI != null)
                {
                    mainUI.SetActive(false);
                }

                pauseUI.SetActive(true);
            }

        }
    }

    public int GetShotgunAmmo()
    {
        return shotGunAmmo;
    }

    public int ChangeShotgunAmmo(int change)
    {
        shotGunAmmo += change;
        return 1;
    }

    public void SetShotgunAmmo(int value)
    {
        shotGunAmmo = value;
    }

    public int GetRifleAmmo()
    {
        return rifleAmmo;
    }

    public int ChangeRifleAmmo(int change)
    {
        rifleAmmo += change;
        return 1;
    }

    public int SetRifleAmmo(int  value)
    {
        rifleAmmo = value;
        return 1;
    }

    public void SetHeldItem(string item)
    {
        heldItem = item;
    }

    public void PauseAllAudio()
    {
        foreach (AudioSource currAudioSource in Audiosources)
        {
            currAudioSource.Pause();
        }
    }

    public void ResumeAllAudio()
    {
        foreach (AudioSource currAudioSource in Audiosources)
        {
            currAudioSource.Play();
        }
    }

    public void ApplySettings()
    {
        if(playerLook != null)
        {
            playerLook.mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);
            Debug.Log("Setting MouseSense");
        }
    }

}
