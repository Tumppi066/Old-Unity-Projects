using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniverseSettings : MonoBehaviour
{
    public bool useNBodyPhysics = false;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        try
        {
            if (useNBodyPhysics)
            {
                FindObjectOfType<Universe>().useNBodyPhysics = true;
            }
            else
            {
                FindObjectOfType<Universe>().useNBodyPhysics = false;
            }
        }
        catch { }
    }

    public void SetNBodyPhysics(bool isOn)
    {
        if (isOn)
        {
            useNBodyPhysics = true;
        }
        else
        {
            useNBodyPhysics = false;
        }
    }
}
