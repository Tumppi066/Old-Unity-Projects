using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Body : MonoBehaviour
{

    // Thanks Sebastian Lague
    // https://youtu.be/7axImc1sxa0
   
    public float mass;
    public string planetType;
    public bool useRandomPe;
    [Header("Either manually set the orbital speed or calculate it")]
    public bool calculateOrbitalSpeedAutomatically = true;
    public Body nextBody;
    public Vector3 startVelocity;
    public Vector3 velocity;
    [Header("Settings for moons (not necessary on planets)")]
    public bool isMoon;
    public Body mainPlanet;
    [Header("Setting to toggle spaceship mode.")]
    public bool isSpaceShip;
    [Header("Is set by the script")]
    public Universe Universe;
    public bool isReal = true;


    private float startTime = 0;
    public float msToUpdate = 0;
    public Vector3 CalculateOrbitalVelocity()
    {
        try { 
            nextBody = GetComponentsInParent<Body>()[1]; 
            Universe = FindObjectOfType<Universe>();
            if (!useRandomPe)
            {
                return new Vector3(0, Mathf.Sqrt(Universe.gravitationalConstant * nextBody.mass / (-transform.position.x + nextBody.gameObject.transform.position.x)), 0);
            }
            else
            {
                Vector3 vel = new Vector3(0, Mathf.Sqrt(Universe.gravitationalConstant * nextBody.mass / (-transform.position.x + nextBody.gameObject.transform.position.x)), 0);
                vel += vel * Random.Range(-0.1f, 0.1f);
                return vel;
            }
        } catch { return new Vector3(0,0,0);  }
    }

    private void Start()
    {
        try { nextBody = GetComponentsInParent<Body>()[1]; } catch { }
        Universe = FindObjectOfType<Universe>();
        if (calculateOrbitalSpeedAutomatically)
        {
            startVelocity = CalculateOrbitalVelocity();
        }
        velocity = startVelocity;
        if(transform.position == new Vector3(-1, 0, 0))
        {
            Destroy(gameObject);
        }
        //if (isMoon)
        //{
        //    mainPlanet = GetComponentInParent<Body>();
        //}
    }   

    public void UpdateVelocity(float timeStep, Body[] bodies = null,  Body singleBody = null)
    {
        startTime = Time.realtimeSinceStartup;
        if(bodies == null)
        {
            Body body = singleBody;
            if (body != this && body != null)
            {
                float dist = (body.transform.position - transform.position).sqrMagnitude;
                Vector3 dir = (body.transform.position - transform.position).normalized;
                Vector3 force = dir * Universe.gravitationalConstant * mass * body.mass / dist;
                Vector3 acc = force / mass;
                velocity += acc * timeStep;
            }
        }
        else
        {
            foreach (var body in bodies)
            {
                Universe = FindObjectOfType<Universe>();
                if (body != this && body != null)
                {
                    float dist = (body.transform.position - transform.position).sqrMagnitude;
                    Vector3 dir = (body.transform.position - transform.position).normalized;
                    Vector3 force = dir * Universe.gravitationalConstant * mass * body.mass / dist;
                    Vector3 acc = force / mass;
                    velocity += acc * timeStep;
                }
            }
        }
    }

    public void UpdateRelativeVelocity(float newVel)
    {
        Vector3 dir = (GetComponentsInParent<Body>()[1].gameObject.transform.position - transform.position).normalized;
        Debug.Log(new Vector3(dir.y, dir.x, 0));
        Vector3 force = new Vector3(dir.y,-dir.x,0) * newVel;
        velocity = force;
    }

    public void UpdatePosition(float timeStep)
    {
        transform.position += velocity * timeStep;
        msToUpdate = Time.realtimeSinceStartup - startTime;
        msToUpdate *= 1000;
    }

    public void UpdateVelocityMoon(float timeStep)
    {

        startTime = Time.realtimeSinceStartup;
        float dist = (mainPlanet.transform.position - transform.position).sqrMagnitude;
        Vector3 dir = (mainPlanet.transform.position - transform.position).normalized;
        Vector3 force = dir * Universe.gravitationalConstant * mass * mainPlanet.mass / dist;
        Vector3 acc = force / mass;
        velocity += acc * timeStep;

    }
    
    
    private void OnMouseDown()
    {
        if (!isSpaceShip)
        {
            CameraControl camera = FindObjectOfType<CameraControl>();
            PlanetManagement manage = FindObjectOfType<PlanetManagement>();
            if(camera.currentlyFocusedObject != gameObject)
            {
                camera.currentlyFocusedObject = gameObject;
                manage.targetedBody = gameObject;
            }
            else 
            {
                manage.targetedBody = null;
                camera.currentlyFocusedObject = null;
            }
        }
        else
        {
            CameraControl camera = FindObjectOfType<CameraControl>();
            if (camera.currentlyFocusedObject != gameObject)
            {
                camera.currentlyFocusedObject = gameObject;
            }
            else
            {
                camera.currentlyFocusedObject = null;
            }
        }
    }

    private void OnMouseOver()
    {
        if (!isSpaceShip)
        {
            FindObjectOfType<BodyInformationDisplay>().targetedBody = gameObject;
            FindObjectOfType<OrbitInformationDisplay>().targetedBody = gameObject;
        }
    }


}
