using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateHohmanTransfer : MonoBehaviour
{

    public BodyInformation from;
    public BodyInformation to;

    // Thanks Ai-Solutions
    // https://ai-solutions.com/_freeflyeruniversityguide/hohmann_transfer.htm
    public float HohmanTransfer(BodyInformation otherBodyInfo, BodyInformation thisBodyInfo)
    {
        Universe Universe = FindObjectOfType<Universe>();
        Universe.Print(otherBodyInfo.name + " and " + thisBodyInfo.name, gameObject);
        float speed = thisBodyInfo.speedOnOrbit;

        float requiredTransferSpeed;
        float thisBodyOrbit = thisBodyInfo.distanceFromObject;
        float otherBodyOrbit = otherBodyInfo.distanceFromObject;

        float u = Universe.gravitationalConstant * thisBodyInfo.gameObject.GetComponentsInParent<Body>()[1].mass;

        float targetSemiMajorAxis = (thisBodyOrbit + otherBodyOrbit) / 2;
        float periapsisVelocity;
        if (thisBodyOrbit > targetSemiMajorAxis)
        {
            periapsisVelocity = Mathf.Sqrt((u) * (2 / thisBodyOrbit - 1 / targetSemiMajorAxis));
        }
        else
        {
            periapsisVelocity = Mathf.Sqrt((u) * (2 / thisBodyOrbit - 1 / targetSemiMajorAxis));
        }

        requiredTransferSpeed = speed + (periapsisVelocity - speed);
        Universe.Print("Required velocity : " + requiredTransferSpeed.ToString(), gameObject);
        return requiredTransferSpeed;

        // TODO : Add transfertime and start the burn at the right time

        //float transferTime = Mathf.PI * Mathf.Sqrt(Mathf.Pow(thisBodyOrbit + otherBodyOrbit, 3) / 8 * u) * 2;
        //Debug.Log("Transfer time : " + transferTime.ToString());
        //float targetAngularVelocity = Mathf.Sqrt(u / Mathf.Pow(otherBodyOrbit, 3));
        //Debug.Log("Target angular velocity : " + targetAngularVelocity.ToString());
        //float angularAlignment = Mathf.PI - (targetAngularVelocity * transferTime);
        //angularAlignment = Mathf.Rad2Deg * angularAlignment;
        //Debug.Log("Alignment : " + angularAlignment.ToString());

        from = null;
        to = null;

    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition), Vector2.down, float.PositiveInfinity);
        //         
        //    if(hit)
        //    {
        //        if(from == null)
        //        {
        //            from = hit.collider.GetComponent<BodyInformation>();
        //        } else if(to == null)
        //        {
        //            to = hit.collider.GetComponent<BodyInformation>();
        //        }
        //    }
        //
        //}
        //
        //if(from != null && to != null)
        //{
        //    HohmanTransfer(to, from);
        //}
    }

}
