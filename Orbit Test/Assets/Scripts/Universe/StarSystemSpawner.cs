using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using TMPro;
using System.Threading;

public class StarSystemSpawner : MonoBehaviour
{

    [Header("Everything")]
    public bool randomPe;
    public bool allowWhenNbody = false;
    public int seed;
    public TMP_InputField seedInput;
    public bool isCreatingSystem;
    public SliderManager creationSlider;
    [Header("Planets")]
    public int maxPlanets;
    public int minPlanets;
    public float maxPlanetDistance;
    public float minPlanetDistance;
    [Range(0,1)]
    public float planetMaxSizeOfStar;
    public float planetMinSize;
    public float planetSeparation;
    public GameObject planetPrefab;
    public PlanetTypes[] planetTypes;
    [Header("Moons")]
    public int maxMoons;
    public int minMoons;
    public float minMoonDistance;
    public float moonSeparationSizeOfPlanet;
    [Range(0,1)]
    public float moonMaxSizeOfPlanet;
    [Range(0, 1)]
    public float moonMinSizeOfPlanet;
    public GameObject moonPrefab;
    public PlanetTypes[] moonTypes;
    [Header("Star")]
    public float starMaxSize;
    public float starMinSize;
    public GameObject mainStar;

    private float maxMoonDistance;
    private float[] planetXPositions;
    private float[] planetSizes;
    private float[] planetMass;
    private int[] planetTypeIDs;
    private GameObject[] planets;
    private GameObject[] moons;
    private Universe Universe;
    private float timeTaken;

    private void Start()
    {
        creationSlider.gameObject.active = false;
        Universe = FindObjectOfType<Universe>();
        maxMoonDistance = planetSeparation / 2.5f;
    }


    public void ChangeSystemWithKnownSeed()
    {
        Universe.DeleteLogs(gameObject);
        if (!Universe.useNBodyPhysics || allowWhenNbody)
        {
            seed = int.Parse(seedInput.text);
            Random.seed = seed;
            Universe.Print("Starting new system generation with known seed : " + seed, gameObject);
            if (maxPlanets >= 1000)
            {
                Universe.Print("<color=red>Warning</color> You have chosen a maximum of " + maxPlanets + " planets, this may take a while", gameObject);
            }
            if (maxMoons >= 500)
            {
                Universe.Print("<color=red>Warning</color> You have chosen a maximum of " + maxMoons + " moons, this may take a while", gameObject);
            }
            Invoke("StartSpawningSystem", 0.01f);
        }
        else
        {
            Universe.Print("Automatic system generation not available when NBody Physics is enabled.", gameObject);
        }
    }

    public void ChangeSystem()
    {
        if (!isCreatingSystem)
        {       
            FindObjectOfType<Universe>().DeleteLogs(gameObject);
            if (!Universe.useNBodyPhysics || allowWhenNbody)
            {
                seed = (int)System.DateTime.Now.Ticks;
                Random.seed = seed;
                Universe.Print("Starting new system generation with seed : " + seed, gameObject);
                creationSlider.gameObject.SetActive(true);
                if (maxPlanets >= 1000)
                {
                    Universe.Print("<color=red>Warning</color> You have chosen a maximum of " + maxPlanets + " planets, this may take a while", gameObject);
                }
                if (maxMoons >= 500)
                {
                    Universe.Print("<color=red>Warning</color> You have chosen a maximum of " + maxMoons + " moons, this may take a while", gameObject);
                }
                Invoke("StartSpawningSystem", 0.01f);
            }
            else
            {
                Universe.Print("Automatic system generation not available when NBody Physics is enabled.", gameObject);
            }
        }
        else
        {
            Universe.Print("<color=red>Warning</color> Can't create a new system while one is already being created", gameObject);
        }
    }

    private void StartSpawningSystem()
    {
        timeTaken = 0;
        creationSlider.mainSlider.value = 0;
        isCreatingSystem = true;
        creationSlider.mainSlider.maxValue = maxPlanets + maxMoons;
        IEnumerator planetCoroutine = SpawnPlanets();
        StartCoroutine(planetCoroutine);
    }

