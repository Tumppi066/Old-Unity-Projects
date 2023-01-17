using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRescaler : MonoBehaviour
{
    public void ChangeAllCanvasSize(float size)
    {
        Canvas[] all = FindObjectsOfType<Canvas>();
        foreach (Canvas c in all)
        {
            c.scaleFactor = size;
        }
    }
}
