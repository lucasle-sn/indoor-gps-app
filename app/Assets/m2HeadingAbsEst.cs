using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class m2HeadingAbsEst : MonoBehaviour
{
    public SensorReader ssReader;

    // System setting
    private const double Ts = 0.02;
    private const double GRAVITY_FREQUENCY = 0.2;
    //private const double COEFF_GRAVITY_LP = Ts * 2 * Math.PI * GRAVITY_FREQUENCY;
    private const double COEFF_GRAVITY_LP = 0.025;


    // Variables for dataLog
    private bool logEnabled = false;
    private bool startFlag = false;
    private string appendData = "";


    // Sensor data
    private double xAcc, yAcc, zAcc;
    private double xAccGrv = 0, yAccGrv = 0, zAccGrv = -1;
    private double xMag, yMag, zMag;
    private double xMagCalib, yMagCalib, zMagCalib;

    // Sensor Const
    private double[,] enu2ned = new double[3, 3] { { 0, 1, 0 }, { 1, 0, 0 }, { 0, 0, -1 } };
    private double[,] Cs = new double[3, 3] { { 1.0089, 0.0048, 0.0038 }, { 0.0048, 0.9921, -0.0027 }, { 0.0038, -0.0027, 0.9991 } };
    private double[,] dMag = new double[3,1]{ { 0.0538 }, { 0.0711 }, { -0.2280 } };



    // Calculation
    private float theta = 0, phi = 0, psi = 0;
    private float theta0 = 0, phi0 = 0, psi0 = 0;
    private float inclAngle;

    private double[,] qa;
    private double[,] qm;
    private double[,] qam;
    private double[,] qamOrg;
    private double[,] qenu2ned = new double[,] { { 0 }, { 1 / Math.Sqrt(2) }, { 1 / Math.Sqrt(2) }, { 0 } };

    float test = 0;
    double Gamma = 0;


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
        this.GetMagCalib();
        this.GetAccGravity();

        test = 2;
        
        if (logEnabled)
        {
            test = 1;
            this.CalculateHeading();

            double[] eulerAngle = MatLib.GetEulerAngle(qam);

            theta = Convert.ToSingle(eulerAngle[0]);
            phi = Convert.ToSingle(eulerAngle[1]);
            psi = Convert.ToSingle(eulerAngle[2]);


            //this.SetRelativeHeading();

            appendData = this.AppendData();
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


    private void GetMagCalib()
    {
        double[,] mag = new double[,] { { xMag }, { yMag }, { zMag } };
        double[,] magCorrected = MatLib.MultiplyMatrix(Cs, MatLib.AddMatrix(mag, dMag, -1));

        xMagCalib = (magCorrected[0, 0]);
        yMagCalib = (magCorrected[1, 0]);
        zMagCalib = (magCorrected[2, 0]);

        //double magMag = Math.Sqrt(xMagCalib * xMagCalib + yMagCalib * yMagCalib + zMagCalib * zMagCalib);
        //xMagCalib = xMagCalib / magMag;
        //yMagCalib = yMagCalib / magMag;
        //zMagCalib = zMagCalib / magMag;
    }

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

    private double CalInclinationAngle(double[,] C)
    {
        double[,] magCalib = new double[,] { { xMagCalib }, { yMagCalib }, { zMagCalib } };

        double[,] Ctrans = MatLib.Transpose(C);
        double[,] magCalib_glb = MatLib.MultiplyMatrix(Ctrans, magCalib);
        double angle = Math.Atan2(magCalib_glb[2,0], magCalib_glb[0, 0]);

        return Math.Round(MatLib.R2D(angle),2);
    }

    private void CalculateHeading()
    {
        if (zAccGrv >= 0)
        {
            double lambda1 = Math.Sqrt((zAccGrv + 1) / 2);
            qa[0, 0] = lambda1;
            qa[1, 0] = yAccGrv / (2 * lambda1);
            qa[2, 0] = -xAccGrv / (2 * lambda1);
            qa[3, 0] = 0;
        }
        else
        {
            double lambda2 = Math.Sqrt((1 - zAccGrv) / 2);
            qa[0, 0] = yAccGrv / (2 * lambda2);
            qa[1, 0] = lambda2;
            qa[2, 0] = 0;
            qa[2, 0] = xAccGrv / (2 * lambda2);
        }

        double[,] magCalib = new double[,] { { xMagCalib }, { yMagCalib }, { zMagCalib } };
        double[,] l = MatLib.MultiplyMatrix(MatLib.GetDCMQuaternion(qa), magCalib);

        double lx = l[0, 0];
        double ly = l[1, 0];
         Gamma = (lx * lx + ly * ly);
        double GammaSqrt = Math.Sqrt(Gamma);

        if (lx>= 0)
        {
            qm[0, 0] = Math.Sqrt((Gamma + lx * GammaSqrt) / (2 * Gamma));
            qm[1, 0] = 0;
            qm[2, 0] = 0;
            qm[3, 0] = -ly / Math.Sqrt(2 * (Gamma + lx * GammaSqrt));
        }
        else
        {
            qm[0, 0] = -ly / Math.Sqrt(2 * (Gamma - lx * GammaSqrt));
            qm[1, 0] = 0;
            qm[2, 0] = 0;
            qm[3, 0] = Math.Sqrt((Gamma - lx * GammaSqrt) / (2 * Gamma));
        }

        qam = MatLib.CrossQuaternion(qa, qm);
        //qam = MatLib.CrossQuaternion(qam, qenu2ned);
        //qamOrg = qam;
    }

    private void SetRelativeHeading()
    {
        if (startFlag)
        {
            theta0 = theta;
            phi0 = phi;
            psi0 = psi;

            startFlag = false;
        }

        double thetaRel = theta - theta0;
        double phiRel = phi - phi0;
        double psiRel = psi - psi0;

        double[,] C = MatLib.GetDCMEuler(thetaRel, phiRel, psiRel);
        qam = MatLib.GetUnitQuaternion(C);
    }


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
        theta0 = 0; phi0 = 0; psi0 = 0;
        inclAngle = 0;
    }

    // GETTER & SETTER
    public float GetTheta()
    {
        return Convert.ToSingle(qa[0,0]);
        //return qw[1,0];
    }

    public float GetPhi()
    {
        return Convert.ToSingle(Gamma);
        //return sw[3, 0];
    }

    public float GetPsi()
    {
        return Convert.ToSingle(test);
        //return zGyr;
    }

    public double[,] GetQam()
    {
        return qam;
    }

    public string GetAppendData()
    {
        return appendData;
    }

    public void SetLogEnabled(bool logEnabled)
    {
        this.logEnabled = logEnabled;
    }

    public void SetStartFlag(bool startFlag)
    {
        this.startFlag = startFlag;
    }
}
