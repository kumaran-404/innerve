using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;



public class handTracking : MonoBehaviour
{
    public PlayerScript MyPlayer;
    public GameObject righthand;
    public Animator m_Animator;
    [System.Serializable]
    public class Coor
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class finger
    {

        public Coor WRIST;
        public Coor THUMB_CMC;
        public Coor THUMB_MCP;
        public Coor THUMB_IP;
        public Coor THUMB_TIP;
        public Coor INDEX_FINGER_MCP;
        public Coor INDEX_FINGER_PIP;
        public Coor INDEX_FINGER_DIP;
        public Coor INDEX_FINGER_TIP;
        public Coor MIDDLE_FINGER_MCP;
        public Coor MIDDLE_FINGER_PIP;
        public Coor MIDDLE_FINGER_DIP;
        public Coor MIDDLE_FINGER_TIP;
        public Coor RING_FINGER_MCP;
        public Coor RING_FINGER_PIP;
        public Coor RING_FINGER_DIP;
        public Coor RING_FINGER_TIP;
        public Coor PINKY_MCP;
        public Coor PINKY_PIP;
        public Coor PINKY_DIP;
        public Coor PINKY_TIP;

    }

    [System.Serializable]
    public class MyData
    {
        public bool LeftHandExists;
        public finger Left;
        public bool RightHandExists;
        public finger Right;
        public bool RightThumbsUp;
        public bool RightThumbsDow;
        public bool RightPinch;
        public bool RightShoot;
        public bool RightShootClic;
        public bool RightStanding;
        public bool RightPeace;
        public bool RightThreese;
        public bool RightYo;
        public bool LeftThumbsUp;
        public bool LeftThumbsDown;
        public bool LeftPinch;
        public bool LeftShoot;
        public bool LeftShootClick;
        public bool LeftStanding;
        public bool LeftPeace;
        public bool LeftThreese;
        public bool LeftYo;
        public bool Nothing;
    }

    public int udpPort = 5052;
    private UdpClient udpClient;
    public MyData data;
    public MyData lastKnownData;

    // Start is called before the first frame update
    //public temp Data;
    //public MyData myDataInstance;
    public GameObject[] handPoints;

    private Coor[] leftCoorArray;
    private Coor[] rightCoorArray;
    private Coor[] lastKnownLeftCoorArray;
    private Coor[] lastKnownRightCoorArray;
    private Coor[] coorArray;
    public bool isRightPick;
    public bool isRightStanding;
    public bool isRightShoot;
    public bool isLeftPick;
    

    void Start()
    {
        udpClient = new UdpClient(udpPort);
        udpClient.BeginReceive(ReceiveData, null);
        m_Animator = righthand.GetComponent<Animator>();
         m_Animator.SetBool("isgrab", false);
         m_Animator.SetBool("isshoot", false);
    }

