using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuStuff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, 1 * Time.deltaTime, 0);
    }
}
