/*
 * The University of Melbourne
 * School of Engineering
 * MCEN90032 Sensor Systems
 * Author: Quang Trung Le (987445)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCounter : MonoBehaviour
{
    // Measured data
    private int stepCount = 0;
    private int cumStepCount = 0;


    // Variables for dataLog
    private string appendData = "";
    private bool logEnabled  = false;

    private bool startTimeFlag = false;
    private float startTime = 0.0f;


    // Sensor data
    private float xAcc, yAcc, zAcc;
    private float xGyr, yGyr, zGyr;
    private float xMag, yMag, zMag;
    private float time = 0.0f;


    // Variables for Pedometer
    private const float FILTER_COEFF_MAX = 0.38f;
    private const float FILTER_COEFF_MIN = 0.06f;
    private const float FILTER_COEFF_G = 0.025f;


    private float accMag = 0;
    private float accMagG = 1.0f;
    private float accMagLin_HP = 0.0f;
    private float accMagLin_BP = 0.0f;
    private float accMagLin_BP_lastMaxima = 0.0f;
    private float lastAccMagLin_BP = 0.0f;

    private bool stepCheckFlag = false;
    private bool firstStepCheckFlag = false;
    private bool realPeakUpdatedFlag = true;
    private bool maximaCheckFlag = true;


    private float lastPeakTime = 0;
    private float lastMaximaTime = 0;

    private const float THR = 0.1f;
    private const float SAMPLING_TIME = 0.02f;
    private const float THR_INTERVAL_MIN = 0.33f; // step 0.33-2 seconds
    private const float THR_INTERVAL_MAX = 2.0f;




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
        if (SystemInfo.supportsAccelerometer && SystemInfo.supportsGyroscope)
        {
            this.UpdateSensorData();

            if (logEnabled)
            {
                if (startTimeFlag)
                {
                    this.ResetParameters();
                    startTime = Time.time;
                    startTimeFlag = false;
                }

                time = Time.time - startTime;
                RunStepCount();
                
                //appendData = MergeAppendData();
            }

            this.ProcessAccMagnitude();
        }
    }


    // get data from ss
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

    // Filter magnitude
    private void ProcessAccMagnitude()
    {
        accMag = Mathf.Sqrt(xAcc * xAcc + yAcc * yAcc + zAcc * zAcc);
        accMagLin_HP = accMag - accMagG;
        accMagLin_BP = (1 - FILTER_COEFF_MAX) * accMagLin_BP + FILTER_COEFF_MAX * accMagLin_HP;
        accMagG = (1 - FILTER_COEFF_G) * accMagG + FILTER_COEFF_G * accMag;
    }

    private string MergeAppendData()
    {
        //Log the time and sensor data
        string data = System.Math.Round(time,2).ToString();
        data += "," + xAcc + "," + yAcc + "," + zAcc;
        data += "," + xGyr + "," + yGyr + "," + zGyr;
        data += "," + xMag + "," + yMag + "," + zMag;
        //data += "," + accMagLin_BP;

        return data;
    }

    // step count algorithm
    private void RunStepCount()
    {
        if (firstStepCheckFlag && (time - lastPeakTime > THR_INTERVAL_MAX))
        {
            stepCount --;
            cumStepCount --;
            firstStepCheckFlag = false;
        }

        if (accMagLin_BP >= THR)
            stepCheckFlag = true;
        else
        {
            stepCheckFlag = false;
            if (!realPeakUpdatedFlag)
            {
                CheckFirstPeak();
                UpdatePeak();
            }

            maximaCheckFlag = false;
            realPeakUpdatedFlag = true;
        }


        if (stepCheckFlag && (accMagLin_BP < lastAccMagLin_BP) && ((time - SAMPLING_TIME) - lastPeakTime >= THR_INTERVAL_MIN))
        {
            float newMaxima = time - SAMPLING_TIME;
            realPeakUpdatedFlag = false;

            if (!maximaCheckFlag)
            {
                lastMaximaTime = newMaxima;
                accMagLin_BP_lastMaxima = accMagLin_BP;
                maximaCheckFlag = true;
            }
            else
            {
                if (newMaxima - lastMaximaTime < THR_INTERVAL_MIN)
                    if (accMagLin_BP_lastMaxima < lastAccMagLin_BP)
                        lastMaximaTime = newMaxima;
                else
                {
                    CheckFirstPeak();
                    UpdatePeak();
                    lastMaximaTime = newMaxima;
                }
            }
        }

        lastAccMagLin_BP = accMagLin_BP;
    }



    // Reset all parameters
    private void ResetParameters()
    {
        // Variables for dataLog
        appendData = "";

        stepCount = 0;
        stepCheckFlag = false;
        firstStepCheckFlag = false;
        realPeakUpdatedFlag = true;
        maximaCheckFlag = true;


        lastPeakTime = 0;
        lastMaximaTime = 0;
    }

    // Check lone-peak
    private void CheckFirstPeak()
    {
        if (lastMaximaTime - lastPeakTime > THR_INTERVAL_MAX)
            firstStepCheckFlag = true;
        else
            firstStepCheckFlag = false;
    }

    // Update the peak
    private void UpdatePeak()
    {
        lastPeakTime = lastMaximaTime;
        stepCount ++;
        cumStepCount++;

        realPeakUpdatedFlag = true;
    }



    // ===== GETTERS & SETTERS =====
    public void SetLogEnabled(bool logEnabled)
    {
        this.logEnabled = logEnabled;
    }

    public string GetAppendData()
    {
        return appendData;
    }

    public int GetStepCount()
    {
        return stepCount;
    }

    public int GetCumStepCount()
    {
        return cumStepCount;
    }

    public void SetStartTimeFlag(bool startTimeFlag)
    {
        this.startTimeFlag = startTimeFlag;   
    }

    public void SetStepCount(int stepCount)
    {
        this.stepCount = stepCount;
    }

    public void SetCumStepCount(int cumStepCount)
    {
        this.cumStepCount = cumStepCount;
    }

}
