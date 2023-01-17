using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentCanvasManager : MonoBehaviour
{
    // Fps and performance counter options
    public TMP_Text fps;
    public TMP_Text tweens;
    private float FPS;
    public TMP_Text physicsTime;
    private float physicsTimeValue;
    public TMP_Text orbitTime;
    private float orbitTimeValue;
    public TMP_Text overallTime;
    private float overallTimeValue;
    public TMP_Text seed;
    public TMP_Text physicsFPS;

    // Console handling options
    public GameObject consolePrefab;
    public GameObject console;

    private void Start()
    {

        if (SceneManager.GetActiveScene().name == "PersistentScene")
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log("<color=yellow>PersistentCanvasManager</color> Debug initialization done, loading mainmenu.");
            SceneManager.LoadScene("MainMenu");
        }

        foreach (GameObject ob in GameObject.FindGameObjectsWithTag("ConsoleLog"))
        {
            Destroy(ob);
        }
        Application.logMessageReceived += ShowLog;
    }

    private void Update()
    {
        // Handle fps and performance counters
        Tween tween = DOTween.To(() => FPS, x => FPS = x, (1 / Time.deltaTime), 0.25f);
        tween.SetEase(Ease.Linear);
        fps.text = "FPS : " + FPS.ToString("0.0");
        
        if(FPS < 1/ Time.fixedDeltaTime - 1)
        {
            fps.color = Color.red;
            overallTime.color = Color.red;
        }
        else if (FPS > 1 / Time.fixedDeltaTime + 1)
        {
            fps.color = Color.white;
            overallTime.color = Color.white;
        }
        else
        {
            fps.color = Color.green;
            overallTime.color = Color.green;
        }

        physicsFPS.text = "Physics FPS : " + (1 / Time.fixedDeltaTime).ToString("0.0");
        tweens.text = "Tweens : " + DOTween.PlayingTweens().Count.ToString();
        tween = DOTween.To(() => overallTimeValue, x => overallTimeValue = x, (Time.deltaTime * 1000), 0.25f);
        tween.SetEase(Ease.Linear);
        overallTime.text = "Frametime : " + overallTimeValue.ToString("0.00") + "ms / " + (Time.fixedDeltaTime*1000).ToString("0.00") + "ms";

        if (FindObjectOfType<StarSystemSpawner>())
        {
            seed.text = "Seed : " + FindObjectOfType<StarSystemSpawner>().seed.ToString();
        }
        try
        {
            nBodySimulation simulation = FindObjectOfType<nBodySimulation>();
            physicsTime.gameObject.SetActive(true);
            orbitTime.gameObject.SetActive(true);
            float physicsUpdateTime = 0;
            foreach (Body ob in FindObjectsOfType<Body>())
            {
                physicsUpdateTime += ob.msToUpdate;
            }
            tween = DOTween.To(() => physicsTimeValue, x => physicsTimeValue = x, (physicsUpdateTime), 0.25f);
            tween.SetEase(Ease.Linear);
            physicsTime.text = "Physics update time : " + Math.Round(physicsTimeValue, 2).ToString() + "ms";
            float orbitDrawTime = 0;
            foreach (DrawOrbit orb in FindObjectsOfType<DrawOrbit>())
            {
                orbitDrawTime += orb.msToUpdate;
            }
            tween = DOTween.To(() => orbitTimeValue, x => orbitTimeValue = x, (orbitDrawTime), 0.25f);
            tween.SetEase(Ease.Linear);
            orbitTime.text = "Orbit update time : " + Math.Round(orbitTimeValue, 2).ToString() + "ms";
        }
        catch{
            physicsTime.gameObject.SetActive(false);
            orbitTime.gameObject.SetActive(false);
        }

    }

    public void DeleteLogs(GameObject ob)
    {
        GameObject[] obs = GameObject.FindGameObjectsWithTag("ConsoleLog");
        if (obs.Length > 0)
        {
            foreach (GameObject o in obs)
            {
                if (o.GetComponent<TMP_Text>().text.Contains(ob.name))
                {
                    Destroy(o);
                }
            }
        }
    }

    void ShowLog(string logString, string stackTrace, LogType type)
    {
        GameObject prefab = Instantiate(consolePrefab, console.transform);
        TMP_Text prefabText = prefab.GetComponent<TMP_Text>();
        prefabText.text = logString;
        if (type != LogType.Log)
        {
            if (type == LogType.Error)
            {
                prefabText.color = Color.red;
            }
            else if (type == LogType.Warning)
            {
                prefabText.color = Color.yellow;
            }
            else if (type == LogType.Exception)
            {
                prefabText.color = Color.magenta;
                prefabText.text += "\n" + stackTrace;
            }
        }
        Destroy(prefab, 8);
    }
}
