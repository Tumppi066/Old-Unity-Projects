using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpaceShip : Body
{
    [Range(0,1)]
    public float thrusterPower = 0.1f;
    public float thrusterRotation = 0.1f;

    public void AddForce(Vector3 force)
    {
        GetComponent<Body>().velocity += force;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            AddForce(transform.up * thrusterPower);
        }
        if (Input.GetKey(KeyCode.S))
        {
            AddForce(-transform.up * thrusterPower);
        }
        if (Input.GetKey(KeyCode.A))
        {
            AddForce(-transform.right * thrusterPower);
        }
        if (Input.GetKey(KeyCode.D))
        {
            AddForce(transform.right * thrusterPower);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.DORotate(new Vector3(0, 0, -1 * thrusterRotation), 0.1f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.DORotate(new Vector3(0, 0, 1 * thrusterRotation), 0.1f);
        }
    }
}
