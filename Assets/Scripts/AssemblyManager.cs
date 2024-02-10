using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyManager : MonoBehaviour
{
    public Transform wheelloc;
    public GameObject wheell;

    public Transform susloc;
    public Transform batloc;
    public GameObject batl;
    public Transform mudguardloc;
    public GameObject mudguardl;
    public test test;
    public int i = 0;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name=="wheel"){
            i+=1;
            other.gameObject.tag = "immovable";
            other.transform.parent = null;
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            other.transform.position = wheelloc.position;
            wheell.SetActive(false);
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            other.transform.eulerAngles = new Vector3(90,0,0);
            test.Speak("Tyre has been placed, place the wheel perpendicular to the axle");
            
        }else if(other.gameObject.name=="Battery"){
 other.transform.position = batloc.position;
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            other.transform.parent = null;
            rigidbody.isKinematic = true;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            batl.SetActive(false);
            other.gameObject.tag = "immovable";
            i+=1;
            test.Speak("100 watt Battery has been placed");
        }else if(other.gameObject.name=="tinker"){
            other.transform.parent = null;
             other.transform.position = susloc.position;
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            other.gameObject.tag = "immovable";
            i+=1;
           
            test.Speak("Suspension has been placed");
        }else if(other.gameObject.name=="mudguard"){
 other.transform.position = mudguardloc.position;
            other.transform.parent = null;
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            other.gameObject.GetComponent<Rigidbody>().useGravity = false;
            other.gameObject.tag = "immovable";
            i+=1;
            test.Speak("Mud guard has been placed");
        }else{

        }

        if(i>=4){
             test.Speak("Training completed all objects placed");
        }

        if(i>=2){
             test.Speak("Keep going assemble the rest of the parts to complete the build");

        }
    }
}