    IEnumerator SpawnPlanets()
    {
        float start = Time.realtimeSinceStartup;
        creationSlider.gameObject.SetActive(true);
        if (planets != null)
        {
            foreach (GameObject planet in planets)
            {
                if (planet != null)
                {
                    Destroy(planet.GetComponent<DrawOrbit>().line.gameObject);
                    Destroy(planet);
                }
            }
        }
        int planetCount = Random.Range(minPlanets, maxPlanets);
        float starSize = Random.Range(starMinSize, starMaxSize);
        creationSlider.maxValue = planetCount;
        mainStar.transform.localScale = new Vector3(starSize, starSize, starSize);
        mainStar.GetComponentInParent<CircleCollider2D>().radius = starSize / 2;
        mainStar.GetComponentInParent<BodyInformation>().radius = starSize;
        mainStar.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().pointLightInnerRadius = starSize;
        mainStar.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().pointLightOuterRadius = maxPlanetDistance * 2f;
        minPlanetDistance += starSize;

        // Calculate planet x positions
        planetXPositions = new float[planetCount];
        
        for(int x = 0; x < planetCount; x++)
        {
            int runs = 0;
            float planetXPosition = 0;
            bool notReady = true;
            int threshold = 100;
            while (notReady)
            {
                if(runs > threshold)
                {
                    planetXPosition = 0;
                    break;
                }
                notReady = false;
                planetXPosition = Random.Range(minPlanetDistance, maxPlanetDistance);
                for (int y = 0; y < planetCount; y++)
                {
                    if(Mathf.Abs(planetXPosition - planetXPositions[y]) < planetSeparation)
                    {
                        notReady = true;
                    }
                }
                runs += 1;
            }
            yield return new WaitForEndOfFrame();
            creationSlider.GetComponentInChildren<TMP_Text>().text = System.Math.Round(1 / Time.deltaTime, 1) + " planets/s";
            creationSlider.mainSlider.value += 1;
            if (randomPe)
            {
                planetXPositions[x] = planetXPosition + Random.Range(-planetXPosition * 0.1f, planetXPosition * 0.1f);
            }
            else
            {
                planetXPositions[x] = planetXPosition;
            }
        }

        // Calculate planet sizes
        planetSizes = new float[planetCount];
        for(int i = 0; i < planetCount; i++)
        {
            planetSizes[i] = Random.Range(planetMinSize, planetMaxSizeOfStar * starSize);
        }

        // Calculate planet mass
        planetMass = new float[planetCount];
        planetTypeIDs = new int[planetCount];
        for (int i = 0; i < planetCount; i++)
        {
            int planetTypeID = Random.Range(0, planetTypes.Length - 1);
            planetTypeIDs[i] = planetTypeID;
            planetMass[i] = planetSizes[i] * planetTypes[planetTypeID].planetTypeDensity;
        }

        // Instantiate planets
        planets = new GameObject[planetCount];
        for(int i = 0; i < planetCount; i++)
        {
            if(planetXPositions[i] != 0)
            {
                GameObject planet = Instantiate(planetPrefab, new Vector3(-planetXPositions[i], 0, 0), Quaternion.identity, transform);
                planet.GetComponent<SpriteRenderer>().color = planetTypes[planetTypeIDs[i]].planetTypeColor;
                planet.gameObject.name = planetTypes[planetTypeIDs[i]].planetTypeName;
                planet.transform.localScale = new Vector3(planetSizes[i], planetSizes[i], planetSizes[i]);
                Body planetBody = planet.GetComponent<Body>();
                planetBody.mass = planetMass[i];
                planetBody.useRandomPe = randomPe;
                planet.GetComponent<DrawOrbit>().timeStep = 0.1f * -planet.transform.localPosition.x / 10;
                planet.GetComponent<DrawOrbit>().iterations = Mathf.RoundToInt(2*Mathf.PI*Mathf.Sqrt((Mathf.Pow(Mathf.Abs(planetXPositions[i]),3))/(Universe.gravitationalConstant*planet.GetComponentsInParent<Body>()[1].mass)) * 1 / planet.GetComponent<DrawOrbit>().timeStep);
                planets[i] = planet;
            }
        }
        minPlanetDistance -= starSize;
        Universe.Print("Tried to spawn " + planets.Length + " planets", gameObject);
        creationSlider.maxValue = planets.Length;
        creationSlider.mainSlider.value = planets.Length;
        timeTaken += Time.realtimeSinceStartup - start;
        StartCoroutine("SpawnMoons");
        yield break;
    }

