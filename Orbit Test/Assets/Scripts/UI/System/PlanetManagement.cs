using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class PlanetManagement : MonoBehaviour
{
    public GameObject targetedBody;
    public Image fade;

    public Camera camera;
    public Vector3 offset;

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        fade = GetComponent<Image>();
    }

    private void Update()
    {
        if (targetedBody == null)
        {
            gameObject.transform.DOLocalMove(new Vector3(0, 500, 0), 0.1f);
            fade.color = new Color(fade.color.g, fade.color.g, fade.color.b, 0);
        }
        else
        {
            float newScale = (targetedBody.GetComponent<BodyInformation>().radius * 3) / camera.orthographicSize;
            gameObject.transform.localScale = new Vector3(newScale, newScale, newScale);
            gameObject.transform.DOMove(camera.WorldToScreenPoint(targetedBody.transform.position) + offset, 0.1f);
            fade.color = new Color(fade.color.g, fade.color.g, fade.color.b, 255);
        }
    }
}
