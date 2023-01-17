using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyEconomy : MonoBehaviour
{
    public float deltaV;
    public float deltaVRegen = 0.5f;

    public GameObject spaceShipPrefab;
    private Universe Universe;
    private void Start()
    {
        Universe = FindObjectOfType<Universe>();
    }

    public void SendSpaceShip(GameObject to)
    {
        float transferSpeed = FindObjectOfType<CalculateHohmanTransfer>().HohmanTransfer(GetComponent<BodyInformation>(), to.GetComponent<BodyInformation>());
        GameObject ship = Instantiate(spaceShipPrefab, GetComponentsInParent<Body>()[1].gameObject.transform);
        SpaceShip shipInfo = ship.GetComponent<SpaceShip>();
        shipInfo.startVelocity = GetComponent<Body>().velocity;
        shipInfo.UpdateRelativeVelocity(transferSpeed);
    }

    private void FixedUpdate()
    {
        deltaV += deltaVRegen * Universe.timeStep;
    }
}
