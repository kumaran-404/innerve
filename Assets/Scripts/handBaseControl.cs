using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handBaseControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform basePoint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         transform.position = basePoint.position;
    }
}
