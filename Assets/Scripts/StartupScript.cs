using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.Cardboard;

public class StartupScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;
         if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Api.IsGearButtonPressed)
        {
            Api.ScanDeviceParams();
        }

        if (Api.IsCloseButtonPressed)
        {
            Application.Quit();
        }

        if (Api.IsTriggerHeldPressed)
        {
            Api.Recenter();
        }

        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }

        Api.UpdateScreenParams();
    }
}