    IEnumerator SpawnMoons()
    {
        float start = Time.realtimeSinceStartup;
        try
        {
            foreach (GameObject moon in moons)
            {
                Destroy(moon.GetComponent<DrawOrbit>().line);
                Destroy(moon);
            }
        } catch { }

        int moonCount = Random.Range(minMoons, maxMoons);
        creationSlider.maxValue += moonCount;
        int[] moonLocations = new int[moonCount];

        for(int i = 0; i < moonCount; i++)
        {
            moonLocations[i] = Random.Range(0, planets.Length-1);
        }

        float[] moonDistances = new float[moonCount];
        for(int i = 0; i < moonCount; i++)
        {
            int runs = 0;
            float moonXPosition = 0;
            bool notReady = true;
            int threshold = 100;
            while (notReady)
            {
                if (runs > threshold)
                {
                    moonXPosition = 0;
                    break;
                }
                notReady = false;
                moonXPosition = Random.Range(minMoonDistance + planetSizes[moonLocations[i]], maxMoonDistance);
                for (int y = 0; y < moonCount; y++)
                {
                    if (Mathf.Abs(moonXPosition - moonDistances[y]) < moonSeparationSizeOfPlanet * planetSizes[moonLocations[i]])
                    {
                        moonXPosition = 0;
                        notReady = true;
                    }
                }
                runs += 1;
            }
            yield return new WaitForEndOfFrame();
            creationSlider.mainSlider.value += 1;
            creationSlider.GetComponentInChildren<TMP_Text>().text = System.Math.Round(1 / Time.deltaTime, 1) + " moons/s";
            if (randomPe)
            {
                moonDistances[i] = moonXPosition + Random.Range(-moonXPosition * 0.1f, moonXPosition * 0.1f);
            }
            else
            {
                moonDistances[i] = moonXPosition;
            }
        }

        float[] moonSizes = new float[moonCount];
        for(int i = 0; i < moonCount; i++)
        {
            moonSizes[i] = Random.Range(moonMinSizeOfPlanet * planetSizes[moonLocations[i]], moonMaxSizeOfPlanet * planetSizes[moonLocations[i]]);
        }

        float[] moonMass = new float[moonCount];
        int[] moonTypeIDs = new int[moonCount];
        for (int i = 0; i < moonCount; i++)
        {
            moonTypeIDs[i] = Random.Range(0, moonTypes.Length);
            moonMass[i] = moonSizes[i] * moonTypes[moonTypeIDs[i]].planetTypeDensity;
        }
        moons = new GameObject[moonCount];
        for (int i = 0; i < moonCount; i++)
        {
            if(moonDistances[i] != 0)
            {
                try
                {
                    GameObject moon = Instantiate(moonPrefab, planets[moonLocations[i]].transform.position + new Vector3(-moonDistances[i], 0, 0), Quaternion.identity, planets[moonLocations[i]].transform);
                    if (moon.transform.position == new Vector3(0, 0, 0))
                    {
                        Destroy(moon);
                    }
                    else
                    {
                        moons[i] = moon;
                    }
                    moon.GetComponent<SpriteRenderer>().color = moonTypes[moonTypeIDs[i]].planetTypeColor;
                    moon.gameObject.name = moonTypes[moonTypeIDs[i]].planetTypeName;
                    moon.transform.localScale = new Vector3(moonSizes[i] / moon.transform.lossyScale.x, moonSizes[i] / moon.transform.lossyScale.y, 1);
                    Body moonBody = moon.GetComponent<Body>();
                    moonBody.mass = moonMass[i];
                    moonBody.isMoon = true;
                    moonBody.useRandomPe = randomPe;
                    moonBody.mainPlanet = planets[moonLocations[i]].GetComponent<Body>();
                    moon.GetComponent<DrawOrbit>().timeStep = Mathf.Clamp(0.1f * -moon.transform.localPosition.x / 10, 0.1f, 10f);
                    moon.GetComponent<DrawOrbit>().iterations = Mathf.RoundToInt(2 * Mathf.PI * Mathf.Sqrt((Mathf.Pow(Mathf.Abs(moonDistances[i]), 3)) / (Universe.gravitationalConstant * moon.GetComponentsInParent<Body>()[1].mass)) * 1 / moon.GetComponent<DrawOrbit>().timeStep);
                }
                catch
                {
                }
            }
        }
        Universe.Print("Tried to spawn " + GameObject.FindGameObjectsWithTag("Moon").Length + " moons", gameObject);
        timeTaken += Time.realtimeSinceStartup - start;
        isCreatingSystem = false;
        creationSlider.gameObject.SetActive(false);
        Universe.Print("System created in <color=green> " + System.Math.Round(timeTaken, 1) + " </color>seconds", gameObject);
        FindObjectOfType<nBodySimulation>().UpdateBodies();
        var bodies = FindObjectsOfType<BodyInformation>();
        foreach (BodyInformation body in bodies)
        {
            if (body.isStar)
            {
                body.CalculateMassForStar();
            }
        }
        yield break;
    }
    

    // Functions for settings menu
    public void ChangeMinPlanets(string amount)
    {
        minPlanets = int.Parse(amount);
    }
    public void ChangeMaxPlanets(string amount)
    {
        maxPlanets = int.Parse(amount);
    }
    public void ChangeMinDistance(string amount)
    {
        minPlanetDistance = float.Parse(amount);
    }
    public void ChangeMaxDistance(string amount)
    {
        maxPlanetDistance = float.Parse(amount);
    }
    public void ChangeMaxPlanetSize(float amount)
    {
        planetMaxSizeOfStar = amount;
    }
    public void ChangeMinPlanetSize(string amount)
    {
        planetMinSize = float.Parse(amount);
    }
    public void ChangeMinPlanetSeparation(string amount)
    {
        planetSeparation = float.Parse(amount);
    }
    public void ChangeMinMoons(string amount)
    {
        minMoons = int.Parse(amount);
    }
    public void ChangeMaxMoons(string amount)
    {
        maxMoons = int.Parse(amount);
    }
    public void ChangeMinMoonDistance(string amount)
    {
        minMoonDistance = float.Parse(amount);
    }
    public void ChangeMoonSeparation(string amount)
    {
        moonSeparationSizeOfPlanet = float.Parse(amount);
    }
    public void ChangeMoonMaxSize(float amount)
    {
        moonMaxSizeOfPlanet = amount;
    }
    public void ChangeMoonMinSize(float amount)
    {
        moonMinSizeOfPlanet = amount;
    }
    public void ChangeStarMaxSize(string amount)
    {
        starMaxSize = float.Parse(amount);
    }
    public void ChangeStarMinSize(string amount)
    {
        starMinSize = float.Parse(amount);
    }
}