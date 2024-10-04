using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite hoverSprite;

    [SerializeField] AudioSource hoverSource;

    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainUI;

    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject handCamera;

    GameController gameController;
    [SerializeField] Slider sensSlider;

    [SerializeField] SettingsController settingsController;
    float pitch;

    // Start is called before the first frame update
    private void Start()
    {
        if(gameObject.GetComponent<Image>() != null)
        {
            gameObject.GetComponent<Image>().sprite = baseSprite;
        }

        if (sensSlider != null)
        {
            sensSlider.value = settingsController.sensitivity;
            sensSlider.onValueChanged.AddListener(SliderChangeValue);
        }


        try
        {
            gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        }
        catch
        {
            Debug.Log("NO GAME CONTROLLER");
        }
    }

    void OnEnable()
    {
        if( sensSlider != null )
        {
            sensSlider.value = settingsController.sensitivity;
            sensSlider.onValueChanged.AddListener(SliderChangeValue);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void Hover()
    {
        hoverSource.pitch = Random.Range(1f, 1.05f);
        hoverSource.Play();
        gameObject.GetComponent<Image>().sprite = hoverSprite;
    }

    public void UnHover()
    {
        gameObject.GetComponent<Image>().sprite = baseSprite;
    }

    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ToSettings()
    {
        if(mainMenu != null)
        {
            mainMenu.SetActive(false);
        }

        if(pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        gameObject.GetComponent<Image>().sprite = baseSprite;
        settingsMenu.SetActive(true);
    }

    public void ToMainMenu()
    {
        if(settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        gameObject.GetComponent<Image>().sprite = baseSprite;
        mainMenu.SetActive(true);
    }

    public void ToPauseMenu()
    {
        if(settingsMenu !=null)
        {
            settingsMenu.SetActive(false);
        }

        if(mainUI != null)
        {
            mainUI.SetActive(false);
        }

        gameObject.GetComponent <Image>().sprite = baseSprite;
        pauseMenu.SetActive(true);
    }

    public void ToStartScreen()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Resume()
    {
        if(pauseMenu!=null)
        {
            pauseMenu.SetActive(false);
        }
        gameObject.GetComponent<Image>().sprite=baseSprite;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera.GetComponent<PlayerLook>().enabled = true;
        handCamera.GetComponent<HandsLook>().enabled = true;

        mainUI.SetActive(true);
        Time.timeScale = 1.0f;
        if(gameController != null)
        {
            gameController.pauseMusic.Stop();
            gameController.isPaused = false;
            gameController.ResumeAllAudio();
        }
        
    }

    public void Apply()
    {
        if(sensSlider != null)
        {
            Debug.Log($"Slider Value: {sensSlider.value}");
            settingsController.sensitivity = sensSlider.value;
        }

        //Debug.Log(settingsController.sensitivity);

        Debug.Log($"Current Player Prefs Sensitivity: { PlayerPrefs.GetFloat("Sensitivity", 1f)}");
        Debug.Log("Attempting Save");
        PlayerPrefs.SetFloat("Sensitivity", settingsController.sensitivity);
        PlayerPrefs.Save();
        Debug.Log("Save Complete");
        Debug.Log($"New Player Prefs Sensitivity: {PlayerPrefs.GetFloat("Sensitivity", 1f)}");

        if (gameController != null)
        {
            gameController.ApplySettings();
        }
        
    }

    public void SliderChangeValue(float value)
    {
        pitch = value;
        hoverSource.pitch = pitch;
        hoverSource.Play();
        
        //Debug.Log($"Mouse Sensitivity: {pitch}");
    }
}