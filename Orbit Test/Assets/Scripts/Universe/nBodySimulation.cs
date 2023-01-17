using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
public class nBodySimulation : MonoBehaviour
{
    // Thanks Sebastian Lague
    // https://youtu.be/7axImc1sxa0

    public Body[] stars;
    public Body[] planets;
    public Body[] moons;
    public SpaceShip[] ships;
    public Body[] allBodies = new Body[0];

    public float PhysicsTime;
    public float OrbitsTime;
    public float OrbitUpdatesPerFrame;

    private Universe Universe;

    private void Start()
    {
        Universe = FindObjectOfType<Universe>();

        StartCoroutine("UpdateOrbits");

        UpdateBodies();

        Time.fixedDeltaTime = Universe.physicsTimeStep;
    }

    public void UpdateBodies()
    {
        Universe = FindObjectOfType<Universe>();
        Universe.Print("Updated object lists", gameObject);
        GameObject[] planetGameObjects = GameObject.FindGameObjectsWithTag("Planet");
        planets = new Body[planetGameObjects.Length];
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i] = planetGameObjects[i].GetComponent<Body>();
        }

        GameObject[] starGameObjects = GameObject.FindGameObjectsWithTag("Star");
        stars = new Body[starGameObjects.Length];
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i] = starGameObjects[i].GetComponent<Body>();
        }

        GameObject[] moonGameObjects = GameObject.FindGameObjectsWithTag("Moon");
        moons = new Body[moonGameObjects.Length];
        for (int i = 0; i < moons.Length; i++)
        {
            moons[i] = moonGameObjects[i].GetComponent<Body>();
        }

        GameObject[] shipGameObjects = GameObject.FindGameObjectsWithTag("SpaceShip");
        ships = new SpaceShip[shipGameObjects.Length];
        for (int i = 0; i < ships.Length; i++)
        {
            ships[i] = shipGameObjects[i].GetComponent<SpaceShip>();
        }

        foreach (GameObject line in GameObject.FindGameObjectsWithTag("Line")){
            Destroy(line);
        }

        // Ugly way to update the allbodies array
        allBodies = new Body[stars.Length + planets.Length + moons.Length];
        for (int i = 0; i < stars.Length; i++)
        {
            allBodies[i] = stars[i];
        }
        for (int i = 0; i < planets.Length; i++)
        {
            allBodies[i + stars.Length] = planets[i];
        }
        for (int i = 0; i < moons.Length; i++)
        {
            allBodies[i + stars.Length + planets.Length] = moons[i];
        }
        
        //foreach (DrawOrbit draw in FindObjectsOfType<DrawOrbit>())
        //{
        //    draw.Simulate();
        //}

    }
    
    IEnumerator UpdateOrbits()
    {
        yield return new WaitUntil(()=> allBodies.Length >= 1);
        
        int orbits = 0;
        for (int i = 0; i < allBodies.Length; i++)
        {
            if (orbits >= OrbitUpdatesPerFrame)
            {
                yield return new WaitForEndOfFrame();
                orbits = 0;
            }
            if (allBodies[i] != null)
            {              
                orbits++;
                if (allBodies[i].GetComponent<DrawOrbit>())
                {
                    allBodies[i].GetComponent<DrawOrbit>().UpdateOrbits();
                    if (!allBodies[i].GetComponent<DrawOrbit>().isInCameraView)
                    {
                        orbits -= 1;
                    }
                }
            }
        }

        StartCoroutine("UpdateOrbits");
        yield break;
    }

    private void FixedUpdate()
    {
        
        if (!Universe.useNBodyPhysics)
        {
            try
            {
                // Update planets
                foreach (var planet in planets)
                {
                    if(planet.gameObject.transform.position == new Vector3(0, 0, 0))
                    {
                        Destroy(planet.gameObject);
                    }
                    planet.UpdateVelocity(Universe.timeStep, stars);
                }
                foreach (var planet in planets)
                {
                    planet.UpdatePosition(Universe.timeStep);
                }

                // Update moons
                foreach (var moon in moons)
                {
                    moon.UpdateVelocityMoon(Universe.timeStep);
                }
                foreach (var moon in moons)
                {
                    moon.UpdatePosition(Universe.timeStep);
                }

                // Update ships
                foreach (var ship in ships)
                {
                    ship.UpdateVelocity(Universe.timeStep, stars);
                }
                foreach (var ship in ships)
                {
                    ship.UpdatePosition(Universe.timeStep);
                }
            }
            catch 
            {
                UpdateBodies();
            }
        }
        else
        {
            try
            {
                float startTime = Time.realtimeSinceStartup;
                // Everything at once
                for (int i = 0; i < allBodies.Length; i++)
                {
                    allBodies[i].UpdateVelocity(Universe.timeStep, bodies: allBodies);
                }
                for (int i = 0; i < allBodies.Length; i++)
                {
                    allBodies[i].UpdatePosition(Universe.timeStep);
                }
            }
            catch
            {
                UpdateBodies();
            }
        }
    }

    public void ChangeOrbitUpdatesPerFrame(float amount)
    {
        OrbitUpdatesPerFrame = amount;
    }

}
