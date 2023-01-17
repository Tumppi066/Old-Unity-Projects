using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BodyInformationDisplay : MonoBehaviour
{
    public GameObject targetedBody;
    public Image fade;
    public TMP_Text[] textBoxes;
    public Camera camera;
    public Vector3 offset;
    private Universe Universe;
    /*
    Name
    Mass
    Distance
    Radius
    Moons
    */
    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        fade = GetComponent<Image>();
        Universe = FindObjectOfType<Universe>();
    }

    private void Update()
    {
        if(targetedBody == null)
        {
            //gameObject.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            fade.color = new Color(fade.color.g, fade.color.g, fade.color.b, 0);
        }
        else
        {
            gameObject.transform.DOMove(camera.WorldToScreenPoint(targetedBody.transform.position) + offset, 0.1f);
            textBoxes[0].text = targetedBody.name;
            BodyInformation BI = targetedBody.GetComponent<BodyInformation>();
            textBoxes[1].text = Math.Round(BI.mass, Universe.valueRounding).ToString();
            textBoxes[2].text = Math.Round(BI.radius, Universe.valueRounding).ToString();
            textBoxes[3].text = BI.amountOfMoons.ToString();

            fade.color = new Color(fade.color.g, fade.color.g, fade.color.b, 0.01f);
        }
        targetedBody = null;
    }
}
