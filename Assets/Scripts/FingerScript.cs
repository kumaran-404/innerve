using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerScript : MonoBehaviour
{
    public Transform basePoint;
     private LineRenderer line;
    public Transform endPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         line = GetComponent<LineRenderer>();
         Vector3[] pathPoints = { basePoint.position,  endPoint.position };
        line .positionCount = 2;
        line .SetPositions(pathPoints);
        Vector3 vertical = endPoint.position - basePoint.position;
        transform.position = basePoint.position;
        // vertical.Normalize();
        // vertical.z = -1 * vertical.z;
        // transform.rotation = Quaternion.LookRotation( vertical, Vector3.left);
    }
}
