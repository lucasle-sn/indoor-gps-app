/*
 * The University of Melbourne
 * School of Engineering
 * MCEN90032 Sensor Systems
 * Author: Quang Trung Le (987445)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class HeadingRelEst : MonoBehaviour
{
    public SensorReader ssReader;

    // System setting
    private const float Ts = 0.02f;

    // Variables for dataLog
    private bool logEnabled = false;


    // Sensor data
    private double xGyr, yGyr, zGyr;
    
    // Calculation
    private double[,] last_qw = {{1},{0},{0},{0}};
    private double[,] qw = { { 1 }, { 0 }, { 0 }, { 0 } };
    private double[,] Fk;
    private float theta = 0, phi = 0, psi = 0;
    //private float theta0 = -0.1850f, phi0 = 0.1069f, psi0 = 18.2455f;
    private float theta0 = 0, phi0 = 0, psi0 = 0;


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
        if (SystemInfo.supportsGyroscope)
        {
            this.UpdateSensorData();

            if (logEnabled)
            {
                qw = GetQuaternionGyr();
                double[] eulerAngle = MatLib.GetEulerAngle(qw);

                theta = Convert.ToSingle(eulerAngle[0] - theta0);
                phi = Convert.ToSingle(eulerAngle[1] - phi0);
                psi = Convert.ToSingle(eulerAngle[2] - psi0);

            }  
        }
    }

    // get data from ss
    private void UpdateSensorData()
    {
        double[] gyr = ssReader.GetGyr();

        xGyr = gyr[0];
        yGyr = gyr[1];
        zGyr = gyr[2];
    }

    // Calculate Quaternion
    private double[,] GetQuaternionGyr()
    {
        double[,] sw = new double[4, 1];

        sw[0, 0] = 0;
        sw[1, 0] = xGyr;
        sw[2, 0] = yGyr;
        sw[3, 0] = zGyr;

        double[,] SkewMat = MatLib.GetSkewSymMatrix(sw);
        Fk = MatLib.AddMatrix(MatLib.Eyes(4), MatLib.MultiplyMatrix(Ts / 2, SkewMat), 1);

        qw = MatLib.MultiplyMatrix(Fk, last_qw);

        last_qw = qw;
        return qw;
    }

    // Reset
    public void ResetParameters()
    {
        theta = 0; phi = 0; psi = 0;
        theta0 = 0; phi0 = 0; psi0 = 0;
    }

    // GETTER & SETTER
    public float GetTheta()
    {
        return theta;
        //return qw[1,0];
    }

    public float GetPhi()
    {
        return phi;
        //return sw[3, 0];
    }

    public float GetPsi()
    {
        return psi;
        //return zGyr;
    }

    public void SetLogEnabled(bool logEnabled)
    {
        this.logEnabled = logEnabled;
    }

    public double[,] GetQw()
    {
        return qw;
    }

    public double[,] GetFk()
    {
        return Fk;
    }
}
