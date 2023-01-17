using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Nobi.UiRoundedCorners;
public class SettingsMenuSizes : MonoBehaviour
{

    public Vector2[] sizes;
    public string[] sizeNames;
    public GameObject[] menus;
    public int DesiredMenu;

    private void Start()
    {
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1);
    }
    public void ChangeSize(string sizeString)
    {
        
        float[] size = new float[2];
        for(int i = 0; i < sizeNames.Length; i++)
        {
            if(sizeNames[i] == sizeString)
            {
                DesiredMenu = i;
                size[0] = sizes[i][0];
                size[1] = sizes[i][1];
            }
        }
        Rect rect = GetComponent<RectTransform>().rect;
        float width = rect.width;
        DOTween.To(() => width, x => width = x, size[0], 0.5f).OnUpdate(() =>
        {
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        });
        float height = rect.height;
        DOTween.To(() => height, x => height = x, size[1], 0.5f).OnUpdate(() =>
        {
            GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        });
        Invoke("ChangeMenu", 0.3f);
    }

    public void ChangeMenu()
    {
        
        try { 
            foreach (GameObject menu in menus)
            {
                menu.active = false;
            }
            menus[DesiredMenu].active = true;
        }
        catch { }
    }

}
