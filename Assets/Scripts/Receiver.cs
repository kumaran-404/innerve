using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public int udpPort = 5678;
    private UdpClient udpClient;


    // Start is called before the first frame update
    void Start()
    {
        udpClient = new UdpClient(udpPort);
        udpClient.BeginReceive(ReceiveData, null);
    }

    private void ReceiveData(IAsyncResult result)
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, udpPort);
            byte[] receivedBytes = udpClient.EndReceive(result, ref endPoint);
            string receivedData = Encoding.ASCII.GetString(receivedBytes);

            Data d = JsonUtility.FromJson<Data>(receivedData);

            Debug.Log(d.ToString());
            Debug.Log(d.LeftHandExists);

            // Continue listening for more data
            udpClient.BeginReceive(ReceiveData, null);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error receiving UDP data: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}

class Data
{
    public bool LeftHandExists;
    public bool RightHandExists;
    public Hand Left;
}

class Hand
{

}