using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    public handTracking handTrackingData;
    public PlayerScript playerScript;
    public Transform handPickUpPoint;
    private bool dropped;
    private Transform player;
    private Transform cameraTransform;
    public Collider otherobject;
    public Animator m_Animator;
    // public Transform EmptyScene;
    void Start()
    {
        handTrackingData  = FindObjectOfType<handTracking>();
        playerScript = FindObjectOfType<PlayerScript>();
        playerScript.moveForward = true;
        dropped = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(handTrackingData.isRightPick);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="pickable" && handTrackingData.isRightPick){

            Debug.Log("pickkkk");
            // GameObject g = other.gameObject;
            otherobject = other;
            other.transform.parent = handPickUpPoint;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }else if(other.gameObject.tag=="movable" && handTrackingData.isRightPick){
             otherobject = other;
            other.transform.parent = handPickUpPoint;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag=="pickable" && handTrackingData.isRightPick){
            Debug.Log("pickkkk");
            // GameObject g = other.gameObject;
            otherobject = other;
            other.transform.parent = handPickUpPoint;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }else if(other.gameObject.tag=="movable" && handTrackingData.isRightPick){
             otherobject = other;
            other.transform.parent = handPickUpPoint;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void FixedUpdate() {
            if(handTrackingData.isRightStanding){
                if(otherobject.gameObject.tag=="pickable"){
                    otherobject.gameObject.GetComponent<Rigidbody>().useGravity = true;
                }else{
                    otherobject.gameObject.GetComponent<Rigidbody>().useGravity = false;
                }
                otherobject.transform.parent = null;
                dropped = true;
                playerScript.moveForward = false;
                // yield return new WaitForSeconds(1);
            }
            else if(handTrackingData.isRightShoot){
                Debug.Log("right shoot");
                playerScript.moveForward = true;
            }else if(!handTrackingData.isRightShoot){
                playerScript.moveForward = false;
                
            }else{

            }
    }

    // private IEnumerator OnTriggerStay(Collider other)
    // {
    //     if (other.gameObject.tag=="pickable" && !handTrackingData.isRightPick)
    //     {
    //         yield return new WaitForSeconds(1);
    //         other.transform.parent = null;
    //         yield return new WaitForSeconds(1);
    //         other.gameObject.GetComponent<Rigidbody>().useGravity = true;
    //         dropped = true;
    //     }else if(other.gameObject.tag=="pickable" && handTrackingData.isRightPick){
    //         Debug.Log("pickkkk++++++++++");
    //         other.gameObject.GetComponent<Rigidbody>().useGravity = false;
    //         Debug.Log(handTrackingData.isRightPick);
    //         // GameObject g = other.gameObject;
    //         other.transform.parent = handPickUpPoint;
    //         // dropped = false;
    //     }
    // }



}
