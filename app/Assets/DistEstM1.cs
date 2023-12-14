/*
 * The University of Melbourne
 * School of Engineering
 * MCEN90032 Sensor Systems
 * Author: Quang Trung Le (987445)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistEstM1 : MonoBehaviour
{
    public StepCounter stepCounter;

    private float distance = 0;
    private float cumDistance = 0;


    // Variables for distance measurement
    private float HEIGHT = 1.8f;
    private int stepCountLast2s = 0;
    private int count2s = 0;

    private bool logEnabled = false;
    private bool startTimeFlag = false;


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
        if (logEnabled)
        {
            if (startTimeFlag)
            {
                ResetParameter();
                startTimeFlag = false;
            }

            count2s = count2s + 1;
            if (count2s % 100 == 0)
            {
                count2s = 0;

                int stepCount = stepCounter.GetStepCount();

                int stepCount2s = stepCount - stepCountLast2s;
                float stride2s = GetStride2s(stepCount2s);

                distance += stride2s;
                cumDistance += stride2s;

                stepCountLast2s = stepCount;
            }
        }
    }


    // Get step Length
    private float GetStride2s(int count2s)
    {
        float stride2s;
        switch (count2s)
        {
            case 0:
                stride2s = 0f;
                break;
            case 1:
                stride2s = 1 / 5f;
                break;
            case 2:
                stride2s = 1 / 4f;
                break;
            case 3:
                stride2s = 1 / 3f;
                break;
            case 4:
                stride2s = 1 / 2f;
                break;
            case 5:
                stride2s = 1.0f / 1.2f;
                break;
            case 6:
            case 7:
                stride2s = 1f;
                break;
            default:
                stride2s = 1.2f;
                break;
        }

        stride2s = stride2s * HEIGHT * count2s;
        return stride2s;
    }

    // Reset
    private void ResetParameter()
    {
        distance = 0;

        stepCountLast2s = 0;
        count2s = 0;
    }



    // ===== GETTERS & SETTERS
    public void SetDistance(float distance)
    {
        this.distance = distance;
    }

    public float GetDistance()
    {
        return distance;
    }

    public void SetCumDistance(float cumDistance)
    {
        this.cumDistance = cumDistance;
    }

    public float GetCumDistance()
    {
        return cumDistance;
    }

    public void SetLogEnabled(bool logEnabled)
    {
        this.logEnabled = logEnabled;
    }

    public void SetStartTimeFlag(bool startTimeFlag)
    {
        this.startTimeFlag = startTimeFlag;
    }

    public void SetHeight(float height)
    {
        this.HEIGHT = height;
    }

}
