using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetIconContainer : MonoBehaviour
{
    public bool isGroupLeader = false;
    public int groupedBodyCount;
    public GameObject[] groupedBodies = new GameObject[100];
    public Vector3 groupCenter;

    void Update()
    {
        if(isGroupLeader)
        {
            try
            {
                groupCenter = Vector3.zero;
                for(int i = 0; i < groupedBodyCount; i++)
                {
                    groupCenter += groupedBodies[i].transform.position;
                    Debug.Log("groupCenter: " + groupCenter);
                    Debug.Log("groupedBodies[i].transform.position: " + groupedBodies[i].transform.position);
                }
                groupCenter /= groupedBodyCount;
                transform.position = Camera.main.WorldToViewportPoint(groupCenter);
            }
            catch
            {

            }
        }
    }

}
