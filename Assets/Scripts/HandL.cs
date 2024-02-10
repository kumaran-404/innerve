using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandL : MonoBehaviour
{
    // Start is called before the first frame update
    // Gameo
     private LineRenderer line;
     private LineRenderer line1;
    public Transform baseFinger;
    public Transform middleFingerEnd;

    public Transform leftPoint;
    public Transform rightPoint;
    public Vector3 InitialScale;
    void Start()
    {
        InitialScale = transform.localScale;
        // UpdateTransformForScale();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //  if(baseFinger.hasChanged || middleFingerEnd.hasChanged){
        //     UpdateTransformForScale();
        //  }
        //position
         line = GetComponent<LineRenderer>();
         Vector3[] pathPoints = { baseFinger.position,  middleFingerEnd.position };
        line .positionCount = 2;
        line .SetPositions(pathPoints);
        transform.position = baseFinger.position;
        Vector3 vertical = middleFingerEnd.position - baseFinger.position;
        // transform.localScale = vertical;

        // transform.position = vertical; wrist up down
        vertical.Normalize();
        vertical.z = 1 * vertical.z;
        // angle1 = Vector3.angle(vertical,transform.forward);
        transform.rotation = Quaternion.LookRotation( vertical, transform.up);
        //Rotation
        // line1 = GetComponent<LineRenderer>();
        //  Vector3[] pathPoints1 = { leftPoint.position,  rightPoint.position };
        // line1 .positionCount = 2;
        // line1 .SetPositions(pathPoints1);
        Vector3 horizontal = leftPoint.position - rightPoint.position;
        horizontal.Normalize();
        float angle = 0f;
        if(gameObject.name == "Hand_R_"){
            // wrist sideways
            angle = Vector3.Angle(horizontal, new Vector3(1,0,0)) * -1;
        }else if(gameObject.name == "Hand_L_"){
            angle = Vector3.Angle(horizontal, transform.up) * -1;
        }else{
            angle = Vector3.Angle(horizontal, transform.up) * -1;
        }
        Debug.Log(angle);
        // transform.LookAt
        
        // transform.eulerAngles = new Vector3(transform.eulerAngles.x+angle,transform.eulerAngles.y,transform.eulerAngles.z);
        // transform.rotation = Quaternion.LookRotation( horizontal, transform.up);
        // Debug.Log(angle);
        
    }

    void UpdateTransformForScale(){
        float distance = Vector3.Distance(baseFinger.position,middleFingerEnd.position);
        transform.localScale = new Vector3(InitialScale.x,distance/2f,InitialScale.z);

        Vector3 middlePoint = (baseFinger.transform.position + middleFingerEnd.transform.position) / 2f;
        transform.position = middlePoint;

        Vector3 rotationDirection = (middleFingerEnd.position - baseFinger.position);
        transform.up = rotationDirection;
    }
}
