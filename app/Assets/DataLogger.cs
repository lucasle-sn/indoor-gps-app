/*
 * Basic data logging for MCEN90032 Sensor Systems.
 * 
 * Author: Ricardo Garcia-Rosas
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLogger : MonoBehaviour
{
    public string fileName = "default.csv";
    public string dataFormat = "";  // Data written to file

    private StreamWriter fileWriter; 
    private string filePath; 

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // Prevents the screen from turning off and pausing the datalog
        InitialiseLogger();
    }


    /// <summary>
    /// Creates a new file for logging data.
    /// Useful when you want to log different sessions in different files.
    /// </summary>
    /// <param name="fileName">The new file's name and extension</param>
    /// <param name="dataFormat">Optional data format string. Will replace the data format string if provided.</param>
    public void CreateNewLogFile(string fileName, string dataFormat = "")
    {
        // First check that the file name is not empty
        if (fileName == string.Empty)
            throw new System.Exception("The provided file name is empty");

        this.fileName = fileName;
        if (dataFormat != string.Empty)
            this.dataFormat = dataFormat;

        InitialiseLogger();
    }


    public void InitialiseLogger()
    {
        // First check that the file name is not empty. If empty, throw exception
        if (fileName == string.Empty)
            throw new System.Exception("The provided file name is empty");

        filePath = Path.Combine(Application.persistentDataPath, fileName);
        FileStream fileStream = new FileStream(filePath, FileMode.Append);

        fileWriter = new StreamWriter(fileStream);
        fileWriter.WriteLine(dataFormat);
        SaveLog();
    }

    // The OnApplicationQuit routine is called whene the application is closed.
    private void OnApplicationQuit()
    {
        CloseLog();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Appends new data to the current Log file.
    /// The log uses strings to log data, so make sure the data provided to this
    /// method is already in the correct format.
    /// </summary>
    /// <param name="data">The string data to be appended.</param>
    public void AppendData(string data)
    {
        try
        {
            fileWriter.WriteLine(data);
        }
        catch (System.Exception e)
        {
            Debug.Log("Couldn't append data to the Log! :'(. Error:" + e.ToString());
        }
    }

    /// <summary>
    /// Saves the current log file.
    /// </summary>
    public void SaveLog()
    {
        if (fileWriter != null)
        {
            try
            {
                fileWriter.Flush();
            }
            catch (System.Exception e)
            {
                Debug.Log("Something went wrong when saving the Log! D:. Error:" + e.ToString());
            }
        }
    }

    /// <summary>
    /// Saves and closes the Log. Must be called on OnApplicationQuit.
    /// </summary>
    public void CloseLog()
    {
        if (fileWriter != null)
        {
            try
            {
                fileWriter.Close();
            }
            catch (System.Exception e)
            {
                Debug.Log("Something went wrong when saving the Log! D:. Error:" + e.ToString());
            }
        }
    }
}
