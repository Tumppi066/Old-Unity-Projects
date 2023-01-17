using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class PlanetIconManager : MonoBehaviour
{
    private Universe universe;   
    private nBodySimulation nBodySimulation;
    private CameraControl cameraControl;
    public GameObject planetIconPrefab;
    public GameObject[] planetIcons = new GameObject[10000];
    public string zoomLevel = "Star"; // Star, Planet, Moon
    public float planetLimit = 100f;
    public float moonLimit = 20f;
    public float groupDistanceLimitX = 175f;
    public float groupDistanceLimitY = 70f;
    // Start is called before the first frame update
    void Start()
    {
        universe = GameObject.FindObjectOfType<Universe>();
        nBodySimulation = GameObject.FindObjectOfType<nBodySimulation>();
        cameraControl = GameObject.FindObjectOfType<CameraControl>();
    }

    void SetPlanetIconTexts(GameObject icon, Color color, string name, string details)
    {
        TMP_Text[] planetTexts = icon.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text planetText in planetTexts)
        {
            try
            {
                planetText.color = color;
            }
            catch
            {
                planetText.color = new Color(255,206,0,255);
            }
        }
        planetTexts[0].text = name;
        planetTexts[1].text = details;
    }

    // Update is called once per frame
    void Update()
    {


        Body[] bodies;

        float currentZoom = Camera.main.orthographicSize;
        if (currentZoom < moonLimit && zoomLevel != "Moon")
        {
            zoomLevel = "Moon";
            universe.Print("Zoom level changed to <color=yellow>Moon</color>", gameObject);
        }
        else if (currentZoom < planetLimit && currentZoom > moonLimit && zoomLevel != "Planet")
        {
            zoomLevel = "Planet";
            universe.Print("Zoom level changed to <color=yellow>Planet</color>", gameObject);
        }
        else if (currentZoom > planetLimit && zoomLevel != "Star")
        {
            zoomLevel = "Star";
            universe.Print("Zoom level changed to <color=yellow>Star</color>", gameObject);
        }

        if(zoomLevel == "Planet")
        {
            bodies = nBodySimulation.planets;
        }
        else if(zoomLevel == "Moon")
        {
            bodies = nBodySimulation.moons;
        }
        else
        {
            bodies = new Body[nBodySimulation.stars.Length + nBodySimulation.planets.Length];
            nBodySimulation.planets.CopyTo(bodies, 0);
            nBodySimulation.stars.CopyTo(bodies, nBodySimulation.planets.Length);
        }

        for (int i = 0; i < bodies.Length; i++)
        {
            if (planetIcons[i] == null)
            {
                planetIcons[i] = Instantiate(planetIconPrefab, transform);
            }
        }

        if(planetIcons != null)
        {
            foreach(GameObject planetIcon in planetIcons)
            {


                // Group the icons if they are close together
                // And then disable the ones that are not needed
                // And change the text to show the amount of planets in that group
                // And change the color to show the average color of the planets in that group
                if(planetIcon != null && planetIcon.active == true)
                {
                    
                    bool foundMatch = true;
                    float iterations = 0; // Make sure we don't get stuck in an infinite loop
                    while(foundMatch == true)
                    {
                        foundMatch = false;
                        foreach (GameObject planetIcon2 in planetIcons)
                        {
                            if (planetIcon != planetIcon2 && planetIcon2 != null && planetIcon2.active != false)
                            {
                                Vector2 Distance = planetIcon.transform.position - planetIcon2.transform.position;
                                // Check for negative values
                                if (Distance.x < 0){ Distance.x *= -1; }
                                if (Distance.y < 0){ Distance.y *= -1; }
                                
                                if (Distance.x < groupDistanceLimitX || Distance.y < groupDistanceLimitY)
                                {
                                    foundMatch = true;
                                    PlanetIconContainer planetIconContainer = planetIcon.GetComponent<PlanetIconContainer>();
                                    planetIconContainer.isGroupLeader = true;
                                    planetIconContainer.groupedBodyCount++;
                                    planetIconContainer.groupedBodies[planetIconContainer.groupedBodyCount] = planetIcon2;
                                    planetIcon2.active = false;
                                }
                            }
                        }
                        iterations++;
                        if(iterations > 100)
                        {
                            foundMatch = false;
                        }
                    }
                }
                else if(planetIcon != null && planetIcon.active == false)
                {
                    bool tooClose = false;
                    foreach(GameObject planetIcon2 in planetIcons)
                    {
                        if(planetIcon != planetIcon2 && planetIcon2 != null && planetIcon2.active != false)
                        {

                            Vector2 Distance = planetIcon.transform.position - planetIcon2.transform.position;
                            // Check for negative values
                            if (Distance.x < 0){ Distance.x *= -1; }
                            if (Distance.y < 0){ Distance.y *= -1; }

                            if(Distance.x < groupDistanceLimitX || Distance.y < groupDistanceLimitY)
                            {
                                tooClose = true;
                            }
                        }
                    }
                    if(tooClose == false)
                    {
                        planetIcon.active = true;
                    }
                }

                if(planetIcon != null && planetIcon.GetComponent<PlanetIconContainer>().isGroupLeader)
                {
                    bool tooClose = false;
                    foreach(GameObject planetIcon2 in planetIcons)
                    {
                        if(planetIcon != planetIcon2 && planetIcon2 != null && planetIcon2.active == false)
                        {
                            Vector2 Distance = planetIcon.transform.position - planetIcon2.transform.position;
                            // Check for negative values
                            if (Distance.x < 0){ Distance.x *= -1; }
                            if (Distance.y < 0){ Distance.y *= -1; }

                            if(Distance.x < groupDistanceLimitX || Distance.y < groupDistanceLimitY)
                            {
                                tooClose = true;
                            }
                        }
                    }
                    if(tooClose == false)
                    {
                        planetIcon.GetComponent<PlanetIconContainer>().isGroupLeader = false;
                        planetIcon.GetComponent<PlanetIconContainer>().groupedBodyCount = 0;
                        for(int i = 0; i < 100; i++)
                        {
                            planetIcon.GetComponent<PlanetIconContainer>().groupedBodies[i] = null;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < bodies.Length; i++)
        {
            //planetIcons[i].transform.DOMove(Camera.main.WorldToScreenPoint(bodies[i].transform.position), 0.01f);
            //PlanetIconContainer planetIconContainer = planetIcons[i].GetComponent<PlanetIconContainer>();
            //if(planetIconContainer.isGroupLeader)
            //{
            //    planetIcons[i].transform.position = planetIconContainer.groupCenter;
            //    SetPlanetIconTexts(planetIcons[i], Color.white, "Group", planetIconContainer.groupedBodyCount.ToString() + " bodies");
            //}
            //else if(planetIcons[i].active == true)
            //{

                planetIcons[i].transform.position = Camera.main.WorldToScreenPoint(bodies[i].transform.position);

                try
                {
                    SetPlanetIconTexts(planetIcons[i], bodies[i].GetComponent<SpriteRenderer>().color, bodies[i].name, bodies[i].GetComponent<BodyInformation>().amountOfMoons.ToString() + " moons");
                }
                catch
                {
                    SetPlanetIconTexts(planetIcons[i], new Color(255,206,0,255), bodies[i].name, (bodies[i].GetComponent<BodyInformation>().amountOfMoons + 1).ToString() + " bodies");
                }
            //}



            //if(zoomLevel == "Planet")
            //{
            //    planetIcons[i].transform.localScale = Vector3.one * 2;
            //}
            //else if(zoomLevel == "Moon")
            //{
            //    planetIcons[i].transform.localScale = Vector3.one * 1;
            //}
            //else
            //{
            //    planetIcons[i].transform.localScale = Vector3.one * 10;
            //}
        }

        for (int i = bodies.Length; i < planetIcons.Length; i++)
        {
            if (planetIcons[i] != null)
            {
                Destroy(planetIcons[i]);
            }
        }

    }
}
