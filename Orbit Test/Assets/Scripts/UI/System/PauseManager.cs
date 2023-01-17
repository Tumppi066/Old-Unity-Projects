using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private Universe Universe;
    public float previousTimeStep;
    public Color normalColor;
    public Color pausedColor;
    public bool isPaused;

    void Start()
    {
        Universe = FindObjectOfType<Universe>();    
    }

    public void PauseUnPause()
    {
        isPaused = !isPaused;
        Universe.timeStep = previousTimeStep;
    }
    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseUnPause();
        }

        if (!isPaused)
        {
            previousTimeStep = Universe.timeStep;
            GetComponent<Image>().color = normalColor;
            if(Universe.timeStep == 0)
            {
                GetComponent<Image>().color = pausedColor;
            }
        }
        if (isPaused)
        {
            Universe.timeStep = 0;
            GetComponent<Image>().color = pausedColor;
        }
    }

}
