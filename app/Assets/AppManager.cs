/*
 * The University of Melbourne
 * School of Engineering
 * MCEN90032 Sensor Systems
 * Author: Quang Trung Le (987445)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppManager : MonoBehaviour
{
    public bool remoteMode = false;
    private bool logEnabled = false;

    // Modules
    public DataLogger dataLogger;
    public StepCounter stepCounter;
    public DistEstM1 distEstM1;
    public DistEstM2 distEstM2;
    public HeadingRelEst headingRelEst;
    public HeadingAbsEst headingAbsEst;
    public HeadingEst headingEst;
    public SensorReader sensorReader;


    //private string filename = "test1";
    public GameObject lbTracking;
    public GameObject lbReady;
    public GameObject lbFileCreated;

    public Toggle showSsDataFlag;
    public Toggle showInputFileName;



    // Step & Distance
    public TextMeshProUGUI lbStepCount;
    public TextMeshProUGUI lbCumStepCount;
    public TextMeshProUGUI lbDistance;

    public TextMeshProUGUI lbDistanceM1;
    public TextMeshProUGUI lbCumDistanceM1;    
    public TextMeshProUGUI lbDistanceM2;
    public TextMeshProUGUI lbCumDistanceM2;


    private int stepCount = 0;
    private int cumStepCount = 0;
    private float distanceM1 = 0;
    private float cumDistanceM1 = 0;
    private float distanceM2 = 0;
    private float cumDistanceM2 = 0;

    public Button btResetCounter;


    // HEading
    private float thetaRel = 0;
    private float phiRel = 0;
    private float psiRel = 0;    
    
    private float thetaAbs = 0;
    private float phiAbs = 0;
    private float psiAbs = 0;

    private float thetaKm = 0;
    private float phiKm = 0;
    private float psiKm = 0;

    public TextMeshProUGUI lbThetaRel;
    public TextMeshProUGUI lbPhiRel;
    public TextMeshProUGUI lbPsiRel;

    public TextMeshProUGUI lbThetaAbs;
    public TextMeshProUGUI lbPhiAbs;
    public TextMeshProUGUI lbPsiAbs;

    public TextMeshProUGUI lbThetaKm;
    public TextMeshProUGUI lbPhiKm;
    public TextMeshProUGUI lbPsiKm;

    public Button btResetHeading;

    // Start is called before the first frame update
    void Start()
    {
        SetSensorsEnable(true);
    }

    // Update is called once per frame
    void Update()
    {
        lbStepCount.text = stepCount.ToString();
        lbCumStepCount.text = cumStepCount.ToString();

        lbDistanceM1.text = distanceM1.ToString();
        lbCumDistanceM1.text = cumDistanceM1.ToString();        
        lbDistanceM2.text = distanceM2.ToString();
        lbCumDistanceM2.text = cumDistanceM2.ToString();


        lbThetaRel.text = thetaRel.ToString();
        lbPhiRel.text = phiRel.ToString();
        lbPsiRel.text = psiRel.ToString();        
        
        lbThetaAbs.text = thetaAbs.ToString();
        lbPhiAbs.text = phiAbs.ToString();
        lbPsiAbs.text = psiAbs.ToString();

        lbThetaKm.text = thetaKm.ToString();
        lbPhiKm.text = phiKm.ToString();
        lbPsiKm.text = psiKm.ToString();

    }

    // Fixed Update
    void FixedUpdate()
    {
        stepCount = stepCounter.GetStepCount();
        cumStepCount = stepCounter.GetCumStepCount();
        distanceM1 = distEstM1.GetDistance();
        cumDistanceM1 = distEstM1.GetCumDistance();        
        distanceM2 = distEstM2.GetDistance();
        cumDistanceM2 = distEstM2.GetCumDistance();


        thetaRel = headingRelEst.GetTheta();
        phiRel = headingRelEst.GetPhi();
        psiRel = headingRelEst.GetPsi();

        thetaAbs = headingAbsEst.GetTheta();
        phiAbs = headingAbsEst.GetPhi();
        psiAbs = headingAbsEst.GetPsi();

        thetaKm = headingEst.GetTheta();
        phiKm = headingEst.GetPhi();
        psiKm = headingEst.GetPsi();


        if (dataLogger != null && logEnabled)
        {
            string data = GetAppendData();
            if (data != "")
                dataLogger.AppendData(data);
        }
    }



    // ===== DATA LOGGER FUNCTIONS =====
    /// <summary>
    /// Updates the filename with the given name.
    /// Automatically assigns .csv extension.
    /// </summary>
    /// <param name="filename">The filename without extension</param>
    public void UpdateFileName(string filename)
    {
        if (filename == string.Empty)
            throw new System.ArgumentNullException("The filename is invalid");

        // Add extension and set logger filename
        filename += ".csv";
        dataLogger.fileName = filename;
    }


    /// <summary>
    /// Creates a new log file with the current filename.
    /// Automatically closes existing logs.
    /// </summary>
    public void CreateNewLog()
    {
        // Close existing log file
        dataLogger.CloseLog();

        // Create the new log
        dataLogger.InitialiseLogger();

        // Show the prompt
        StartCoroutine(DisplayCreatedPrompt());
    }

    /// <summary>
    /// Displays the "log created" message for 2 seconds.
    /// </summary>
    /// <returns>Yield instruction.</returns>
    private IEnumerator DisplayCreatedPrompt()
    {
        lbReady.SetActive(false);
        lbFileCreated.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        lbReady.SetActive(true);
        lbFileCreated.SetActive(false);
    }

    // Starts the data logging process.
    public void StartDataLogging()
    {
        //if (!logEnabled)
        //    SetSensorsEnable(true);

        SetLogEnabled(true);

        // Update the status UI
        lbTracking.SetActive(true);
        lbReady.SetActive(false);

        // Enable starttime flag
        stepCounter.SetStartTimeFlag(true);
        distEstM1.SetStartTimeFlag(true);
        distEstM2.SetStartTimeFlag(true);

        sensorReader.SetStartTimeFlag(true);
        headingAbsEst.SetStartTimeFlag(true);
        headingEst.SetStartTimeFlag(true);

        btResetCounter.interactable = false;
        btResetHeading.interactable = false;
    }

    /// <summary>
    /// Enables or disables the sensors.
    /// </summary>
    /// <param name="enable">Bool to enable/disable sensors.</param>
    public void SetSensorsEnable(bool enable)
    {
        // Enable gyro and compass
        Input.gyro.enabled = enable;
        Input.compass.enabled = enable;
    }

    // Handles stopping the data log.
    public void StopDataLogging()
    {
        if (logEnabled)
            StartCoroutine(StopLogging());
    }

    /// <summary>
    /// Coroutine to stop the data logger.
    /// Waits for the next FixedUpdate so the log is first disabled
    /// and then the file is closed.
    /// This avoids the file being closed while attempting to append data.
    /// Check Unity's documentation and tutorials for more information on Coroutines.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StopLogging()
    {
        //SetSensorsEnable(false);
        SetLogEnabled(false);

        // Wait for deltaTime seconds
        yield return new WaitForFixedUpdate();
        // Save log
        dataLogger.SaveLog();

        // Update the status UI
        lbTracking.SetActive(false);
        lbReady.SetActive(true);

        btResetCounter.interactable = true;
        btResetHeading.interactable = true;
    }


    // ==============================
    // Append data to file
    private string GetAppendData()
    {
        string data = sensorReader.GetAppendData();
        //data += "," + headingAbsEst.GetAppendData();
        data += "," + distanceM1 + "," + distanceM2;
        data += "," + thetaRel + "," + phiRel + "," + psiRel;
        data += "," + thetaAbs + "," + phiAbs + "," + psiAbs;
        data += "," + thetaKm + "," + phiKm + "," + psiKm;
        return data;
    }

    // Reset Sensor Counter
    public void ResetCounter()
    {
        stepCounter.SetStepCount(0);
        stepCounter.SetCumStepCount(0);
        distEstM1.SetDistance(0);
        distEstM1.SetCumDistance(0);        
        distEstM2.SetDistance(0);
        distEstM2.SetCumDistance(0);

    }

    // Reser Heading info
    public void ResetHeading()
    {
        headingAbsEst.ResetParameters();
        headingRelEst.ResetParameters();
        headingEst.ResetParameters();
    }

    public void ShowSsData(bool showSsDataEnabled)
    {
        //lbCumStepCount.enabled = showSsDataFlag.isOn;
    }

    public void ShowInputFileName(bool inputFileNameEnabled)
    {
    }


    // ===== DISTANCE ESTIMATOR FUNCTIONS =====
    public void UpdateHeight(string height)
    {
        if (height != string.Empty)
        {
            distEstM1.SetHeight(float.Parse(height));
        }
        
    }
    // ==============================

    private void SetLogEnabled(bool logEnabled)
    {
        this.logEnabled = logEnabled;
        stepCounter.SetLogEnabled(this.logEnabled);
        distEstM1.SetLogEnabled(this.logEnabled);
        distEstM2.SetLogEnabled(this.logEnabled);

        sensorReader.SetLogEnabled(this.logEnabled);
        headingRelEst.SetLogEnabled(this.logEnabled);
        headingAbsEst.SetLogEnabled(this.logEnabled);
        headingEst.SetLogEnabled(this.logEnabled);
    }

}

