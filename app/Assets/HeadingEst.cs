/*
 * The University of Melbourne
 * School of Engineering
 * MCEN90032 Sensor Systems
 * Author: Quang Trung Le (987445)
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadingEst : MonoBehaviour
{
    public HeadingRelEst headingRel;
    public HeadingAbsEst headingAbs;

    private float theta = 0, phi = 0, psi = 0;
    private float theta0 = 0, phi0 = 0, psi0 = 0;


    private double[,] qw;
    private double[,] qam = { { 1 }, { 0 }, { 0 }, { 0 } };
    private double[,] qk = { { 1 }, { 0 }, { 0 }, { 0 } };


    private double[,] Q = MatLib.MultiplyMatrix(Math.Pow(10, -10), MatLib.Eyes(4));
    private double[,] R = MatLib.MultiplyMatrix(Math.Pow(10, -3), MatLib.Eyes(4));

    private double[,] C = MatLib.Eyes(4);
    private double[,] Pk = MatLib.Zeros(4);

    // Variables for dataLog
    private bool logEnabled = false;
    private bool startTimeFlag = false;
    private string appendData = "";


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
        if (SystemInfo.supportsGyroscope && SystemInfo.supportsAccelerometer)
        {
            if (logEnabled)
            {
                //qw = headingRel.GetQw();
                qam = headingAbs.GetQam();

                if (startTimeFlag)
                {
                    qk[0, 0] = qam[0, 0];
                    qk[1, 0] = qam[1, 0];
                    qk[2, 0] = qam[2, 0];
                    qk[3, 0] = qam[3, 0];

                    startTimeFlag = false;
                }
                
                this.RunKalmanFilter();

                double[] eulerAngle = MatLib.GetEulerAngle(qk);

                theta = Convert.ToSingle(eulerAngle[0]);
                phi = Convert.ToSingle(eulerAngle[1]);
                psi = Convert.ToSingle(eulerAngle[2]);
            }
        }
    }


    // Fusion heading estimator
    private void RunKalmanFilter()
    {
        double[,] A = headingRel.GetFk();

        double[,] qkn = MatLib.MultiplyMatrix(A, qk);
        double[,] Pkn = MatLib.MultiplyMatrix(MatLib.MultiplyMatrix(A, Pk), MatLib.Transpose(A));
        Pkn = MatLib.AddMatrix(Pkn, Q,1);

        double[,] Kksub = MatLib.MultiplyMatrix(Pkn, MatLib.Transpose(C));
        double[,] Kk = MatLib.AddMatrix(MatLib.MultiplyMatrix(C, Kksub),R,1);
        Kk = MatLib.InvertMatrix44(Kk);
        Kk = MatLib.MultiplyMatrix(Kksub, Kk);

        qk = MatLib.AddMatrix(qam, MatLib.MultiplyMatrix(C, qkn), -1);
        qk = MatLib.AddMatrix(qkn, MatLib.MultiplyMatrix(Kk, qk), 1);
        Pk = MatLib.AddMatrix(MatLib.Eyes(4), MatLib.MultiplyMatrix(Kk, C), 1);
        Pk = MatLib.MultiplyMatrix(Pk, Pkn);
    }

    // Reset data
    public void ResetParameters()
    {
        //Calculation
        theta = 0; phi = 0; psi = 0;
    }



    // GETTER & SETTER
    public float GetTheta()
    {
        return Convert.ToSingle(theta);
        //return qw[1,0];
    }

    public float GetPhi()
    {
        return Convert.ToSingle(phi);
        //return sw[3, 0];
    }

    public float GetPsi()
    {
        return psi;
        //return zGyr;
    }

    public string GetAppendData()
    {
        return appendData;
    }

    public void SetLogEnabled(bool logEnabled)
    {
        this.logEnabled = logEnabled;
    }

    public void SetStartTimeFlag(bool startTimeFlag)
    {
        this.startTimeFlag = startTimeFlag;
    }
}
