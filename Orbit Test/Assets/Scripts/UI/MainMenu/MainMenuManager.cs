using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Michsky.UI;
public class MainMenuManager : MonoBehaviour
{

    private bool resolutionChange;
    private bool wasResolutionChange;
    private float physicsFPS = 100f;

    private void Start()
    {
        physicsFPS = 1 / Time.fixedDeltaTime;
        FindObjectOfType<Michsky.UI.ModernUIPack.SliderManager>().mainSlider.value = 1/Time.fixedDeltaTime;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void LoadRandomSystem()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void ChangePhysicsFPS(float newFPS)
    {
        Time.fixedDeltaTime = 1f / newFPS;
        physicsFPS = 1f / newFPS;
        if(Application.targetFrameRate > 0)
        {
            Application.targetFrameRate = (int)newFPS;
        }
    }

    public void Fullscreen(bool on)
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, on);
        Debug.Log("<color=yellow>ScreenManager</color> Set fullscreen to <color=green>" + on + "</color> and resolution to <color=green>" + Screen.resolutions[Screen.resolutions.Length - 1].width + "x" + Screen.resolutions[Screen.resolutions.Length - 1].height + "</color>");
    }

    private void OnRectTransformDimensionsChange()
    {
        resolutionChange = true;
    }
    
    public void CapFPSToPhysics(bool OnOff)
    {
        if (OnOff)
        {
            Debug.Log(physicsFPS);
            Application.targetFrameRate = (int)(1/physicsFPS);
            Debug.Log("<color=yellow>ScreenManager</color> Set application target framerate to <color=green>" + Application.targetFrameRate + "</color>.");
        }
        else
        {
            Application.targetFrameRate = -1;
            Debug.Log("<color=yellow>ScreenManager</color> Framerate uncapped.");
        }
    }

    private void Update()
    {
        if (resolutionChange)
        {
            if (!wasResolutionChange)
            {
                resolutionChange = false;
                wasResolutionChange = true;
                Debug.Log("<color=yellow>ScreenManager</color> New resolution is <color=green>" + Screen.width + "x" + Screen.height + "</color>");
            }
            else
            {
                wasResolutionChange = false;
            }
        }
    }

}
