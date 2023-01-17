using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyInformation : MonoBehaviour
{
    [Header("Related to the body")]
    public float distanceFromObject;
    public float radius;
    public float mass;
    public float amountOfMoons;
    public bool isMoon;
    public Body body;
    public float speedOnOrbit;
    public float deltaVforOrbit;
    public float ap;
    public float pe;
    [Header("Use only for the main star")]
    public bool isStar;

    private Universe Universe;
    private void Start()
    {
        body = GetComponent<Body>();
        isMoon = body.isMoon;
        radius = transform.lossyScale.x;
        mass = body.mass;
        Universe = FindObjectOfType<Universe>();
        deltaVforOrbit = mass * Mathf.Pow(radius,2) * 9.4f;
    }
    public void CalculateMassForStar()
    {
        mass = 0;
        var bodies = FindObjectsOfType<Body>();
        foreach (Body b in bodies)
        {
            mass += b.mass;
        }
        Debug.Log(mass);
    }
    private void Update()
    {
        try
        {
            ap = GetComponent<DrawOrbit>().ap;
            pe = GetComponent<DrawOrbit>().pe;
        }
        catch { }

        try
        {
            distanceFromObject = Vector3.Distance(transform.position, GetComponentsInParent<Body>()[1].gameObject.transform.position);
            speedOnOrbit = Mathf.Sqrt(Universe.gravitationalConstant * GetComponentsInParent<Body>()[1].mass * (2 / distanceFromObject - 1 / distanceFromObject));
        } catch { }
        
        amountOfMoons = GetComponentsInChildren<Body>().Length-1;
    }
}
