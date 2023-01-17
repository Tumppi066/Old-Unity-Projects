using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawOrbit : MonoBehaviour
{
    public float timeStep = 1f;
    public bool enabled = false;
    public int iterations = 1000;
    public LineRenderer line;
    Vector3[] points;
    public Vector2 locationOnScreen;
    public Body thisBody;
    private Universe Universe;
    public GameObject prefab;
    public GameObject linePrefab;
    public Body referenceObject;
    public float lineThickness;
    public bool isInCameraView;
    public float msToUpdate;

    // While drawing the planet's orbit it is also easy to get the approximate ap and pe.
    public float ap;
    public float pe;

    private Vector3 lastKnownPosition = new Vector3(0,0,0);

    private void Start()
    {
        thisBody = GetComponent<Body>();
        Universe = FindObjectOfType<Universe>();
        pe = GameObject.FindObjectOfType<StarSystemSpawner>().maxPlanetDistance + 1;
    }

    public void Simulate()
    {
        float start = Time.realtimeSinceStartup;
        locationOnScreen = Camera.main.WorldToViewportPoint(transform.position);
        Rect display = new Rect(-0.1f, -0.1f, 1.2f, 1.2f);
        if (!display.Contains(locationOnScreen))
        {
            msToUpdate = 0;
            DrawLines();
            isInCameraView = false;
        }
        else
        {
            isInCameraView = true;
        }

        if (isInCameraView)
        {
            referenceObject = GetComponentsInParent<Body>()[1];
            try
            {
                points = new Vector3[iterations];
            }
            catch
            {
                //Debug.Log("<color=yellow>DrawOrbit - " + thisBody.name + "</color> mismatch in values.");
            }
                GameObject planet = Instantiate(prefab, transform);
            Body virtualPlanet = planet.GetComponent<Body>();
            virtualPlanet.tag = "VirtualPlanet";
            virtualPlanet.gameObject.transform.position = gameObject.transform.position;
            virtualPlanet.mass = thisBody.mass;
            virtualPlanet.calculateOrbitalSpeedAutomatically = thisBody.calculateOrbitalSpeedAutomatically;
            virtualPlanet.isReal = false;
            if (!Application.isPlaying)
            {
                virtualPlanet.velocity = thisBody.startVelocity;
            }
            else
            {
                virtualPlanet.velocity = thisBody.velocity;
            }
            virtualPlanet.Universe = Universe;
            if (thisBody.isMoon)
            {
                virtualPlanet.isMoon = thisBody.isMoon;
                virtualPlanet.mainPlanet = thisBody.mainPlanet;
            }

            for(int i = 0; i < iterations; i++)
            {
                if (virtualPlanet.isMoon)
                {
                    virtualPlanet.UpdateVelocityMoon(timeStep);
                }
                else
                {
                    virtualPlanet.UpdateVelocity(timeStep, null ,referenceObject);
                }
                virtualPlanet.UpdatePosition(timeStep);
                points[i] = virtualPlanet.gameObject.transform.position;
                // Here we calculate the ap and pe.
                if (Vector2.Distance(virtualPlanet.gameObject.transform.position, referenceObject.gameObject.transform.position) < pe)
                {
                    pe = Vector2.Distance(virtualPlanet.gameObject.transform.position, referenceObject.gameObject.transform.position);
                }
                if (Vector2.Distance(virtualPlanet.gameObject.transform.position, referenceObject.gameObject.transform.position) > ap)
                {
                    ap = Vector2.Distance(virtualPlanet.gameObject.transform.position, referenceObject.gameObject.transform.position);
                }
            }
            DestroyImmediate(planet);
            DrawLines();
        }
        msToUpdate = Time.realtimeSinceStartup - start;
        msToUpdate *= 1000;
    }

    public void DrawLines()
    {
        // VERY, experimental code to update iterations and timestep for drawing orbit.
        if (isInCameraView)
        {
            timeStep = Mathf.Clamp(0.1f * GetComponent<BodyInformation>().ap / 10, 0.1f, 10f);
            iterations = Mathf.RoundToInt(2 * Mathf.PI * Mathf.Sqrt((Mathf.Pow(Mathf.Abs(GetComponent<BodyInformation>().ap), 3)) / (Universe.gravitationalConstant * GetComponentsInParent<Body>()[1].mass)) * 1 / timeStep);
            if(iterations > 3000)
            {
                iterations = 3000;
            }
            if(iterations < 0)
            {
                //Debug.Log("<color=yellow>DrawOrbit - " + thisBody.name + "</color> error with determining orbit lenght.");
            }
            else
            {
                line.positionCount = iterations;
                line.widthMultiplier = lineThickness;
                line.enabled = true;
                line.SetColors(thisBody.GetComponent<SpriteRenderer>().color * 1f, thisBody.GetComponent<SpriteRenderer>().color);
                line.SetPositions(new Vector3[iterations]);
                line.SetPositions(points);
            }
        }
        else if (GetComponent<Body>().isMoon)
        {
            line.enabled = false;
        }

    }

    public void UpdateOrbits()
    {
        if (line == null && gameObject.tag != "Star")
        {
            line = Instantiate(linePrefab).GetComponent<LineRenderer>();
        }
        Simulate();
    }

    private void Update()
    {
        msToUpdate = 0;
    }

}
