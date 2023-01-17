using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public bool isEscMenuOpen = true;
    public bool isSettingTabsOpen = true;
    public GameObject settingsTabs;
    public GameObject settingsMenu;
    public Vector2 size;
    public float time = 0.25f;
    public void EscMenu()
    {
        isEscMenuOpen = !isEscMenuOpen;
    }
    public void SettingTabs()
    {
        isSettingTabsOpen = !isSettingTabsOpen;
    }
    void Update()
    {
        if (isEscMenuOpen && settingsMenu != null)
        {
            Rect rect = GetComponent<RectTransform>().rect;
            float width = rect.width;
            if(size != new Vector2(0, 0))
            {
                DOTween.To(() => width, x => width = x, size[0], time).OnUpdate(() =>
                {
                    GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                });
                float height = rect.height;
                DOTween.To(() => height, x => height = x, size[1], time).OnUpdate(() =>
                {
                    GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                });
            }
            else if(settingsMenu != null)
            {
                DOTween.To(() => width, x => width = x, 100, time).OnUpdate(() =>
                {
                    GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                });
                float height = rect.height;
                DOTween.To(() => height, x => height = x, 100, time).OnUpdate(() =>
                {
                    GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                });
            }
        }
        else
        {
            Rect rect = GetComponent<RectTransform>().rect;
            float width = rect.width;
            DOTween.To(() => width, x => width = x, 0, time).OnUpdate(() =>
            {
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            });
            float height = rect.height;
            DOTween.To(() => height, x => height = x, 0, time).OnUpdate(() =>
            {
                GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            });
        }
        if (isSettingTabsOpen && isEscMenuOpen && settingsTabs != null)
        {
            Rect rect = settingsTabs.GetComponent<RectTransform>().rect;
            float width = rect.width;
            DOTween.To(() => width, x => width = x, 100, time).OnUpdate(() =>
            {
                settingsTabs.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            });
            float height = rect.height;
            DOTween.To(() => height, x => height = x, 150, time).OnUpdate(() =>
            {
                settingsTabs.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            });
        }
        else if(settingsTabs != null)
        {
            FindObjectOfType<SettingsMenuSizes>().ChangeSize("Closed");
            Rect rect = settingsTabs.GetComponent<RectTransform>().rect;
            float width = rect.width;
            DOTween.To(() => width, x => width = x, 0, time).OnUpdate(() =>
            {
                settingsTabs.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            });
            float height = rect.height;
            DOTween.To(() => height, x => height = x, 0, time).OnUpdate(() =>
            {
                settingsTabs.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            });
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
