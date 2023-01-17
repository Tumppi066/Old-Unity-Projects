using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InformationPanel : MonoBehaviour
{
    public void Close()
    {
        transform.DOMoveX(Screen.width * 2, 0.25f);
    }
}
