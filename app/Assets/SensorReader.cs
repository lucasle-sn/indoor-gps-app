/*
 * The University of Melbourne
 * School of Engineering
 * MCEN90032 Sensor Systems
 * Author: Quang Trung Le (987445)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SensorReader : MonoBehaviour
{
    private double xAcc = 0, yAcc = 0, zAcc = 0;
    private double xGyr = 0, yGyr = 0, zGyr = 0;
    private double xMag = 0, yMag = 0, zMag = 0;

    private string appendData = "";
    private bool startTimeFlag = false;
    private float startTime = 0.0f;
    private float time = 0.0f;
    private bool logEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        this.UpdateSensorData();

        if (logEnabled)
        {
            if (startTimeFlag)
            {
                startTime = Time.time;
                startTimeFlag = false;
            }

            time = Time.time - startTime;

            appendData = AppendData();
        }


    }

    // Update sensor data
    private void UpdateSensorData()
    {
        if (SystemInfo.supportsAccelerometer)
        {
            xAcc = Input.acceleration.x;
            yAcc = Input.acceleration.y;
            zAcc = Input.acceleration.z;
        }

        if (SystemInfo.supportsGyroscope)
        {
            xGyr = Input.gyro.rotationRate.x;
            yGyr = Input.gyro.rotationRate.y;
            zGyr = Input.gyro.rotationRate.z;

            xMag = Input.compass.rawVector.x;
            yMag = Input.compass.rawVector.y;
            zMag = Input.compass.rawVector.z;
        }
    }

    // Get Append data
    private string AppendData()
    {
        string data = Math.Round(time, 2).ToString();
        data += "," + xAcc + "," + yAcc + "," + zAcc;
        data += "," + xGyr + "," + yGyr + "," + zGyr;
        data += "," + xMag + "," + yMag + "," + zMag;

        return data;
    }


    // ==================== GETTERS & SETTERS ====================
    public double[] GetAcc()
    {
        return new double[] { xAcc, yAcc, zAcc };
    }

    public double[] GetGyr()
    {
        return new double[] { xGyr, yGyr, zGyr };
    }

    public double[] GetMag()
    {
        return new double[] { xMag, yMag, zMag };
    }

    public void SetStartTimeFlag(bool startTimeFlag)
    {
        this.startTimeFlag = startTimeFlag;
    }

    public void SetLogEnabled(bool logEnabled)
    {
        this.logEnabled = logEnabled;
    }

    public string GetAppendData()
    {
        return appendData;
    }
}
