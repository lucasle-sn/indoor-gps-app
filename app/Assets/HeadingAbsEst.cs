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

public class HeadingAbsEst : MonoBehaviour
{
    public SensorReader ssReader;

    // System setting
    private const double Ts = 0.02;
    private const double GRAVITY_FREQUENCY = 0.2;
    //private const double COEFF_GRAVITY_LP = Ts * 2 * Math.PI * GRAVITY_FREQUENCY;
    private const double COEFF_GRAVITY_LP = 0.025;


    // Variables for dataLog
    private bool logEnabled = false;
    private bool startTimeFlag = false;
    private string appendData = "";


    // Sensor data
    private double xAcc, yAcc, zAcc;
    private double xAccGrv = 0, yAccGrv = 0, zAccGrv = -1;
    private double xMag, yMag, zMag;
    private double xMagCalib, yMagCalib, zMagCalib;

    // Sensor Const
    private double[,] enu2ned = new double[3, 3] { { 0, 1, 0 }, { 1, 0, 0 }, { 0, 0, -1 } };
    private double[,] Cs = new double[3, 3] { { 1.0089, 0.0048, 0.0038 }, { 0.0048, 0.9921, -0.0027 }, { 0.0038, -0.0027, 0.9991 } };
    private double[,] dMag = new double[3, 1] { { 0.0538 }, { 0.0711 }, { -0.2280 } };



    // Calculation
    private float theta = 0, phi = 0, psi = 0;
    private double thetaRel = 0, phiRel = 0, psiRel = 0;
    private float theta0 = 0, phi0 = 0, psi0 = 0;
    private float inclAngle;
    private double[,] qam = new double[4, 1] { { 1},{ 0},{ 0},{ 0} };



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
            this.UpdateSensorData();
            this.GetMagCalib();
            this.GetAccGravity();
            

            if (logEnabled)
            {
                double[,] C = GetDCM();
                C = MatLib.MultiplyMatrix(C, enu2ned);
                double[] eulerAngle = MatLib.GetEulerAngle(C);

                theta = Convert.ToSingle(eulerAngle[0]);
                phi = Convert.ToSingle(eulerAngle[1]);
                psi = Convert.ToSingle(eulerAngle[2]);

                qam = MatLib.GetUnitQuaternion(C);
            }
        }
    }


    // get data from ss
    private void UpdateSensorData()
    {
        double[] acc = ssReader.GetAcc();
        xAcc = acc[0];
        yAcc = acc[1];
        zAcc = acc[2];

        double[] mag = ssReader.GetMag();
        xMag = mag[0];
        yMag = mag[1];
        zMag = mag[2];
    }

    // Calib Magnetometer
    private void GetMagCalib()
    {
        double[,] mag = new double[,] { { xMag }, { yMag }, { zMag } };
        double[,] magCorrected = MatLib.MultiplyMatrix(Cs, MatLib.AddMatrix(mag, dMag, -1));

        xMagCalib = (magCorrected[0, 0]);
        yMagCalib = (magCorrected[1, 0]);
        zMagCalib = (magCorrected[2, 0]);

        double magMag = Math.Sqrt(xMagCalib * xMagCalib + yMagCalib * yMagCalib + zMagCalib * zMagCalib);
        xMagCalib = xMagCalib / magMag;
        yMagCalib = yMagCalib / magMag;
        zMagCalib = zMagCalib / magMag;
    }

    // Get Gravity Acceleration
    private void GetAccGravity()
    {
        xAccGrv = (1 - COEFF_GRAVITY_LP) * xAccGrv + COEFF_GRAVITY_LP * xAcc;
        yAccGrv = (1 - COEFF_GRAVITY_LP) * yAccGrv + COEFF_GRAVITY_LP * yAcc;
        zAccGrv = (1 - COEFF_GRAVITY_LP) * zAccGrv + COEFF_GRAVITY_LP * zAcc;

        double mag = Math.Sqrt(xAccGrv * xAccGrv + yAccGrv * yAccGrv + zAccGrv * zAccGrv);
        xAccGrv = xAccGrv / mag;
        yAccGrv = yAccGrv / mag;
        zAccGrv = zAccGrv / mag;
    }

    // Get Directional Cosine MAtrix
    private double[,] GetDCM()
    {
        double[,] C = new double[3, 3];

        double[,] accGrv = new double[,] { { xAccGrv }, { yAccGrv }, { zAccGrv } };
        double[,] magCalib = new double[,] { { xMagCalib }, { yMagCalib }, { zMagCalib } };


        double[,] r3 = accGrv;
        double[,] r2 = MatLib.Cross(accGrv, magCalib);
        double[,] r1 = MatLib.Cross(r2, r3);

        r3 = MatLib.Normalize(r3);
        r2 = MatLib.Normalize(r2);
        r1 = MatLib.Normalize(r1);

        for (int i = 0; i < 3; i++)
        {
            C[i, 0] = r1[i, 0];
            C[i, 1] = r2[i, 0];
            C[i, 2] = r3[i, 0];
        }

        return C;
    }

    private double CalInclinationAngle(double[,] C)
    {
        double[,] magCalib = new double[,] { { xMagCalib }, { yMagCalib }, { zMagCalib } };

        double[,] Ctrans = MatLib.Transpose(C);
        double[,] magCalib_glb = MatLib.MultiplyMatrix(Ctrans, magCalib);
        double angle = Math.Atan2(magCalib_glb[2, 0], magCalib_glb[0, 0]);

        return Math.Round(MatLib.R2D(angle), 2);
    }

    private void SetRelativeHeading()
    {
        if (startTimeFlag)
        {
            theta0 = theta;
            phi0 = phi;
            psi0 = psi;

            startTimeFlag = false;
        }

        thetaRel = Math.Round(theta - theta0, 2);
        phiRel = Math.Round(phi - phi0,2);
        psiRel = Math.Round(psi - psi0,2);

        double[,] dcm = MatLib.GetDCMEuler(thetaRel, phiRel, psiRel);
        qam = MatLib.GetUnitQuaternion(dcm);
    }

    // Append data to csv
    public string AppendData()
    {
        string data = xAccGrv.ToString() + "," + yAccGrv.ToString() + "," + zAccGrv.ToString();
        data += "," + xMagCalib.ToString() + "," + yMagCalib.ToString() + "," + zMagCalib.ToString();

        return data;
    }


    public void ResetParameters()
    {
        //Calculation
        theta = 0; phi = 0; psi = 0;
        inclAngle = 0;
    }

    // ==================== GETTER & SETTER ====================
    public float GetTheta()
    {
        //return Convert.ToSingle(thetaRel);
        return theta;
    }

    public float GetPhi()
    {
        //return Convert.ToSingle(phiRel);
        return phi;
    }

    public float GetPsi()
    {
        //return Convert.ToSingle(psiRel);
        return psi;
    }

    public double[,] GetQam()
    {
        return qam;
    }

    public float GetInclAngle()
    {
        return inclAngle;
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
