using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public float sensitivity;
    // Start is called before the first frame update
    void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
