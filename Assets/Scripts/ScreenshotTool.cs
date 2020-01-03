using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ScreenshotTool : MonoBehaviour
{
    public const string screenshotDirectory = "./screenshots";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (!Directory.Exists(screenshotDirectory))
            {
                Debug.Log("had to create directory");
                Directory.CreateDirectory(screenshotDirectory);
            }
            DateTime now = DateTime.Now;
            string savedAs = screenshotDirectory + "/" + now.Year + "." + now.Month + "." + now.Day + "_" + now.Hour + "." + now.Minute + "." + now.Second + ".png";
            ScreenCapture.CaptureScreenshot(savedAs);
            Debug.Log("Saved screenshot at " + savedAs);
        }
    }
}