    private void ReceiveData(IAsyncResult result)
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, udpPort);
            byte[] receivedBytes = udpClient.EndReceive(result, ref endPoint);
            string receivedData = Encoding.ASCII.GetString(receivedBytes);

            data = JsonUtility.FromJson<MyData>(receivedData);

            // Debug.Log(data.);
            // Console.WriteLine(receivedData.ToString());
            //Console.WriteLine(d.ToString());
            //Console.WriteLine(d.LeftHandExists);

            // Continue listening for more data
            udpClient.BeginReceive(ReceiveData, null);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error receiving UDP data: " + e.Message);
        }
    }

    void FixedUpdate()
    {
        //temp myDataInstance = GetComponent<temp>();
        if (data != null)
        {
           
            if (data.LeftHandExists && data.Left != null)
            {
                lastKnownLeftCoorArray = getLeftCoords(data.Left);
                AssignCoorToLeftHandPoints(handPoints,getLeftCoords(data.Left));
                // MyPlayer.moveForward = true;
            }else{
                AssignCoorToLeftHandPoints(handPoints,lastKnownLeftCoorArray);
                // MyPlayer.moveForward = false;
            }
            //  Debug.Log(data.RightHandExists);
            if (data.RightHandExists && data.Right != null)
            {
                // Debug.Log("HHHHHHHHHHHHH");
                lastKnownRightCoorArray = getRightCoords(data.Right);
                AssignCoorToRightHandPoints(handPoints,getRightCoords(data.Right));
                // MyPlayer.moveBackward = true;
            }else{
                AssignCoorToRightHandPoints(handPoints,lastKnownRightCoorArray);
                // MyPlayer.moveBackward = false;
            }
            isRightPick = data.RightPinch;
            isLeftPick = data.LeftPinch;
            isRightShoot = data.RightShoot;
            isRightStanding = data.RightStanding;
            m_Animator.SetBool("isgrab", isRightPick);
            m_Animator.SetBool("isshoot", isRightShoot);
            // Assuming RightHandExists is true and Right is not null
            //if (data.RightHandExists && data.Right != null)
            //{
                //AssignCoorToHandPoints(handPoints, data.Right);
            //}
        }
    }

    // Function to assign Coor values to handPoints
    // private void AssignCoorToHandPoints(GameObject[] handPoints, finger handDataL, finger handDataR)
    // {
    //     // Define an array to map the Coor properties to the handPoints

    //     Coor[] leftCoorArray = {
    //         handDataL.WRIST,
    //         handDataL.THUMB_CMC,
    //         handDataL.THUMB_MCP,
    //         handDataL.THUMB_IP,
    //         handDataL.THUMB_TIP,
    //         handDataL.INDEX_FINGER_MCP,
    //         handDataL.INDEX_FINGER_PIP,
    //         handDataL.INDEX_FINGER_DIP,
    //         handDataL.INDEX_FINGER_TIP,
    //         handDataL.MIDDLE_FINGER_MCP,
    //         handDataL.MIDDLE_FINGER_PIP,
    //         handDataL.MIDDLE_FINGER_DIP,
    //         handDataL.MIDDLE_FINGER_TIP,
    //         handDataL.RING_FINGER_MCP,
    //         handDataL.RING_FINGER_PIP,
    //         handDataL.RING_FINGER_DIP,
    //         handDataL.RING_FINGER_TIP,
    //         handDataL.PINKY_MCP,
    //         handDataL.PINKY_PIP,
    //         handDataL.PINKY_DIP,
    //         handDataL.PINKY_TIP,
    //     }; 

    //     Coor[] righttCoorArray = {
    //         handDataR.WRIST,
    //         handDataR.THUMB_CMC,
    //         handDataR.THUMB_MCP,
    //         handDataR.THUMB_IP,
    //         handDataR.THUMB_TIP,
    //         handDataR.INDEX_FINGER_MCP,
    //         handDataR.INDEX_FINGER_PIP,
    //         handDataR.INDEX_FINGER_DIP,
    //         handDataR.INDEX_FINGER_TIP,
    //         handDataR.MIDDLE_FINGER_MCP,
    //         handDataR.MIDDLE_FINGER_PIP,
    //         handDataR.MIDDLE_FINGER_DIP,
    //         handDataR.MIDDLE_FINGER_TIP,
    //         handDataR.RING_FINGER_MCP,
    //         handDataR.RING_FINGER_PIP,
    //         handDataR.RING_FINGER_DIP,
    //         handDataR.RING_FINGER_TIP,
    //         handDataR.PINKY_MCP,
    //         handDataR.PINKY_PIP,
    //         handDataR.PINKY_DIP,
    //         handDataR.PINKY_TIP
    //     };

    //     Coor[] coorArray = {
    //         leftCoorArray,
    //         righttCoorArray
    //     };

    //     Coor[] lastKnownLeftCoorArray = {
            
    //     };
    //     Coor[] lastKnownRightCoorArray = {
            
    //     };

    //     for (int i = 0; i < 42; i++)
    //     {
            
    //                 Debug.Log("Handpoints");
    //                 handPoints[i].transform.localPosition = new Vector3(6 - coorArray[i].x * 10, 10 - coorArray[i].y * 10, coorArray[i].z * 10);
         
    //     }
    // }

    private void AssignCoorToLeftHandPoints(GameObject[] handPoints, Coor[] localLeftCoorArray){
        for (int i = 21; i < 42; i++)
        {
            
                    // Debug.Log("Handpoints");
                    handPoints[i].transform.localPosition = new Vector3(6 - localLeftCoorArray[i-21].x * 10, 10 - localLeftCoorArray[i-21].y * 10, localLeftCoorArray[i-21].z * 10);
         
        }
    }

    private void AssignCoorToRightHandPoints(GameObject[] handPoints, Coor[] localRightCoorArray){
        for (int i = 0; i < 21; i++)
        {
                    // Debug.Log("Right Handpoints");
                    handPoints[i].transform.localPosition = new Vector3( (6 + localRightCoorArray[i].x * 10), 10 - localRightCoorArray[i].y * 10, localRightCoorArray[i].z * 10);
         
        }
    }

    private Coor[] getLeftCoords(finger handDataL){
        Coor[] localLeftCoorArray = {
            handDataL.WRIST,
            handDataL.THUMB_CMC,
            handDataL.THUMB_MCP,
            handDataL.THUMB_IP,
            handDataL.THUMB_TIP,
            handDataL.INDEX_FINGER_MCP,
            handDataL.INDEX_FINGER_PIP,
            handDataL.INDEX_FINGER_DIP,
            handDataL.INDEX_FINGER_TIP,
            handDataL.MIDDLE_FINGER_MCP,
            handDataL.MIDDLE_FINGER_PIP,
            handDataL.MIDDLE_FINGER_DIP,
            handDataL.MIDDLE_FINGER_TIP,
            handDataL.RING_FINGER_MCP,
            handDataL.RING_FINGER_PIP,
            handDataL.RING_FINGER_DIP,
            handDataL.RING_FINGER_TIP,
            handDataL.PINKY_MCP,
            handDataL.PINKY_PIP,
            handDataL.PINKY_DIP,
            handDataL.PINKY_TIP,
        }; 
        return localLeftCoorArray;
    }

    private Coor[] getRightCoords(finger handDataR){
        Coor[] localRightCoorArray = {
            handDataR.WRIST,
            handDataR.THUMB_CMC,
            handDataR.THUMB_MCP,
            handDataR.THUMB_IP,
            handDataR.THUMB_TIP,
            handDataR.INDEX_FINGER_MCP,
            handDataR.INDEX_FINGER_PIP,
            handDataR.INDEX_FINGER_DIP,
            handDataR.INDEX_FINGER_TIP,
            handDataR.MIDDLE_FINGER_MCP,
            handDataR.MIDDLE_FINGER_PIP,
            handDataR.MIDDLE_FINGER_DIP,
            handDataR.MIDDLE_FINGER_TIP,
            handDataR.RING_FINGER_MCP,
            handDataR.RING_FINGER_PIP,
            handDataR.RING_FINGER_DIP,
            handDataR.RING_FINGER_TIP,
            handDataR.PINKY_MCP,
            handDataR.PINKY_PIP,
            handDataR.PINKY_DIP,
            handDataR.PINKY_TIP
        }; 
        return localRightCoorArray;
    }
    // Update is called once per frame

    void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }

}
