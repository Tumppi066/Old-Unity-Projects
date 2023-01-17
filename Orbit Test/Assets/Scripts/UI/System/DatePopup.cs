using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DatePopup : MonoBehaviour
{
    public Vector2 popupSize;
    public GameObject popup;
    public GameObject button;
    public bool isOpen;
    float height;
    float width;
    private void Start()
    {
        Invoke("ResetPopup", 0.25f);
    }
    public void ResetPopup()
    {
        popup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        popup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
    }
    public void ChangeState()
    {
        isOpen = !isOpen;
        button.transform.DORotate(new Vector3(0, 0, isOpen ? -180 : 0), 0.25f);
        if (isOpen)
        {
            DOTween.To(() => width, x => width = x, popupSize[0], 0.25f).OnUpdate(() =>
            {
                popup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            });
            DOTween.To(() => height, x => height= x, popupSize[1], 0.25f).OnUpdate(() =>
            {
                popup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            });
        }
        if (!isOpen)
        {
            DOTween.To(() => width, x => width = x, 0, 0.25f).OnUpdate(() =>
            {
                popup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            });
            DOTween.To(() => height, x => height = x, 0, 0.25f).OnUpdate(() =>
            {
                popup.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            });
        }
    }
}
