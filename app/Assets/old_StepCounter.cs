using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCounter_old : MonoBehaviour
{
    private string appendData = "";
    private bool logEnabled  = false;

    private float xAcc, yAcc, zAcc;
    private float xGyr, yGyr, zGyr;
    private float xMag, yMag, zMag;
    private float time = 0.0f;

    private const float FILTER_COEFF_MAX = 0.38f;
    private const float FILTER_COEFF_MIN = 0.06f;
    private const float FILTER_COEFF_g = 0.025f;


    private float accMag = 0;
    private float accMagG = 1.0f;
    private float accMagLin_HP = 0.0f;
    private float accMagLin_BP = 0.0f;
    private float lastAccMagLin_BP = 0.0f;

    private bool stepCheckFlag = false;
    private bool firstStepCheckFlag = false;
    private int stepCount = 0;
    private int cumStepCount = 0;

    private float lastPeak = 0;
    private const float THR = 0.15f;
    private const float SAMPLING_TIME = 0.02f;
    private const float THR_INTERVAL = 0.33f; // step 0.33-2 seconds
    private const float THR_INTERVAL_MAX = 2.0f;

    private bool startTimeFlag = false;
    private float startTime = 0.0f;


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
        if (SystemInfo.supportsAccelerometer == true)
        {
            getSensorData();

            if (logEnabled)
            {
                if (startTimeFlag)
                {
                    resetParameters();
                    startTime = Time.time;
                    startTimeFlag = false;
                }

                time = Time.time - startTime;
                countStep();
                appendData = mergeData();
            }

            updatAccMag();
        }
    }


    // get data from ss
    private void getSensorData()
    {
        xAcc = Input.acceleration.x;
        yAcc = Input.acceleration.y;
        zAcc = Input.acceleration.z;

        xGyr = Input.gyro.rotationRate.x;
        yGyr = Input.gyro.rotationRate.y;
        zGyr = Input.gyro.rotationRate.z;

        xMag = Input.compass.rawVector.x;
        yMag = Input.compass.rawVector.y;
        zMag = Input.compass.rawVector.z;
    }

    private void updatAccMag()
    {
        accMag = Mathf.Sqrt(xAcc * xAcc + yAcc * yAcc + zAcc * zAcc);
        accMagLin_HP = accMag - accMagG;
        accMagLin_BP = (1 - FILTER_COEFF_MAX) * accMagLin_BP + FILTER_COEFF_MAX * accMagLin_HP;
        accMagG = (1 - FILTER_COEFF_g) * accMagG + FILTER_COEFF_g * accMag;
    }

    private string mergeData()
    {
        //Log the time and sensor data
        string data = System.Math.Round(time,2).ToString();
        data += "," + xAcc + "," + yAcc + "," + zAcc;
        data += "," + accMagLin_BP;
        //dataLogger.AppendData(data);
        return data;
    }

    // step count algorithm
    public void countStep()
    {
        if ((accMagLin_BP >= THR) && (stepCheckFlag == false))
        {
            stepCheckFlag = true;
        }

        if ((firstStepCheckFlag == true) && (time - lastPeak > THR_INTERVAL_MAX))
        {
            stepCount = stepCount - 1;
            cumStepCount = cumStepCount - 1;

            firstStepCheckFlag = false;
        }

        if ((stepCheckFlag == true) && (accMagLin_BP < lastAccMagLin_BP) && ((time - SAMPLING_TIME) - lastPeak >= THR_INTERVAL)) // && (time - SAMPLING_TIME - timeWindow >= THR_WINDOW)
        {
            if (firstStepCheckFlag == false)
            {
                if ((time - SAMPLING_TIME - lastPeak) > THR_INTERVAL_MAX)
                    firstStepCheckFlag = true;
            }
            else
                firstStepCheckFlag = false;

            lastPeak = time - SAMPLING_TIME;
            stepCount = stepCount + 1;
            cumStepCount = cumStepCount + 1;
        }

        if ((stepCheckFlag == true) && (accMagLin_BP <= THR))
            stepCheckFlag = false;

        lastAccMagLin_BP = accMagLin_BP;
    }


    // Reset counter
    public void resetCounter()
    {
        stepCount = 0;
        cumStepCount = 0;
    }

    // Reset all parameters
    private void resetParameters()
    {
        startTime = 0.0f;
        time = 0f;
        stepCheckFlag = false;
        firstStepCheckFlag = false;
        stepCount = 0;
        lastPeak = 0;
    }



    // ===== GETTERS & SETTERS =====
    public void setLogEnable(bool logEnabled)
    {
        this.logEnabled = logEnabled;
    }

    public string getAppendData()
    {
        return appendData;
    }

    public int getStepCount()
    {
        return stepCount;
    }

    public int getCumStepCount()
    {
        return cumStepCount;
    }

    public void setStartTimeFlag(bool startTimeFlag)
    {
        this.startTimeFlag = startTimeFlag;   
    }
}
